# Upgrading from v1 to v2

v2 of `ZCrew.Extensions.DependencyInjection.Registration` reshaped the fluent API around shared-component semantics and trimmed the surface of the core interfaces. Most chains keep working unchanged, but a handful of changes are source-breaking — this page lists them with the minimum migration you need to make.

The decorator library (`ZCrew.Extensions.DependencyInjection`) is unaffected.

## At a glance

| Change                                                         | Severity          | What to do                                                                                                                                      |
|----------------------------------------------------------------|-------------------|-------------------------------------------------------------------------------------------------------------------------------------------------|
| `IServiceSource` no longer implements `IServiceCollection`     | High              | Add `.ToServiceCollection()` (or a lifetime helper) to the end of any chain you pass to an `IServiceCollection` API.                            |
| Default sharing semantics changed for `Singleton` / `Scoped`   | High (behavioral) | Audit multi-interface registrations. To restore the old per-service-type instances, use `.AsSingletonIndependent()` / `.AsScopedIndependent()`. |
| Most fluent methods moved from interfaces to extension methods | Low               | Recompile. Only breaks `nameof(...)`, custom `IServiceSelector` / `ITypeFilter` implementations, and reflection.                                |
| Open-generic `AsInterface()` bug fix                           | Low               | None, unless you were relying on the broken behavior.                                                                                           |

## `IServiceSource` no longer implements `IServiceCollection`

In v1 the terminal type of the registration chain *was* an `IServiceCollection`:

```csharp
// v1
public interface IServiceSource : IServiceCollection { ... }
```

This let you pass a chain directly anywhere an `IServiceCollection` was expected:

```csharp
// v1 — compiled fine
var provider = Classes.FromThisAssembly()
    .BasedOn<IRepository>()
    .AsInterface()
    .AsLifetime(ServiceLifetime.Singleton)
    .BuildServiceProvider();
```

In v2, `IServiceSource` is a standalone type. You materialize an `IServiceCollection` explicitly:

```csharp
// v2
public interface IServiceSource { ... }
```

```csharp
// v2 — call .ToServiceCollection() (or a lifetime helper) before treating it as one
var provider = Classes.FromThisAssembly()
    .BasedOn<IRepository>()
    .AsInterface()
    .AsSingleton()                 // returns IServiceCollection
    .BuildServiceProvider();
```

The lifetime helpers (`AsSingleton`, `AsScoped`, `AsTransient`, and the `*Dependent` / `*Independent` variants, plus `AsLifetime(...)`) all return `IServiceCollection` directly, so you usually do not need a separate `.ToServiceCollection()` call.

## Default sharing semantics changed

This is the most subtle change because nothing about it fails to compile.

In v1, registering one implementation against multiple service types produced **one `ServiceDescriptor` per service type**. At `Singleton` lifetime that meant **one instance per service type**, not one shared instance:

```csharp
public class CustomerService : ICustomerService, IAuditable { }

// v1
services.Add(
    Classes.From(typeof(CustomerService))
        .AsAllNonSystemInterfaces()
        .AsLifetime(ServiceLifetime.Singleton)
);

// provider.GetService<ICustomerService>()
// and provider.GetService<IAuditable>()
// returned two different CustomerService instances
```

In v2 the default `SharingMode.SharedComponent` registers the implementation once and forwards every selected service type to it via a factory. Both service types resolve to the **same** singleton or per-scope instance — mirroring Castle Windsor's shared-component model.

```csharp
// v2
services.Add(
    Classes.From(typeof(CustomerService))
        .AsAllNonSystemInterfaces()
        .AsSingleton()
);

// provider.GetService<ICustomerService>()
// and provider.GetService<IAuditable>()
// now return the same instance
```

If you depended on per-service-type instances (e.g. each interface had its own state), opt back into the old behavior with the `*Independent` lifetime helpers, or pass `SharingMode.Independent` to `AsLifetime`:

```csharp
.AsSingletonIndependent()
.AsScopedIndependent()
.AsLifetime(ServiceLifetime.Singleton, SharingMode.Independent)
```

Two related v2 rules worth knowing:

- **Open generics cannot be shared.** Microsoft DI does not support factory-based resolution of open generics ([dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)). `SharedComponent` and `Dependent` modes throw `InvalidOperationException` for open-generic implementations; use `Independent` for those.
- **Transient + sharing is rejected.** `AsLifetime(ServiceLifetime.Transient, sharingMode)` throws `ArgumentException` when `sharingMode != Independent` (in v1 sharing on transients was silently ignored).

See [Shared Components](../shared-components.md) for the full model.

## Most fluent methods moved from interfaces to extension methods

A large chunk of the API moved off `IServiceSelector` and `ITypeFilter` and onto static extension classes (`ServiceSelectorExtensions`, `TypeFilterExtensions.Namespace`, `TypeFilterExtensions.BasedOn`):

`AsInterface`, `AsAllInterfaces`, `AsAllNonSystemInterfaces`, `AsDefaultInterfaces`, `AsDefaultNonSystemInterfaces`, `AsFirstInterface`, `AsInterface<T>`, `AsInterfaces`, `AsSelf`, `AsBase`, `InNamespace`, `InSameNamespaceAs`, `BasedOn<T>`, `BasedOn(Type)`.

Chained call sites keep compiling. You will hit a break only if:

- You used `nameof(IServiceSelector.AsInterface)` (or similar) — extension methods are not members of the interface.
- You wrote your own `IServiceSelector` / `ITypeFilter` implementation — those members no longer exist on the interface, so remove them (the extensions delegate to the smaller core surface).
- You enumerated the interface's `MethodInfo`s via reflection — the moved methods will no longer appear.

### `As(...)` delegate signatures tightened

Two overloads of `As(...)` accept slightly broader/narrower parameter types:

```csharp
// v1
IKeyedServiceSelector As(Func<Type, Type[]> typeSelector);
IKeyedServiceSelector As(Func<Type, Type[], Type[]> typeWithBaseTypesSelector);

// v2
IKeyedServiceSelector As(Func<Type, IEnumerable<Type>> serviceSelector);
IKeyedServiceSelector As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>> serviceSelector);
```

Inline lambdas returning arrays still compile (array covariance). You will need to update:

- Explicitly typed variables: `Func<Type, Type[]> f = ...; .As(f);` → use the new delegate types.
- Lambdas that call `.Length` on the second parameter (`baseTypes`) — `IReadOnlyList<Type>` exposes `.Count`, not `.Length`.

## Open-generic `AsInterface()` bug fix

`AsInterface()` and its overloads previously misbehaved when the implementation was an open-generic type implementing a generic interface. v2 collapses matched top-level interfaces to their generic type definition so the DI container can resolve them.

This is a pure bug fix. It only breaks code that worked around the old behavior — for example, by registering the open-generic interface through a separate path.
