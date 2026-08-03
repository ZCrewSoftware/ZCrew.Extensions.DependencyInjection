# Compile-Time Registration with `[Service]`

`ZCrew.Extensions.DependencyInjection.Registration` ships a Roslyn source generator that turns the `[Service]`
attribute family into a compile-time registration list. It is the attribute-driven counterpart to the reflection-based
[`Classes` / `Types` scan](registration.md): instead of scanning assemblies at startup, the generator collects every
`[Service]` declaration during compilation and emits a `Services.FromThisAssembly()` method you add to your container.

The generator is packed **inside the Registration NuGet package** as an analyzer, so a single
`<PackageReference Include="ZCrew.Extensions.DependencyInjection.Registration" />` brings both the runtime API and the
generator - there is nothing extra to install.

## The `[Service]` attribute family

Mark an implementation with `[Service]` to register it, then refine the registration with the modifier attributes.
Bracket grouping is cosmetic: `[Service, Scoped]` and `[Service][Scoped]` mean the same thing.

```csharp
using ZCrew.Extensions.DependencyInjection.Registration;

[Service]                                          // singleton, registered as Clock
public class Clock;

[Service, Scoped]
[As<IHealthCheck>("Database"), As<IDatabaseHealthCheck>]   // one shared scoped instance…
public class DatabaseHealthCheck : IHealthCheck, IDatabaseHealthCheck;
// …resolve the keyed one with [FromKeyedServices("Database")] IHealthCheck and the unkeyed one as IDatabaseHealthCheck

[Service]
[As<IEmailSender>("smtp"), As<IEmailSender>("ses")]        // one instance, two keyed registrations of the same type
public class Emailer : IEmailSender;

[Service, Keyed("primary")]                        // the implementation's own registration is keyed
public class PrimaryDb : IDb;

[Service, As(typeof(IRepository<>))]               // open generics use the non-generic As(Type) form
public class InMemoryRepository<T> : IRepository<T>;
```

| Attribute                                  | Meaning                                                                                                  |
|--------------------------------------------|----------------------------------------------------------------------------------------------------------|
| `[Service]`                                | Marks the type for registration. On its own, registers the implementation against itself as a singleton. |
| `[As<T>(key?)]` / `[As(Type, key?)]`       | Adds a service type, optionally under a key. Repeatable. Use the non-generic form for open generics.     |
| `[Singleton]` / `[Scoped]` / `[Transient]` | Sets the lifetime. Defaults to `Singleton`. At most one per type.                                        |
| `[Keyed(key)]`                             | Keys the implementation's own registration.                                                              |

`[Service]` targets classes and structs, is `Inherited = false`, and is **not** repeatable (`AllowMultiple = false`) -
a type has exactly one registration, described by its modifier attributes. To register a type against multiple keys of
the same service type, stack `[As<T>(key)]` (as with `Emailer` above) rather than repeating `[Service]`.

### Registration semantics

`[Service]` reuses the exact semantics of the fluent `Service.From(...).As(...)` path:

- **Self-backing.** The implementation is always registered against itself, plus each `[As]` service type. `[Service]`
  with no `[As]` registers just the concrete type.
- **Shared instance.** For `Singleton` and `Scoped` lifetimes with one or more `[As]` service types, the implementation
  is registered once and the service types forward to it, so they all resolve to a single instance. `Transient`
  registers each service type independently.
- **Per-type keys.** Each `[As]` carries its own key (or none); `[Keyed]` keys the implementation's own registration.
  An `[As]` without a key is registered unkeyed even when `[Keyed]` is present. The same service type may appear more
  than once under different keys.
- **Open generics** with service types take the shared path, which Microsoft's container cannot express for open
  generics ([dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)); registering one throws at `Add`
  time. The generator still emits it faithfully - the runtime owns that error.

### Where the attributes come from

The `[Service]` family is **embedded by the generator**: the attribute types are defined inside the generator and
emitted (as `internal`, compiler-only types) into every assembly the generator runs in. They are not public types in
the Registration assembly. This means the attributes only exist where the generator is wired up, so a stray
`using ZCrew.Extensions.DependencyInjection.Registration;` in a project without the generator cannot silently
reference `[Service]` and register nothing - it fails to compile instead.

## Consuming the generated registrations

The generator emits an assembly-local entry point that returns a `ServiceFilter`:

```csharp
Services.FromThisAssembly()   // ZCrew.Extensions.DependencyInjection.Registration.Services - returns a ServiceFilter
```

Add the whole set, or narrow it first with the `ServiceFilter` filters:

```csharp
using ZCrew.Extensions.DependencyInjection.Registration;

// Add every [Service] in this assembly:
services.Add(Services.FromThisAssembly());

// Or filter, then add - the filters each return a ServiceFilter, so they chain and compose with Add:
services.Add(
    Services.FromThisAssembly().Where(service => service.ImplementationType.Namespace == "MyApp.Infrastructure")
);

// ToServiceCollection is the explicit terminal, equivalent to Add:
Services.FromThisAssembly().BasedOn<IRepository>().ToServiceCollection(services);
```

`ServiceFilter` deliberately exposes only filters and the terminal - no raw LINQ (`Select`, `Append`, `Zip`, ...) -
because the `[Service]` family already decided each service's types, key, and lifetime and nothing downstream may
clobber them. Its convenience filters (`Where`, `InNamespace`, `InSameNamespaceAs`, `NameEndsWith`, `GenericTypes`,
`BasedOn`, `HasAttribute`/`HasAttributes`, ...) mirror [`TypeFilter`](type-filters.md) and match on the service's
**implementation type**; to filter on the declared service types instead, use
`Where(service => service.ServiceTypes.Any(...))`.

### Same-assembly requirement

The generated `Services` class is `[Embedded]` and `internal`, so it is invisible to other assemblies. The `[Service]`
types **and** the `Services.FromThisAssembly()` call site must live in the same assembly - you cannot declare
`[Service]` types in one project and consume the entry point from another.

## Diagnostics

The `ServiceRegistrationAnalyzer` reports misuse of the attribute family. All rules are errors.

| Rule      | Reported when…                                                                                                                                                     |
|-----------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `ZCDI001` | A `[Keyed]` or `[As]` key is an array. Arrays compare by reference, so a fresh array never matches a lookup - use a value-equatable key (string, enum, primitive). |
| `ZCDI002` | A modifier attribute (`[As]`, `[Singleton]`/`[Scoped]`/`[Transient]`, `[Keyed]`) is used on a type with no `[Service]`; the modifier has no effect.                |
| `ZCDI003` | An `[As]` service type is one the implementation is not assignable to. (`As<T>` is intentionally unconstrained, so this analyzer enforces assignability instead.)  |
| `ZCDI004` | A type carries more than one of `[Singleton]`, `[Scoped]`, `[Transient]`.                                                                                          |

```csharp
[Service, Keyed(new[] { 1, 2 })]   // ZCDI001: use a value-equatable key
public class Bad;

[Scoped]                           // ZCDI002: no [Service]
public class Orphan;

[Service, As<IUnrelated>]          // ZCDI003: the type does not implement IUnrelated
public class Wrong;

[Service, Singleton, Scoped]       // ZCDI004: pick one lifetime
public class TwoLifetimes;
```

## When to use which

| Use…                         | When…                                                                                     |
|------------------------------|-------------------------------------------------------------------------------------------|
| `[Service]` + generator      | You want each type to declare its own registration inline, resolved at compile time.      |
| `Classes` / `Types` scan     | You want convention-based bulk registration (by base type, namespace, naming) at startup. |

Both share the same runtime `Service`/`ServiceDescriptor` machinery, so their registrations behave identically.

## Trimming and Native AOT

This is the trim-safe registration path, and the reason to prefer it when publishing trimmed or AOT. The generated
`Service.From(typeof(Impl), …)` calls reference each implementation as a `typeof` literal, and `Service` annotates the
implementation with `[DynamicallyAccessedMembers]` for public constructors (so `ServiceDescriptor` can still activate
it) and interfaces (so the `As*` selectors and `ServiceFilter` still see the hierarchy). Nothing is discovered at
startup, so there is nothing for the trimmer to remove out from under you.

The reflection-based `Classes`/`Types` scan is the opposite case: its entry points are annotated
`[RequiresUnreferencedCode]` and produce one `IL2026` per chain, because the trimmer removes unreferenced types before
the scan can see them.

The remaining unavoidable gap is **open generic** service types, which the Microsoft container resolves via
`MakeGenericType`; that is a container limitation rather than something this package can annotate away.
