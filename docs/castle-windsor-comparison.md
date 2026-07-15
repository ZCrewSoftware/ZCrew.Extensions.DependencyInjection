# Castle Windsor Comparison

Feature-by-feature mapping for users coming from [Castle Windsor's registration API](https://github.com/castleproject/Windsor/blob/master/docs/registering-components-by-conventions.md).

## API mapping

### Entry points

| Windsor                               | ZCrew                                 |
|---------------------------------------|---------------------------------------|
| `Classes.FromThisAssembly()`          | `Classes.FromThisAssembly()`          |
| `Classes.FromAssembly(Assembly)`      | `Classes.FromAssembly(Assembly)`      |
| `Classes.FromAssemblyContaining<T>()` | `Classes.FromAssemblyContaining<T>()` |
| `Classes.From(types)`                 | `Classes.From(types)`                 |
| `Types.*`                             | `Types.*`                             |

`Classes` (concrete non-abstract) vs `Types` (everything) split matches.

### Filters

| Windsor                                     | ZCrew                                                      |
|---------------------------------------------|------------------------------------------------------------|
| `.Where(t => t.Name.EndsWith("Service"))`   | `.NameEndsWith("Service")`                                 |
| `.If`/`.Unless` chains                      | Repeated `.Where(...)`                                     |
| `.InNamespace(ns, true)`                    | `.InNamespace(ns, includeSubnamespaces: true)`             |
| Predicate against `IsGenericTypeDefinition` | `.GenericTypeDefinitions()` / `.ConstructedGenericTypes()` |

### Service selection

| Windsor                                      | ZCrew                                                                              |
|----------------------------------------------|------------------------------------------------------------------------------------|
| `.WithService.AllInterfaces()`               | `.AsAllInterfaces()` (or `.AsAllNonSystemInterfaces()` to skip `IDisposable` etc.) |
| `.WithService.DefaultInterfaces()`           | `.AsDefaultInterfaces()`                                                           |
| `.WithService.FromInterface()`               | `.AsInterface()`                                                                   |
| `.WithService.FromInterface(typeof(IFoo))`   | `.AsInterface<IFoo>()`                                                             |
| `.WithService.Self()`                        | `.AsSelf()`                                                                        |
| `.WithService.Base()`                        | `.AsBase()`                                                                        |
| `.WithService.Select((t, baseTypes) => ...)` | `.As((t, baseTypes) => ...)`                                                       |

### Lifetime

| Windsor                 | ZCrew                                                            |
|-------------------------|------------------------------------------------------------------|
| `.LifestyleSingleton()` | `.AsSingleton()`                                                 |
| `.LifestyleScoped()`    | `.AsScoped()`                                                    |
| `.LifestyleTransient()` | `.AsTransient()`                                                 |
| `.LifestyleCustom<T>()` | Not supported — MS DI only models singleton / scoped / transient |

### Keyed services

```csharp
// Windsor
Component.For<IPaymentGateway>().ImplementedBy<StripePaymentGateway>().Named("stripe");
// Resolved via: container.Resolve<IPaymentGateway>("stripe")

// ZCrew
services.AddSingleton(
    Classes.From(typeof(StripePaymentGateway))
        .AsInterface<IPaymentGateway>()
        .Keyed("stripe")
);
// Resolved via: [FromKeyedServices("stripe")] IPaymentGateway gateway
```

## Decorators

Windsor uses [DynamicProxy interceptors](https://github.com/castleproject/Core/blob/master/docs/dynamicproxy.md) — AOP via generated proxies. ZCrew's [`AddDecorator`](decorators.md) is a plain instance-wrapping pattern, not a 1:1 substitute.

## Behavioral differences

- **Automatic instance sharing** — mapping one impl to multiple service types *that include the impl itself* shares one instance across them under `AsSingleton()` / `AsScoped()` (Windsor default). Plain MS DI gives separate instances. Selecting only interfaces (e.g. `AsAllInterfaces()`) keeps them independent. See [shared-components.md](shared-components.md).
- **Open generic forwarding** — MS DI ([dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)) can't resolve open generics through factories, so the shared-component path fails fast at registration when an open generic implementation would be forwarded. Selecting only interfaces registers each independently instead. See [Open generic limitation](shared-components.md#open-generic-limitation).
- **Idempotent `IDisposable` is required** — see below.

### Idempotent `IDisposable`

Windsor disposes each instance exactly once. MS DI's disposal is per-`ServiceDescriptor`, so a shared component (which produces multiple descriptors pointing at the same instance) can route `Dispose()` through more than one path during scope teardown.

`Dispose()` must therefore be idempotent for any impl shared across multiple service types under `AsSingleton()` / `AsScoped()` (that is, whenever the impl is selected alongside its other service types):

```csharp
public class CustomerService : ICustomerService, IAuditable, IDisposable
{
    private bool disposed;

    public void Dispose()
    {
        if (this.disposed) return;
        this.disposed = true;
        // cleanup
    }
}
```
