# Shared services

Say you register `PayPalPaymentGateway` as both `IPaymentGateway` and `IDisposable`. Most of the time you want both to resolve to the same object. That's what Castle Windsor calls a [shared service](https://github.com/castleproject/Windsor/blob/master/docs/registering-components-one-by-one.md#components-with-multiple-services-forwarded-types).

Microsoft's container doesn't do that on its own. Two singleton registrations of the same class give you two instances:

```csharp
services.AddSingleton<IPaymentGateway, PayPalPaymentGateway>();
services.AddSingleton<IDisposable, PayPalPaymentGateway>();
// Two separate PayPalPaymentGateway objects
```

## When sharing kicks in

There's no switch to flip. Whether an implementation is shared comes down to two things:

1. The lifetime. Transients are never shared, because a transient makes a new instance every time you resolve it.
2. Whether the implementation type is itself one of the selected service types.

So you get a shared instance when all of these hold:

- the lifetime is `Singleton` or `Scoped`,
- the implementation maps to more than one service type, and
- the implementation type is one of them.

When that happens, the implementation is registered once as itself and every other service type becomes a factory that resolves back through it, so they all hand you the same object. Otherwise (one service type, a transient lifetime, or a selection that leaves the implementation out) each service type is registered against the implementation independently, exactly like separate `services.AddSingleton(...)` calls.

## Getting the implementation into the selection

The interface selectors (`AsInterface`, `AsAllInterfaces`, `AsDefaultInterfaces`, and so on) map to interfaces only. The concrete type isn't among them, so those registrations stay independent.

To share one instance, put the implementation in the selection. Chain `AsSelf()` before an interface selector, since [selectors accumulate](service-selectors.md#combining-selectors):

```csharp
services.AddSingleton(
    Classes.From(typeof(PayPalPaymentGateway))
        .AsSelf()          // the implementation itself
        .AsAllInterfaces() // plus its interfaces
);
```

One custom selector does the same job if you prefer: `.As(type => type.GetInterfaces().Prepend(type))`.

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

Resolve `IPaymentGateway` and `IDisposable` from the same provider and you get the same object back. `AsServicesFromAttribute` shares the same way when the attribute lists the implementation type among the services.

Compare that with `AsAllInterfaces().AsSingleton()`, which maps to `IPaymentGateway` and `IDisposable` without the implementation. Each interface is registered on its own and gets its own instance. Add `AsSelf()` back in and you're sharing again.

## Lifetime methods

`ServiceLifetimeSelector` sets the lifetime for the whole chain. Each method returns a `ServiceSource`, so finish with `.ToServiceCollection()` or a bulk add (`services.AddSingleton(...)`, `services.Add(...)`):

| Method          | What you get                                                                            |
|-----------------|-----------------------------------------------------------------------------------------|
| `AsSingleton()` | One instance per container. Shared across service types when the implementation is one of them. |
| `AsScoped()`    | One instance per scope. Shared within the scope when the implementation is one of them.  |
| `AsTransient()` | A new instance every resolve. Never shared.                                             |

For a lifetime per implementation type, use `AsLifetime(Func<Type, ServiceLifetime>)` or read it from an attribute with `AsLifetimeByAttribute<TAttribute>` (see [lifetime from attributes](#lifetime-from-attributes)). The sharing rules then apply per service, based on whatever lifetime it resolved to.

## One service type, no forwarding

When a selector maps an implementation to a single service type there's nothing to share, so it's registered directly:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .AsFirstInterface() // only one service type
    .AsSingleton()
// CustomerService → ICustomerService (direct registration)
```

`AsSingleton()` and `AsScoped()` are safe defaults for that reason. Factory forwarding only happens when there are several service types and the implementation is one of them.

## Lifetime from attributes

Instead of one lifetime for the whole chain, `AsLifetimeByAttribute` reads it from an attribute on the implementation. That puts the lifetime next to the class it belongs to, so one convention scan can register singletons, scoped services and transients side by side. The rules are the same across all the overloads:

- Inherited attributes count by default. Each overload has a twin that takes a leading `bool inherited`. Pass `false` to only look at attributes declared on the type itself.
- No match means singleton, the same default you get from skipping the lifetime stage.
- Exactly one match. Two matching attributes on a type throws an `AmbiguousMatchException` when the chain is enumerated.
- Transients are never shared. Anything that resolves to `Transient` registers each service type independently.

## `AsLifetimeByAttribute<TAttribute>(Func<TAttribute, ServiceLifetime>)`

Reads a specific attribute. `TAttribute` can be a concrete attribute type, or an interface that one or more attributes implement:

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

That registers `CustomerStore → IStore (Scoped)`. Types without the attribute fall back to `Singleton`. There is also an `inherited` overload, `AsLifetimeByAttribute<TAttribute>(bool inherited, Func<TAttribute, ServiceLifetime>)`.

## `AsLifetimeByAttribute(Type, Func<Attribute, ServiceLifetime>)`

The non-generic form, for when you only know the attribute type at runtime. The selector gets an `Attribute`, so cast it before reading the lifetime:

```csharp
Classes.FromThisAssembly()
    .BasedOn<IStore>()
    .AsInterface()
    .AsLifetimeByAttribute(typeof(LifestyleAttribute), attribute => ((LifestyleAttribute)attribute).Lifetime)
```

Same result as the generic overload above. There is an `inherited` overload too, `AsLifetimeByAttribute(Type, bool inherited, Func<Attribute, ServiceLifetime>)`.

## Open generic limitation

Microsoft's container can't resolve open generics through a factory ([dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)):

```csharp
// Throws when you resolve it:
services.AddSingleton(typeof(IRepository<>), sp => sp.GetRequiredService(typeof(Repository<>)));
```

Sharing forwards the other service types through a factory, so it can't work for an open generic implementation. This is caught at registration time, but only when the shared path is actually taken (the open generic implementation is one of several service types under `Singleton` or `Scoped`):

```csharp
Classes.FromAssemblyContaining(typeof(Repository<>))
    .BasedOn(typeof(IRepository<>))
    .As(type => type.GetInterfaces().Prepend(type)) // implementation plus its open generic interfaces
    .AsSingleton()
// Throws InvalidOperationException:
//   "Open generic services can not be forwarded."
```

Map an open generic to its interfaces without including the implementation (the usual `AsInterface()` / `AsAllInterfaces()` case) and nothing throws. Each service type is registered independently, the same as writing `services.AddSingleton(typeof(IFoo<>), typeof(Foo<>))` by hand.

If you really need one shared instance for an open generic, you'll have to register it separately, or take another look at the design (needing a shared open generic is often a smell). `ConstructedGenericTypes()` and `GenericTypeDefinitions()` on `TypeFilter` let you split closed and open generics into separate chains.

## Choosing the right lifetime

| What you want                                                            | How                                                                                |
|--------------------------------------------------------------------------|------------------------------------------------------------------------------------|
| Several service types sharing one instance                               | `AsSingleton()` / `AsScoped()`, with the implementation in the selection            |
| Every service type with its own instance                                 | Interfaces only (`AsAllInterfaces()`) with `AsSingleton()` / `AsScoped()`           |
| A new instance every resolve                                             | `AsTransient()`                                                                     |
| Lifetime declared per type by an attribute                               | `AsLifetimeByAttribute<TAttribute>(...)`                                            |
| Lifetime computed per type                                               | `AsLifetime(Func<Type, ServiceLifetime>)`                                           |
