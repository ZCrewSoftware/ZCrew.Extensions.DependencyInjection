# Compile-Time Registration with `[Service]`

`ZCrew.Extensions.DependencyInjection.Registration` ships a Roslyn source generator that turns the `[Service]`
attribute into a compile-time registration list. It is the attribute-driven counterpart to the reflection-based
[`Classes` / `Types` scan](registration.md): instead of scanning assemblies at startup, the generator collects every
`[Service]` declaration during compilation and emits a `Services.FromThisAssembly()` method you add to your container.

The generator is packed **inside the Registration NuGet package** as an analyzer, so a single
`<PackageReference Include="ZCrew.Extensions.DependencyInjection.Registration" />` brings both the runtime API and the
generator - there is nothing extra to install.

## The `[Service]` attribute

Annotate an implementation with `[Service]` to register it. Each attribute instance describes one registration:

```csharp
using Microsoft.Extensions.DependencyInjection;
using ZCrew.Extensions.DependencyInjection.Registration;

[Service]                                                                 // self, singleton
public class Clock;

[Service(typeof(IFoo), typeof(IBar), Lifetime = ServiceLifetime.Scoped)]  // self + IFoo + IBar, one shared instance
public class FooBar : IFoo, IBar;

[Service(typeof(IEmailSender), Key = "smtp")]                             // keyed…
[Service(typeof(IEmailSender), Key = "ses")]                             // …twice: two registrations, one type
public class Emailer : IEmailSender;
```

| Member                                         | Meaning                                                                                           |
|------------------------------------------------|---------------------------------------------------------------------------------------------------|
| `ServiceAttribute(params Type[] serviceTypes)` | The service types to register the implementation against, **beyond itself**.                      |
| `Lifetime` (init)                              | The registration lifetime. Defaults to `ServiceLifetime.Singleton`.                               |
| `Key` (init)                                   | An optional service key. When set, the implementation and its service types are registered keyed. |

`[Service]` targets classes and structs, is `Inherited = false`, and allows multiples (`AllowMultiple = true`) - a
single type can carry several `[Service]` attributes, each producing an independent registration, disambiguated by key.

### Registration semantics

`[Service]` reuses the exact semantics of the fluent `Service.From(...).As(...)` path:

- **Self-backing.** The implementation is always registered against itself, plus each listed service type. `[Service]`
  with no types registers just the concrete type.
- **Shared instance.** For `Singleton` and `Scoped` lifetimes with more than one service type, the implementation is
  registered once and the remaining service types are forwarded to it, so they all resolve to a single instance.
  `Transient` registers each service type independently.
- **Keys.** A `Key` applies to the implementation and all forwarded service types.
- **Open generics** with multiple service types take the shared path, which Microsoft's container cannot express for
  open generics ([dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)); registering one throws at
  `Add` time. The generator still emits it faithfully - the runtime owns that error.

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
because the `[Service]` attribute already decided each service's types, key, and lifetime and nothing downstream may
clobber them. Its convenience filters (`Where`, `InNamespace`, `InSameNamespaceAs`, `NameEndsWith`, `GenericTypes`,
`BasedOn`, `HasAttribute`/`HasAttributes`, ...) mirror [`TypeFilter`](type-filters.md) and match on the service's
**implementation type**; to filter on the declared service types instead, use
`Where(service => service.ServiceTypes.Any(...))`.

### Same-assembly requirement

The generated `Services` class is `[Embedded]` and `internal`, so it is invisible to other assemblies. The `[Service]`
types **and** the `Services.FromThisAssembly()` call site must live in the same assembly - you cannot declare
`[Service]` types in one project and consume the entry point from another.

## `ZCDI001` - array keys

Service keys resolve by their type's default equality, and arrays compare by reference, so a fresh array instance never
matches a lookup. The `RegistrationKeyAnalyzer` reports **`ZCDI001`** ("Registration key cannot be an array") when the
`Key` is an array:

```csharp
[Service(typeof(IFoo), Key = new[] { 1, 2 })]  // ZCDI001: use a value-equatable key (string, enum, primitive)
public class Bad : IFoo;
```

The check targets the `Key` named argument only - the `params Type[] serviceTypes` constructor list is a legitimate
positional array and is never mistaken for a key. Use a value-equatable key such as a string, enum, or primitive
instead.

## When to use which

| Use…                         | When…                                                                                     |
|------------------------------|-------------------------------------------------------------------------------------------|
| `[Service]` + generator      | You want each type to declare its own registration inline, resolved at compile time.      |
| `Classes` / `Types` scan     | You want convention-based bulk registration (by base type, namespace, naming) at startup. |

Both share the same runtime `Service`/`ServiceDescriptor` machinery, so their registrations behave identically.
