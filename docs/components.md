# Components

A **component** is a single concrete type registered against one or more services.
Where [`Classes` and `Types`](type-selectors.md) scan an assembly and apply a convention to everything they find, `Component` starts
from one type you already know, so there is no [type filtering](type-filters.md) or [service selection](service-selectors.md) stage.
This mirrors Castle Windsor's [`Component.For<T>()`](https://github.com/castleproject/Windsor/blob/master/docs/registering-components-one-by-one.md) model.

Every service added to a component is **forwarded to the implementation**, so a `Singleton` or `Scoped` component
resolves to a single shared instance across all of its services.

```csharp
services.Add(Component.From(typeof(PayPalPaymentGateway)).As<IPaymentGateway, IDisposable>());
```

## `Component.From(Type)`

Begins a component from a concrete type. The component starts registered **against the type itself**:

```csharp
services.Add(Component.From<CustomerService>());
```

Registers:

```
CustomerService → CustomerService (Singleton)
```

This is the one place the component API differs from the fluent chain. `Classes.From(...)` treats "register as self" as
a *default* that any `As*` selector replaces; `Component.From` treats it as the *starting point* that `As` adds to.
The implementation is therefore always among a component's services, which is what makes sharing automatic and makes
this a *component* by default.

## `As<T1>()` … `As<T1, …, T8>()`

Adds services to the component, for one to eight types. Each service is forwarded to the implementation:

```csharp
services.Add(Component.From<PayPalPaymentGateway>().As<IPaymentGateway, IDisposable>());
```

Given:

```csharp
public interface IPaymentGateway : IDisposable { }
public class PayPalPaymentGateway : IPaymentGateway { }
```

Registers:

```
PayPalPaymentGateway → PayPalPaymentGateway (registered directly)
IPaymentGateway      → resolves to the PayPalPaymentGateway instance
IDisposable          → resolves to the PayPalPaymentGateway instance
```

Resolving `IPaymentGateway` and `IDisposable` from the same provider yields the **same** `PayPalPaymentGateway` instance.

## `As(Type)` / `As(IEnumerable<Type>)`

The non-generic forms, for service types only known at runtime:

```csharp
services.Add(Component.From<PayPalPaymentGateway>().As(typeof(IPaymentGateway)));
services.Add(Component.From<PayPalPaymentGateway>().As([typeof(IPaymentGateway), typeof(IDisposable)]));
```

`As` **accumulates** rather than replaces, so calls can be chained and mixed freely with the generic overloads:

```csharp
Component.From<PayPalPaymentGateway>()
    .As<IPaymentGateway>()
    .As(typeof(IDisposable))
// PayPalPaymentGateway, IPaymentGateway, IDisposable
```

Duplicate services are allowed and are collapsed when the component is registered, so `As<IPaymentGateway>().As<IPaymentGateway>()` produces one `IPaymentGateway` registration.

## Selecting services by convention

The `As*` selectors from the [registration chain](service-selectors.md) also work on a component, so services can be
chosen by convention without giving up the shared instance:

```csharp
services.Add(Component.From<PayPalPaymentGateway>().AsAllNonSystemInterfaces());
```

Given:

```csharp
public interface IPaymentGateway : IDisposable { }
public class PayPalPaymentGateway : IPaymentGateway { }
```

Registers:

```
PayPalPaymentGateway → PayPalPaymentGateway (registered directly)
IPaymentGateway      → resolves to the PayPalPaymentGateway instance
                       (IDisposable is in System and is excluded)
```

Selection **accumulates onto the implementation** rather than replacing it, so the implementation stays first among the
services and the component stays shared. The same selector on a chain drops it:
`Classes.From(typeof(PayPalPaymentGateway)).AsAllNonSystemInterfaces()` registers `IPaymentGateway` alone, with no
`PayPalPaymentGateway` registration for it to share.

| Method                                                                        | Service types added                                                                      |
|-------------------------------------------------------------------------------|------------------------------------------------------------------------------------------|
| `AsAllInterfaces()`                                                           | Every interface implemented                                                              |
| `AsAllNonSystemInterfaces()`                                                  | Every interface except `System.*`                                                        |
| `AsDefaultInterfaces()`                                                       | Interfaces whose name appears in the class name (`CustomerService` → `ICustomerService`) |
| `AsDefaultNonSystemInterfaces()`                                              | Default interfaces, excluding `System.*`                                                 |
| `AsFirstInterface()`                                                          | The first interface in metadata order                                                    |
| `AsAllTypes()` / `AsAllNonSystemTypes()`                                      | Like the `Interfaces` variants, plus every non-abstract base class                       |
| `AsDefaultTypes()` / `AsDefaultNonSystemTypes()`                              | Like the above, restricted to convention-matching names                                  |
| `AsServicesFromAttribute([bool])`                                             | Service types from an `IServiceTypesProvider` attribute such as `[Services(...)]`        |
| `AsServicesFromAttribute<TAttribute>(…)` / `AsServicesFromAttribute(Type, …)` | Service types projected from any attribute                                               |

Per-method semantics are identical to the chain — see [service selectors](service-selectors.md). Selectors chain and
accumulate here too, so `AsDefaultInterfaces().AsAllInterfaces()` registers the distinct union of both.

### When nothing matches, the implementation is still registered

A selector that finds no services leaves the component untouched rather than emptying it:

```csharp
services.Add(Component.From<Customer>().AsDefaultInterfaces());
// Customer → Customer (Singleton) — Customer has no interfaces
```

The chain registers *nothing* in this situation. This is also why a component has no `AsSelf()` and no `*OrSelf`
selectors: the implementation is seeded from the start, so "or self" is already the behavior — `AsAllInterfacesOrSelf()`
would do exactly what `AsAllInterfaces()` does here. `AsBase()` and `AsInterface()` are absent for a different reason:
they read the base types set by [`BasedOn`](type-filters.md), and a component has no filtering stage to set them.

### Attribute selectors are validated too

Attribute-selected services go through the same check as any other `As` call, so an attribute naming a service type the
implementation isn't based on throws:

```csharp
[Contract(typeof(IProvidedServiceA))]
public class ContractBase;  // ...but does not implement IProvidedServiceA

Component.From<ContractBase>().AsServicesFromAttribute<ContractAttribute>(attribute => attribute.Contracts)
// Throws ArgumentException:
//   "The implementation ContractBase is not based on the service type IProvidedServiceA"
```

The chain accepts this — it registers whatever the attribute names without checking.

## Lifetime

A component with no lifetime set registers as `Singleton` — the same default a [skipped lifetime stage](shared-components.md#lifetime-methods) uses. To choose another, call `AsLifetime`:

```csharp
services.Add(
    Component.From<OrderService>()
        .As<IOrderService>()
        .AsLifetime(ServiceLifetime.Scoped)
);
```

`AsLifetime(Func<Type, ServiceLifetime>)` takes the lifetime from a delegate that receives the implementation type, matching the chain's [per-type lifetime](shared-components.md#lifetime-methods) helper.

`Transient` components are **never shared** — a transient produces a new instance on every resolution by definition, so each service type is registered independently against the implementation:

```csharp
Component.From<PayPalPaymentGateway>()
    .As<IPaymentGateway>()
    .AsLifetime(ServiceLifetime.Transient)
// PayPalPaymentGateway → PayPalPaymentGateway (transient)
// IPaymentGateway      → PayPalPaymentGateway (transient, registered directly)
```

## Service keys

`Keyed(object?)` assigns one key to every service on the component, and `Unkeyed()` removes any key. See [service key selectors](service-key-selectors.md) for how keys behave:

```csharp
services.Add(Component.From<StripePaymentGateway>().As<IPaymentGateway>().Keyed("Stripe"));
```

`Keyed(Func<Type, Type, object?>)` computes the key from the implementation and service type, and is evaluated when the component is added to the container. Returning `null` leaves that registration unkeyed.

## Adding to the container

`services.Add(component)` registers a single component, and a `params` overload registers several at once:

```csharp
services.Add(Component.From<CustomerService>().As<ICustomerService>());

services.Add(
    Component.From<CustomerService>().As<ICustomerService>(),
    Component.From(typeof(OrderService)).As<IOrderService>(),
    Component.From<StripePaymentGateway>().As<IPaymentGateway>().Keyed("Stripe")
);
```

Unlike the [registration chain](registration.md), there are no `AddSingleton` / `AddScoped` / `AddTransient` overloads for a component — a component already carries its own lifetime, so set it with `AsLifetime` and add it with `Add`.

## Validation

`As` verifies that the implementation is assignable to each service **when it is called**, and throws an `ArgumentException` naming both types:

```csharp
Component.From<CustomerService>().As<IPaymentGateway>()
// Throws ArgumentException:
//   "The implementation CustomerService is not based on the service type IPaymentGateway"
```

This is eager, unlike the chain — a `Classes.From(...)` chain defers all work until it is enumerated, whereas a component holds one known type and can check immediately.

## Open generic limitation

Because a component forwards its services through a factory, and Microsoft's container does not support factory-based resolution of open generic types (see [dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)), an open generic component cannot have services added to it:

```csharp
services.Add(Component.From(typeof(InMemoryRepository<>)).As(typeof(IRepository<>)))
// Throws InvalidOperationException:
//   "Open generic services can not be forwarded."
```

An open generic component with no services added registers fine, since there is nothing to forward:

```csharp
services.Add(Component.From(typeof(InMemoryRepository<>)));
// InMemoryRepository<> → InMemoryRepository<> (Singleton)
```

To map an open generic implementation to its interfaces, use the chain instead — `Classes.From(typeof(InMemoryRepository<>)).AsInterface()` registers each service type independently and does not forward. See [shared components](shared-components.md#open-generic-limitation).

## Choosing between `Component` and `Classes`

| Scenario                                                        | Entry point                                            |
|-----------------------------------------------------------------|--------------------------------------------------------|
| One known type, services named explicitly                       | `Component.From(type).As<...>()`                       |
| One known type, services chosen by convention                   | `Component.From(type).AsAllInterfaces()`               |
| Many types matched by a convention                              | `Classes.FromThisAssembly()...`                        |
| Several services must share one instance                        | `Component.From(type).As<...>()` — shared by default   |
| Several services must each have their own instance              | `Classes.From(type).AsAllInterfaces()` — not shared    |
| Open generic implementation mapped to an open generic interface | `Classes.From(typeof(Repository<>)).AsInterface()`     |
