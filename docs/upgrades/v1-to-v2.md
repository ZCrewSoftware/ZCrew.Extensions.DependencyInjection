# Upgrading from v1 to v2

v2 of `ZCrew.Extensions.DependencyInjection.Registration` reshaped the fluent API around shared components and trimmed down the core interfaces. Most chains keep working as they are, but a few changes will break your build. This page lists them with the smallest migration that fixes each one.

The decorator library, `ZCrew.Extensions.DependencyInjection`, is unaffected.

## At a glance

| Change                                                       | Severity          | What to do                                                                                                                        |
|--------------------------------------------------------------|-------------------|-----------------------------------------------------------------------------------------------------------------------------------|
| `IServiceSource` no longer implements `IServiceCollection`   | High              | Add `.ToServiceCollection()`, or a lifetime helper, to the end of any chain you pass to an `IServiceCollection` API.               |
| Sharing behavior changed for `Singleton` and `Scoped`        | High (behavioral) | Check your multi-interface registrations. `.AsSingletonIndependent()` / `.AsScopedIndependent()` get the old instances back.       |
| Most fluent methods became extension methods                 | Low               | Recompile. Only breaks `nameof(...)`, custom `IServiceSelector` / `ITypeFilter` implementations, and reflection.                   |
| Open generic `AsInterface()` bug fix                         | Low               | Nothing, unless you were working around the bug.                                                                                  |

## `IServiceSource` no longer implements `IServiceCollection`

In v1 the end of the chain was an `IServiceCollection`:

```csharp
// v1
public interface IServiceSource : IServiceCollection { ... }
```

which meant you could pass a chain anywhere an `IServiceCollection` was wanted:

```csharp
// v1, compiled fine
var provider = Classes.FromThisAssembly()
    .BasedOn<IRepository>()
    .AsInterface()
    .AsLifetime(ServiceLifetime.Singleton)
    .BuildServiceProvider();
```

In v2 it's a type of its own, and you ask for the collection explicitly:

```csharp
// v2
public interface IServiceSource { ... }
```

```csharp
// v2, call .ToServiceCollection() or a lifetime helper first
var provider = Classes.FromThisAssembly()
    .BasedOn<IRepository>()
    .AsInterface()
    .AsSingleton()                 // returns IServiceCollection
    .BuildServiceProvider();
```

In v2 the lifetime helpers (`AsSingleton`, `AsScoped`, `AsTransient`, the `*Dependent` / `*Independent` variants, and `AsLifetime(...)`) returned an `IServiceCollection` directly. Since the `ServiceLifetimeSelector` split they return a `ServiceSource`, so call `.ToServiceCollection()`, or use a bulk add like `services.AddSingleton(...)` / `services.Add(...)`.

## Sharing behavior changed

This is the subtle one, because nothing about it fails to compile.

In v1, registering one class against several service types produced one `ServiceDescriptor` per service type. At singleton lifetime that meant one instance per service type, not one shared instance:

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
// gave you two different CustomerService instances
```

In v2 the default `SharingMode.SharedComponent` registers the class once and forwards every service type to it through a factory. Both resolve to the same singleton, or the same per-scope instance, matching Castle Windsor's shared component model.

```csharp
// v2
services.AddSingleton(
    Classes.From(typeof(CustomerService))
        .AsAllNonSystemInterfaces()
);

// provider.GetService<ICustomerService>()
// and provider.GetService<IAuditable>()
// now give you the same instance
```

If you were relying on separate instances, say because each interface had its own state, opt back in with the `*Independent` helpers or pass `SharingMode.Independent` to `AsLifetime`:

```csharp
.AsSingletonIndependent()
.AsScopedIndependent()
.AsLifetime(ServiceLifetime.Singleton, SharingMode.Independent)
```

Two related rules to know about in v2:

- **Open generics can't be shared.** Microsoft DI can't resolve open generics through a factory ([dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)). `SharedComponent` and `Dependent` throw an `InvalidOperationException` for open generic implementations, so use `Independent` there.
- **Transients can't be shared.** `AsLifetime(ServiceLifetime.Transient, sharingMode)` throws an `ArgumentException` when `sharingMode` isn't `Independent`. In v1 sharing on a transient was quietly ignored.

See [shared services](../shared-services.md) for the full model.

## Most fluent methods became extension methods

A large part of the API moved off `IServiceSelector` and `ITypeFilter` and onto static extension classes (`ServiceSelectorExtensions`, `TypeFilterExtensions.Namespace`, `TypeFilterExtensions.BasedOn`):

`AsInterface`, `AsAllInterfaces`, `AsAllNonSystemInterfaces`, `AsDefaultInterfaces`, `AsDefaultNonSystemInterfaces`, `AsFirstInterface`, `AsInterface<T>`, `AsInterfaces`, `AsSelf`, `AsBase`, `InNamespace`, `InSameNamespaceAs`, `BasedOn<T>`, `BasedOn(Type)`.

Chained call sites keep compiling. You'll only hit a break if:

- You used `nameof(IServiceSelector.AsInterface)` or similar, since extension methods aren't members of the interface.
- You wrote your own `IServiceSelector` / `ITypeFilter` implementation. Those members are gone from the interface, so delete them. The extensions handle it on top of the smaller core.
- You enumerated the interface's `MethodInfo`s by reflection, since the moved methods no longer show up there.

### `As(...)` delegate signatures tightened

Two overloads of `As(...)` changed their parameter types:

```csharp
// v1
IKeyedServiceSelector As(Func<Type, Type[]> typeSelector);
IKeyedServiceSelector As(Func<Type, Type[], Type[]> typeWithBaseTypesSelector);

// v2
IKeyedServiceSelector As(Func<Type, IEnumerable<Type>> serviceSelector);
IKeyedServiceSelector As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>> serviceSelector);
```

Inline lambdas returning arrays still compile, thanks to array covariance. You will need to update:

- Explicitly typed variables. `Func<Type, Type[]> f = ...; .As(f);` needs the new delegate type.
- Lambdas calling `.Length` on the `baseTypes` parameter. `IReadOnlyList<Type>` has `.Count`.

## Open generic `AsInterface()` bug fix

`AsInterface()` and its overloads used to misbehave when the implementation was an open generic implementing a generic interface. v2 collapses matched top-level interfaces to their generic type definition so the container can resolve them.

This is a straight bug fix. The only code it breaks is code that worked around the old behavior, for instance by registering the open generic interface through a separate path.
