# Services

A service is one concrete type registered against one or more service types.

Where [`Classes` and `Types`](type-selectors.md) scan an assembly and apply a convention to whatever they find, `Service` starts from a single type you already know. There's no [type filtering](type-filters.md) or [service selection](service-selectors.md) stage to go through. This is the same idea as Castle Windsor's [`Component.For<T>()`](https://github.com/castleproject/Windsor/blob/master/docs/registering-components-one-by-one.md).

Every service type you add is forwarded to the implementation, so a `Singleton` or `Scoped` service resolves to one shared instance no matter which service type you ask for.

```csharp
services.Add(Service.From(typeof(PayPalPaymentGateway)).As<IPaymentGateway, IDisposable>());
```

## `Service.From(Type)`

Starts a service from a concrete type. It begins registered against the type itself:

```csharp
services.Add(Service.From<CustomerService>());
```

That gives you:

```
CustomerService → CustomerService (Singleton)
```

This is the one place the service API behaves differently from the fluent chain. `Classes.From(...)` treats "register as self" as a default that any `As*` selector replaces. `Service.From` treats it as the starting point that `As` adds to. The implementation is therefore always in the list, which is what makes sharing automatic.

## `As<T1>()` through `As<T1, …, T8>()`

Adds one to eight service types, each forwarded to the implementation:

```csharp
services.Add(Service.From<PayPalPaymentGateway>().As<IPaymentGateway, IDisposable>());
```

Given:

```csharp
public interface IPaymentGateway : IDisposable { }
public class PayPalPaymentGateway : IPaymentGateway { }
```

you get:

```
PayPalPaymentGateway → PayPalPaymentGateway (registered directly)
IPaymentGateway      → resolves to the PayPalPaymentGateway instance
IDisposable          → resolves to the PayPalPaymentGateway instance
```

Resolve `IPaymentGateway` and `IDisposable` from the same provider and you get the same object.

## `As(Type)` / `As(IEnumerable<Type>)`

The non-generic forms, for service types you only know at runtime:

```csharp
services.Add(Service.From<PayPalPaymentGateway>().As(typeof(IPaymentGateway)));
services.Add(Service.From<PayPalPaymentGateway>().As([typeof(IPaymentGateway), typeof(IDisposable)]));
```

`As` adds rather than replaces, so you can chain calls and mix them with the generic overloads:

```csharp
Service.From<PayPalPaymentGateway>()
    .As<IPaymentGateway>()
    .As(typeof(IDisposable))
// PayPalPaymentGateway, IPaymentGateway, IDisposable
```

Duplicates are fine. They're collapsed when the service is registered, so `As<IPaymentGateway>().As<IPaymentGateway>()` gives you one `IPaymentGateway` registration.

## Picking service types by convention

The `As*` selectors from the [registration chain](service-selectors.md) work here too, so you can pick service types by convention and still get the shared instance:

```csharp
services.Add(Service.From<PayPalPaymentGateway>().AsAllNonSystemInterfaces());
```

Given:

```csharp
public interface IPaymentGateway : IDisposable { }
public class PayPalPaymentGateway : IPaymentGateway { }
```

you get:

```
PayPalPaymentGateway → PayPalPaymentGateway (registered directly)
IPaymentGateway      → resolves to the PayPalPaymentGateway instance
                       (IDisposable is in System, so it's excluded)
```

Selection adds to the implementation instead of replacing it, so the implementation stays first in the list and the service stays shared. The same selector on a chain drops it: `Classes.From(typeof(PayPalPaymentGateway)).AsAllNonSystemInterfaces()` registers `IPaymentGateway` on its own, with no `PayPalPaymentGateway` registration to share.

| Method                                                                        | Service types added                                                                      |
|-------------------------------------------------------------------------------|------------------------------------------------------------------------------------------|
| `AsAllInterfaces()`                                                           | Every interface implemented                                                              |
| `AsAllNonSystemInterfaces()`                                                  | Every interface not from `System.*`                                                      |
| `AsDefaultInterfaces()`                                                       | Interfaces whose name appears in the class name (`CustomerService` → `ICustomerService`) |
| `AsDefaultNonSystemInterfaces()`                                              | Default interfaces, not counting `System.*`                                              |
| `AsFirstInterface()`                                                          | The first interface in metadata order                                                    |
| `AsAllTypes()` / `AsAllNonSystemTypes()`                                      | Like the interface versions, plus every non-abstract base class                          |
| `AsDefaultTypes()` / `AsDefaultNonSystemTypes()`                              | The same, restricted to names that match the convention                                  |
| `AsServicesFromAttribute<TAttribute>(…)` / `AsServicesFromAttribute(Type, …)` | Service types read from an attribute on the implementation                               |

Each method behaves exactly as it does on the chain, so see [service selectors](service-selectors.md) for the details. Selectors accumulate here too, so `AsDefaultInterfaces().AsAllInterfaces()` registers the union of both.

### A selector that matches nothing still leaves you registered

If a selector finds no service types, the service is left as it was rather than emptied:

```csharp
services.Add(Service.From<Customer>().AsDefaultInterfaces());
// Customer → Customer (Singleton). Customer has no interfaces.
```

The chain registers nothing at all in that situation.

That's also why there's no `AsSelf()` and no `*OrSelf` selectors here. The implementation is seeded from the start, so "or self" is already what happens. `AsAllInterfacesOrSelf()` would do exactly what `AsAllInterfaces()` already does.

`AsBase()` and `AsInterface()` are missing for a different reason: they read the base types that [`BasedOn`](type-filters.md) sets, and a service has no filtering stage to set them.

### Attribute selectors are checked too

Service types from an attribute go through the same check as any other `As` call, so an attribute naming a type the implementation doesn't implement throws:

```csharp
[Contract(typeof(IProvidedServiceA))]
public class ContractBase;  // ...but doesn't implement IProvidedServiceA

Service.From<ContractBase>().AsServicesFromAttribute<ContractAttribute>(attribute => attribute.Contracts)
// Throws ArgumentException:
//   "The implementation ContractBase is not based on the service type IProvidedServiceA"
```

The chain accepts this. It registers whatever the attribute names without checking.

## Lifetime

A service with no lifetime set is a `Singleton`, the same default a [skipped lifetime stage](shared-services.md#lifetime-methods) uses. Call `AsLifetime` for anything else:

```csharp
services.Add(
    Service.From<OrderService>()
        .As<IOrderService>()
        .AsLifetime(ServiceLifetime.Scoped)
);
```

`AsLifetime(Func<Type, ServiceLifetime>)` takes the lifetime from a delegate that gets the implementation type, matching the chain's [per-type lifetime](shared-services.md#lifetime-methods) helper.

Transients are never shared. A transient makes a new instance on every resolve by definition, so each service type is registered against the implementation independently:

```csharp
Service.From<PayPalPaymentGateway>()
    .As<IPaymentGateway>()
    .AsLifetime(ServiceLifetime.Transient)
// PayPalPaymentGateway → PayPalPaymentGateway (transient)
// IPaymentGateway      → PayPalPaymentGateway (transient, registered directly)
```

## Service keys

`Keyed(object?)` gives every service type on the service the same key, and `Unkeyed()` takes it away again. See [service key selectors](service-key-selectors.md) for how keys behave:

```csharp
services.Add(Service.From<StripePaymentGateway>().As<IPaymentGateway>().Keyed("Stripe"));
```

`Keyed(Func<Type, Type, object?>)` works the key out from the implementation and service type, and runs when the service is added to the container. Return `null` and that registration is left unkeyed.

## Adding to the container

`services.Add(service)` registers one service, and there's a `params` overload for several at once:

```csharp
services.Add(Service.From<CustomerService>().As<ICustomerService>());

services.Add(
    Service.From<CustomerService>().As<ICustomerService>(),
    Service.From(typeof(OrderService)).As<IOrderService>(),
    Service.From<StripePaymentGateway>().As<IPaymentGateway>().Keyed("Stripe")
);
```

There are no `AddSingleton` / `AddScoped` / `AddTransient` overloads like the [registration chain](registration.md) has. A service already carries its own lifetime, so set it with `AsLifetime` and add it with `Add`.

## Validation

`As` checks that the implementation is assignable to each service type as soon as you call it, and throws an `ArgumentException` naming both:

```csharp
Service.From<CustomerService>().As<IPaymentGateway>()
// Throws ArgumentException:
//   "The implementation CustomerService is not based on the service type IPaymentGateway"
```

This happens right away, unlike the chain. A `Classes.From(...)` chain defers everything until it's enumerated, while a service holds one known type and can check on the spot.

## Open generics

A service forwards its service types through a factory, and Microsoft's container can't resolve open generics that way (see [dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)). So you can't add service types to an open generic service:

```csharp
services.Add(Service.From(typeof(InMemoryRepository<>)).As(typeof(IRepository<>)))
// Throws InvalidOperationException:
//   "Open generic services can not be forwarded."
```

An open generic service with no service types added registers fine, since there's nothing to forward:

```csharp
services.Add(Service.From(typeof(InMemoryRepository<>)));
// InMemoryRepository<> → InMemoryRepository<> (Singleton)
```

To map an open generic implementation to its interfaces, use the chain. `Classes.From(typeof(InMemoryRepository<>)).AsInterface()` registers each service type independently and doesn't forward. See [shared services](shared-services.md#open-generic-limitation).

## `Service` or `Classes`?

| What you have                                                    | What to use                                          |
|------------------------------------------------------------------|------------------------------------------------------|
| One known type, service types named explicitly                   | `Service.From(type).As<...>()`                       |
| One known type, service types by convention                      | `Service.From(type).AsAllInterfaces()`               |
| Lots of types matching a convention                              | `Classes.FromThisAssembly()...`                      |
| Several service types that must share one instance               | `Service.From(type).As<...>()`, shared by default    |
| Several service types that each need their own instance          | `Classes.From(type).AsAllInterfaces()`, not shared   |
| An open generic mapped to an open generic interface              | `Classes.From(typeof(Repository<>)).AsInterface()`   |
