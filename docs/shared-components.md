# Shared Components

When a single implementation is registered against multiple service types — for example, `PayPalPaymentGateway` registered as both `IPaymentGateway` and `IDisposable` — you sometimes want every service type to resolve to the **same instance** within a given scope or process. This mirrors Castle Windsor's [shared component](https://github.com/castleproject/Windsor/blob/master/docs/registering-components-one-by-one.md#components-with-multiple-services-forwarded-types) model.

Microsoft's container does not do this by default. Adding two `Singleton` registrations for the same implementation type produces **two separate instances**:

```csharp
services.AddSingleton<IPaymentGateway, PayPalPaymentGateway>();
services.AddSingleton<IDisposable, PayPalPaymentGateway>();
// Two distinct PayPalPaymentGateway instances are created
```

## When sharing applies

Sharing is **automatic** — there is no separate mode to configure. Whether an implementation is shared across its service types is decided from just two things:

1. The resolved **lifetime**. `Transient` registrations are never shared — a transient produces a new instance on every resolution by definition.
2. Whether the **implementation type is itself one of the selected service types**.

A shared component is produced only when **all** of the following hold:

- the lifetime is `Singleton` or `Scoped`,
- the implementation is mapped to **more than one** service type, and
- the implementation type is **one of those selected service types**.

In that case the implementation is registered once (as itself) and every other service type is registered as a factory that resolves back through it, so they all share a single instance. In every other case — a single service type, a transient lifetime, or a selection that does not include the implementation — each service type is registered directly against the implementation, independently, exactly like separate `services.AddSingleton(...)` calls.

## How to select the implementation as a service

The built-in interface selectors (`AsInterface`, `AsAllInterfaces`, `AsDefaultInterfaces`, …) map an implementation to its **interfaces only** — the concrete implementation type is not among them, so those registrations are independent. To share one instance you must include the implementation itself in the selected services, for example with a custom `As(...)` selection:

```csharp
services.AddSingleton(
    Classes.From(typeof(PayPalPaymentGateway))
        .As(type => type.GetInterfaces().Prepend(type)) // the implementation plus its interfaces
);
```

Given:

```csharp
public interface IPaymentGateway : IDisposable { }
public class PayPalPaymentGateway : IPaymentGateway { }
```

the container ends up with:

```
PayPalPaymentGateway → PayPalPaymentGateway (singleton, registered directly)
IPaymentGateway      → resolves to the PayPalPaymentGateway singleton
IDisposable          → resolves to the PayPalPaymentGateway singleton
```

Resolving `IPaymentGateway` and `IDisposable` from the same provider yields the **same `PayPalPaymentGateway` instance**. `AsServicesFromAttribute` shares in the same way when the attribute lists the implementation type among the provided services.

By contrast, `AsAllInterfaces().AsSingleton()` maps the implementation to `IPaymentGateway` and `IDisposable` **without** the implementation itself, so each interface is registered independently and resolves to its own instance.

## Lifetime methods

`ServiceLifetimeSelector` chooses the lifetime for the whole chain. Each returns a `ServiceSource`; finish the chain with `.ToServiceCollection()` or a bulk-add (`services.AddSingleton(...)`, `services.Add(...)`):

| Method          | Behavior                                                                                                                |
|-----------------|-------------------------------------------------------------------------------------------------------------------------|
| `AsSingleton()` | One instance per container. Shared across the selected service types when the implementation is one of them.            |
| `AsScoped()`    | One instance per scope. Shared across the selected service types within a scope when the implementation is one of them. |
| `AsTransient()` | A new instance on every resolution. Never shared.                                                                       |

To choose a lifetime **per implementation type**, use `AsLifetime(Func<Type, ServiceLifetime>)` or read it from an attribute with `AsLifetimeByAttribute` (see [Lifetime from attributes](#lifetime-from-attributes)). The same sharing rules are applied per component, based on the lifetime resolved for it.

## Single-service short-circuit

When a selector maps an implementation to **one** service type, there is nothing to share, so the implementation is registered directly against that service type:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .AsFirstInterface() // selects only one service
    .AsSingleton()
// CustomerService → ICustomerService (direct registration)
```

`AsSingleton()` and `AsScoped()` are therefore always safe as the default — factory forwarding only happens when multiple service types are involved and the implementation is one of them.

## Lifetime from attributes

Instead of applying one lifetime to the whole chain, `AsLifetimeByAttribute` reads the lifetime from an **attribute applied to the implementation type**. This keeps the lifetime declaration next to the implementation it belongs to, so a single convention scan can register singletons, scoped services, and transients side by side. All overloads share the same rules:

- **Inherited attributes are inspected by default.** Each overload has a companion that takes a leading `bool inherited` parameter; pass `false` to consider only attributes declared directly on the implementation type.
- **No match means Singleton.** Implementation types without a matching attribute fall back to `ServiceLifetime.Singleton` — the same lifetime a skipped lifetime stage would use.
- **A single match is required.** If a type carries more than one matching attribute, an `AmbiguousMatchException` is thrown when the chain is enumerated.
- **Transient components are never shared.** A component whose resolved lifetime is `Transient` registers each service type independently, because a transient can never share an instance.

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

## Open generic limitation

Microsoft's container does not support factory-based resolution of open generic types (see [dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)):

```csharp
// Throws at resolve time:
services.AddSingleton(typeof(IRepository<>), sp => sp.GetRequiredService(typeof(Repository<>)));
```

Because a shared component forwards its other service types through a factory, it cannot be produced for an open generic implementation. This case is detected and fails fast at registration time — but only when the shared-component path is actually taken (the open generic implementation is one of multiple selected service types under a `Singleton` or `Scoped` lifetime):

```csharp
Classes.FromAssemblyContaining(typeof(Repository<>))
    .BasedOn(typeof(IRepository<>))
    .As(type => type.GetInterfaces().Prepend(type)) // implementation plus its open generic interfaces
    .AsSingleton()
// Throws InvalidOperationException:
//   "Open generic services can not be forwarded."
```

Mapping an open generic implementation to its interfaces **without** including the implementation itself (the usual `AsInterface()` / `AsAllInterfaces()` case) registers each service type independently and does not throw — the same behavior you would get from raw `services.AddSingleton(typeof(IFoo<>), typeof(Foo<>))` calls.

If a shared single instance is required for an open generic, there may need to be separate service registration or design changes to the service (if the service has a code smell). The `TypeFilter.ConstructedGenericTypes()` and `TypeFilter.GenericTypeDefinitions()` can be used to only select closed and open generic types respectively.

## Choosing the right lifetime

| Scenario                                                                 | Method                                                                             |
|--------------------------------------------------------------------------|------------------------------------------------------------------------------------|
| Multiple service types (including the implementation) share one instance | `AsSingleton()` / `AsScoped()` with a selection that includes the implementation   |
| Each service type should have its own instance                           | Map interfaces only (e.g. `AsAllInterfaces()`) with `AsSingleton()` / `AsScoped()` |
| New instance on every resolution                                         | `AsTransient()`                                                                    |
| Lifetime declared per type by an attribute                               | `AsLifetimeByAttribute(...)`                                                       |
| Lifetime computed per type by a delegate                                 | `AsLifetime(Func<Type, ServiceLifetime>)`                                          |
