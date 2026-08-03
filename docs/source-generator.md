# Compile-time registration with `[Service]`

`ZCrew.Extensions.DependencyInjection.Registration` ships a Roslyn source generator that turns the `[Service]` attribute family into a registration list built at compile time. It's the attribute-driven counterpart to the [`Classes` / `Types` scan](registration.md): instead of scanning assemblies at startup, the generator collects every `[Service]` declaration while compiling and emits a `Services.FromThisAssembly()` method you hand to your container.

The generator is packed inside the Registration NuGet package as an analyzer, so a single `<PackageReference Include="ZCrew.Extensions.DependencyInjection.Registration" />` gets you both the runtime API and the generator. There's nothing else to install.

## The attributes

Mark a class with `[Service]` to register it, then refine it with the modifier attributes. How you group the brackets makes no difference: `[Service, Scoped]` and `[Service][Scoped]` are the same thing.

```csharp
using ZCrew.Extensions.DependencyInjection.Registration;

[Service]                                          // singleton, registered as Clock
public class Clock;

[Service, Scoped]
[As<IHealthCheck>("Database"), As<IDatabaseHealthCheck>]   // one shared scoped instance...
public class DatabaseHealthCheck : IHealthCheck, IDatabaseHealthCheck;
// ...resolve the keyed one with [FromKeyedServices("Database")] IHealthCheck, the other as IDatabaseHealthCheck

[Service]
[As<IEmailSender>("smtp"), As<IEmailSender>("ses")]        // one instance, two keyed registrations of the same type
public class Emailer : IEmailSender;

[Service, Keyed("primary")]                        // the implementation's own registration is keyed
public class PrimaryDb : IDb;

[Service, As(typeof(IRepository<>))]               // open generics need the non-generic As(Type) form
public class InMemoryRepository<T> : IRepository<T>;
```

| Attribute                                  | What it does                                                                              |
|--------------------------------------------|-------------------------------------------------------------------------------------------|
| `[Service]`                                | Marks the type for registration. On its own, registers it against itself as a singleton.  |
| `[As<T>(key?)]` / `[As(Type, key?)]`       | Adds a service type, optionally keyed. Repeatable. Use the non-generic form for open generics. |
| `[Singleton]` / `[Scoped]` / `[Transient]` | Sets the lifetime. Defaults to `Singleton`. One per type at most.                          |
| `[Keyed(key)]`                             | Keys the implementation's own registration.                                               |

`[Service]` goes on classes and structs, is `Inherited = false`, and can't be repeated (`AllowMultiple = false`). A type has one registration, described by its modifier attributes. To register a type under several keys of the same service type, stack `[As<T>(key)]` like `Emailer` above rather than repeating `[Service]`.

### What gets registered

`[Service]` uses exactly the same rules as the fluent `Service.From(...).As(...)` path:

- **Registered against itself.** The implementation is always registered against itself, plus each `[As]` service type. A bare `[Service]` registers just the concrete type.
- **One shared instance.** For `Singleton` and `Scoped` with at least one `[As]` type, the implementation is registered once and the service types forward to it, so they all resolve to the same object. `Transient` registers each service type independently.
- **Keys are per service type.** Each `[As]` carries its own key, or none. `[Keyed]` keys the implementation's own registration. An `[As]` without a key stays unkeyed even when `[Keyed]` is present, and the same service type can appear more than once under different keys.
- **Open generics with service types** take the shared path, which Microsoft's container can't express for open generics ([dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)). Registering one throws at `Add` time. The generator emits it faithfully and lets the runtime raise the error.

### Where the attributes come from

The `[Service]` family is embedded by the generator. The attribute types are defined inside the generator and emitted as internal, compiler-only types into every assembly the generator runs in. They are not public types in the Registration assembly.

That means the attributes only exist where the generator is wired up. A stray `using ZCrew.Extensions.DependencyInjection.Registration;` in a project without the generator can't quietly reference `[Service]` and register nothing. It fails to compile instead.

## Using the generated registrations

The generator emits an assembly-local entry point returning a `ServiceFilter`:

```csharp
Services.FromThisAssembly()   // ZCrew.Extensions.DependencyInjection.Registration.Services
```

Add the lot, or narrow it down first:

```csharp
using ZCrew.Extensions.DependencyInjection.Registration;

// Every [Service] in this assembly:
services.Add(Services.FromThisAssembly());

// Or filter first. Filters return a ServiceFilter, so they chain and still work with Add:
services.Add(
    Services.FromThisAssembly().Where(service => service.ImplementationType.Namespace == "MyApp.Infrastructure")
);

// ToServiceCollection is the explicit terminal, same as Add:
Services.FromThisAssembly().BasedOn<IRepository>().ToServiceCollection(services);
```

`ServiceFilter` gives you filters and the terminal, and nothing else. No `Select`, `Append` or `Zip`, because the attributes already decided each service's types, key and lifetime and nothing downstream should be able to undo that.

Its filters (`Where`, `InNamespace`, `InSameNamespaceAs`, `NameEndsWith`, `GenericTypes`, `BasedOn`, `HasAttribute` / `HasAttributes`, and so on) mirror [`TypeFilter`](type-filters.md) and match on the implementation type. To filter on the declared service types instead, use `Where(service => service.ServiceTypes.Any(...))`.

### Everything has to be in one assembly

The generated `Services` class is `[Embedded]` and `internal`, so other assemblies can't see it. Your `[Service]` types and the `Services.FromThisAssembly()` call have to live in the same assembly. You can't declare `[Service]` types in one project and call the entry point from another.

## Diagnostics

`ServiceRegistrationAnalyzer` catches misuse of the attributes. All of these are errors.

| Rule      | Raised when                                                                                                                                                    |
|-----------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `ZCDI001` | A `[Keyed]` or `[As]` key is an array. Arrays compare by reference, so a fresh array never matches a lookup. Use something value-equatable: a string, enum or primitive. |
| `ZCDI002` | A modifier (`[As]`, `[Singleton]` / `[Scoped]` / `[Transient]`, `[Keyed]`) is on a type with no `[Service]`, so it does nothing.                                 |
| `ZCDI003` | An `[As]` names a service type the implementation isn't assignable to. (`As<T>` is deliberately unconstrained, so the analyzer checks this instead.)             |
| `ZCDI004` | A type has more than one of `[Singleton]`, `[Scoped]`, `[Transient]`.                                                                                           |

```csharp
[Service, Keyed(new[] { 1, 2 })]   // ZCDI001: use a value-equatable key
public class Bad;

[Scoped]                           // ZCDI002: no [Service]
public class Orphan;

[Service, As<IUnrelated>]          // ZCDI003: the type doesn't implement IUnrelated
public class Wrong;

[Service, Singleton, Scoped]       // ZCDI004: pick one lifetime
public class TwoLifetimes;
```

## Which one should you use?

| Use                     | When                                                                                 |
|-------------------------|--------------------------------------------------------------------------------------|
| `[Service]`             | You want each type to declare its own registration inline, resolved at compile time. |
| `Classes` / `Types`     | You want bulk registration by convention (base type, namespace, naming) at startup.  |

Both run on the same `Service` and `ServiceDescriptor` machinery underneath, so the registrations behave identically.

## Trimming and native AOT

This is the trim-safe path, and the reason to prefer it if you publish trimmed or AOT.

The generated `Service.From(typeof(Impl), …)` calls reference each implementation as a `typeof` literal, and `Service` marks the implementation with `[DynamicallyAccessedMembers]` for public constructors (so `ServiceDescriptor` can still create it) and interfaces (so the `As*` selectors and `ServiceFilter` can still see the hierarchy). Nothing is discovered at startup, so there's nothing for the trimmer to pull out from under you.

The reflection scan is the opposite case. Its entry points are `[RequiresUnreferencedCode]` and produce one `IL2026` per chain, because the trimmer removes unreferenced types before the scan ever runs.

The one gap nobody can close from here is open generic service types, which the Microsoft container resolves with `MakeGenericType`. That's a container limitation, not something this package can annotate away.
