# Shared Components

When a single implementation is registered against multiple service types — for example, `CustomerService` registered as both `ICustomerService` and `IAuditable` — it is often desirable for every service type to resolve to the **same instance** within a given scope or process. This mirrors Castle Windsor's [shared component](https://github.com/castleproject/Windsor/blob/master/docs/registering-components-one-by-one.md#components-with-multiple-services-forwarded-types) model.

Microsoft's container does not do this by default. Adding two `Singleton` registrations for the same implementation type produces **two separate instances**:

```csharp
services.AddSingleton<ICustomerService, CustomerService>();
services.AddSingleton<IAuditable, CustomerService>();
// Two distinct CustomerService instances are created
```

A shared component fixes this: the implementation is registered once and every other service type resolves through that single registration. The result is one `Singleton` (or per-scope `Scoped`) instance behind every service type.

## How shared components work

Given a [service selector](service-selectors.md) that maps a single implementation to multiple service types, the library registers:

1. The implementation itself, either directly (if it is one of the selected service types) or under a hidden shared key.
2. A factory-based descriptor for every other service type that resolves through the implementation registration.

For example:

```csharp
services.AddSingleton(
    Classes.From(typeof(CustomerService))
        .AsAllNonSystemInterfaces()
);
```

Given:

```csharp
public interface ICustomerService { }
public interface IAuditable { }
public class CustomerService : ICustomerService, IAuditable { }
```

Conceptually, the container ends up with:

```
CustomerService    → CustomerService (singleton, hidden shared key)
ICustomerService   → resolves to the singleton above
IAuditable         → resolves to the singleton above
```

Resolving `ICustomerService` and `IAuditable` from the same provider yields the **same `CustomerService` instance**.

## Sharing modes

Sharing behavior is controlled by the `SharingMode` enum:

| Mode                          | Behavior                                                                                                                                                                                               |
|-------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `SharingMode.SharedComponent` | Default for `Singleton` and `Scoped`. The implementation is registered once and every service type resolves to it.                                                                                     |
| `SharingMode.Dependent`       | Each service type is registered as a factory that resolves the implementation. The implementation must already be registered somewhere — either as one of the selected service types or by the caller. |
| `SharingMode.Independent`     | Default for `Transient`. Every service type is registered independently — no shared instance.                                                                                                          |

## Lifetime methods

`ServiceLifetimeSelector` exposes lifetime methods that combine a `ServiceLifetime` with a sharing mode. Each returns a `ServiceSource`; finish the chain with `.ToServiceCollection()` or a bulk-add (`services.AddSingleton(...)`, `services.Add(...)`):

| Method                       | Sharing mode      | Behavior                                                                  |
|------------------------------|-------------------|---------------------------------------------------------------------------|
| `AsSingleton()`              | `SharedComponent` | One singleton instance shared across every selected service type.         |
| `AsSingletonDependent()`     | `Dependent`       | Singleton factories that resolve an implementation registered elsewhere.  |
| `AsSingletonIndependent()`   | `Independent`     | A separate singleton instance per service type.                           |
| `AsScoped()`                 | `SharedComponent` | One per-scope instance shared across every selected service type.         |
| `AsScopedDependent()`        | `Dependent`       | Per-scope factories that resolve an implementation registered elsewhere.  |
| `AsScopedIndependent()`      | `Independent`     | A separate per-scope instance per service type.                           |
| `AsTransient()`              | `Independent`     | A new instance per resolution. Sharing is not meaningful for transients.  |

Calling `AsLifetime(ServiceLifetime.Transient, SharingMode.SharedComponent)` or `AsLifetime(ServiceLifetime.Transient, SharingMode.Dependent)` throws `ArgumentException`. Sharing cannot apply to transient services, so the mismatch is surfaced eagerly rather than silently ignored.

The methods above apply **one** lifetime to the whole chain. To choose a lifetime **per implementation type**, use `AsLifetime(Func<Type, ServiceLifetime>)` or read it from an attribute with `AsLifetimeByAttribute` (see [Lifetime from attributes](#lifetime-from-attributes)). Both use `SharingMode.SharedComponent`, except that any component whose resolved lifetime is `Transient` is registered `Independent` — a transient can never share an instance.

## Lifetime from attributes

Instead of applying one lifetime to the whole chain, `AsLifetimeByAttribute` reads the lifetime from an **attribute applied to the implementation type**. This keeps the lifetime declaration next to the implementation it belongs to, so a single convention scan can register singletons, scoped services, and transients side by side. All overloads share the same rules:

- **Inherited attributes are inspected by default.** Each overload has a companion that takes a leading `bool inherited` parameter; pass `false` to consider only attributes declared directly on the implementation type.
- **No match means Singleton.** Implementation types without a matching attribute fall back to `ServiceLifetime.Singleton` — the same lifetime a skipped lifetime stage would use.
- **A single match is required.** If a type carries more than one matching attribute, an `AmbiguousMatchException` is thrown when the chain is enumerated.
- **Transient components are registered independently.** Sharing defaults to `SharingMode.SharedComponent`, but a component whose resolved lifetime is `Transient` is registered `Independent`, because a transient can never share an instance.

## `AsLifetimeByAttribute()`

Reads the lifetime from any attribute that implements the library's `IServiceLifetimeProvider` interface. The library ships a ready-made one — `[Lifetime]` — so the common case needs no custom attribute:

```csharp
services.Add(
    Classes.From(typeof(CustomerService), typeof(OrderService))
        .AsInterface()
        .AsLifetimeByAttribute()
);
```

Given:

```csharp
[Lifetime(ServiceLifetime.Singleton)]
public class CustomerService : ICustomerService { }

[Lifetime(ServiceLifetime.Scoped)]
public class OrderService : IOrderService { }
```

Registers:

```
CustomerService → ICustomerService (Singleton)
OrderService    → IOrderService    (Scoped)
```

`[Lifetime]` is declared `Inherited = false` — matching `[Keyed]` — so a lifetime does not flow to subclasses by default (this also keeps runtime and source-generated registration in agreement, since a source generator only sees attributes declared directly on a type). Types with no `IServiceLifetimeProvider` attribute fall back to `ServiceLifetime.Singleton`.

To declare lifetimes with your own attribute instead, implement `IServiceLifetimeProvider`:

```csharp
public interface IServiceLifetimeProvider
{
    ServiceLifetime Lifetime { get; }
}
```

Whether such a custom attribute is picked up on derived types follows *its* own `[AttributeUsage(Inherited = …)]`; pass `AsLifetimeByAttribute(inherited: false)` to ignore inherited attributes.

## `AsLifetimeByAttribute<TAttribute>(Func<TAttribute, ServiceLifetime>)`

Projects a specific attribute — one that need not know anything about `IServiceLifetimeProvider` — through a selector. `TAttribute` may be a concrete attribute type or an interface implemented by one or more attributes (marker-interface matching):

```csharp
Classes.FromThisAssembly()
    .BasedOn<IStore>()
    .AsInterface()
    .AsLifetimeByAttribute<LifestyleAttribute>(attribute => attribute.Lifetime)
```

Given:

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class LifestyleAttribute(ServiceLifetime lifetime) : Attribute
{
    public ServiceLifetime Lifetime => lifetime;
}

[Lifestyle(ServiceLifetime.Scoped)]
public class CustomerStore : IStore { }
```

Registers `CustomerStore → IStore (Scoped)`. Types without the attribute fall back to `ServiceLifetime.Singleton`. An `inherited` overload — `AsLifetimeByAttribute<TAttribute>(bool inherited, Func<TAttribute, ServiceLifetime>)` — controls whether inherited attributes are inspected.

## `AsLifetimeByAttribute(Type, Func<Attribute, ServiceLifetime>)`

The non-generic form, for when the attribute type is only known at runtime. The selector receives the matching attribute as `Attribute`, so it is cast before the lifetime is read:

```csharp
Classes.FromThisAssembly()
    .BasedOn<IStore>()
    .AsInterface()
    .AsLifetimeByAttribute(typeof(LifestyleAttribute), attribute => ((LifestyleAttribute)attribute).Lifetime)
```

This registers the same lifetimes as the generic overload above. An `inherited` overload — `AsLifetimeByAttribute(Type, bool inherited, Func<Attribute, ServiceLifetime>)` — is also available.

## `SharedComponent` vs `Dependent`

Both modes produce one shared instance behind every service type. They differ in **who registers the implementation**:

- `SharedComponent` registers the implementation once (under a hidden shared key if it wasn't one of the selected service types, or as itself if it was), then points every service type at that registration. The library handles everything.
- `Dependent` does not add a separate hidden registration for the implementation. The implementation must already be in the container — either because it appears in the selected service types (e.g. via a separate `AsSelf()` selection), or because the caller registered it separately.

> **Warning:** If you use `Dependent` and the implementation type is not registered anywhere, resolution will throw at runtime. Prefer `SharedComponent` (the default) unless you have a specific reason to manage the implementation registration yourself.

A safe `Dependent` example, where a separate `AsSelf()` registration ensures the implementation is in the container:

```csharp
services.AddSingleton(Classes.From(typeof(CustomerService)).AsSelf());
services.Add(
    Classes.From(typeof(CustomerService))
        .AsInterface<ICustomerService>()
        .AsSingletonDependent()
);
// CustomerService  → CustomerService (direct registration)
// ICustomerService → forwards to CustomerService
```

Another safe use is when the implementation is registered through plain `Microsoft.Extensions.DependencyInjection`:

```csharp
services.AddSingleton<CustomerService>();
services.Add(
    Classes.From(typeof(CustomerService))
        .AsInterface<ICustomerService>()
        .AsSingletonDependent()
);
// ICustomerService → forwards to the CustomerService registered above
```

## Single-service short-circuit

When a selector maps an implementation to **one** service type, sharing adds no value and is skipped. The resulting registration is identical to `Independent`:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .AsFirstInterface() // Selects only 1 service
    .AsSingleton()
// CustomerService → ICustomerService (direct registration, no shared component)
```

This means `AsSingleton()` and `AsScoped()` are always safe to use as the default — it only incurs the shared-component cost when multiple service types are involved.

## Open generic limitation

Microsoft's container does not support factory-based resolution of open generic types (see [dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)):

```csharp
// Throws at resolve time:
services.AddSingleton(typeof(IRepository<>), sp => sp.GetRequiredService(typeof(Repository<>)));
```

Because sharing relies on factory resolution, it cannot work for open generic implementations. Rather than silently producing registrations that fail at runtime, this case is detected and fails fast at registration time:

```csharp
Classes.FromAssemblyContaining(typeof(Repository<>))
    .BasedOn(typeof(IRepository<>))
    .AsAllNonSystemInterfaces() // maps Repository<> to multiple open generic interfaces
    .AsSingleton()
// Throws InvalidOperationException:
//   "Open generic services can not be forwarded."
```

To register open generics that target multiple service types, switch to `AsSingletonIndependent()` (or the scoped equivalent). Each service type will be an independent registration, which is the same behavior you would get from raw `services.AddSingleton(typeof(IFoo<>), typeof(Foo<>))` calls.

If having independent services is unacceptable then there may need to be separate service registration or design changes to the service (if the service has a code smell).
The `TypeFilter.ConstructedGenericTypes()` and `TypeFilter.GenericTypeDefinitions()` can be used to only select closed and open generic types respectively.

## Choosing the right method

| Scenario                                                                | Method                                                       |
|-------------------------------------------------------------------------|--------------------------------------------------------------|
| Multiple service types should share one instance                        | `AsSingleton()` / `AsScoped()`                               |
| Forward additional service types to an impl that's already registered   | `AsSingletonDependent()` / `AsScopedDependent()`             |
| Each service type should have its own instance                          | `AsSingletonIndependent()` / `AsScopedIndependent()`         |
| New instance on every resolution                                        | `AsTransient()`                                              |
| Open generic mapped to multiple service types                           | `AsSingletonIndependent()` / `AsScopedIndependent()`         |
| Lifetime declared per type by an attribute                              | `AsLifetimeByAttribute(...)`                                 |
| Lifetime computed per type by a delegate                                | `AsLifetime(Func<Type, ServiceLifetime>)`                    |
