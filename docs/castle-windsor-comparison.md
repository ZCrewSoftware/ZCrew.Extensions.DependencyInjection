# Coming from Castle Windsor

A method-by-method map for anyone moving over from [Castle Windsor's registration API](https://github.com/castleproject/Windsor/blob/master/docs/registering-components-by-conventions.md).

## API mapping

### Entry points

| Windsor                               | ZCrew                                 |
|---------------------------------------|---------------------------------------|
| `Classes.FromThisAssembly()`          | `Classes.FromThisAssembly()`          |
| `Classes.FromAssembly(Assembly)`      | `Classes.FromAssembly(Assembly)`      |
| `Classes.FromAssemblyContaining<T>()` | `Classes.FromAssemblyContaining<T>()` |
| `Classes.From(types)`                 | `Classes.From(types)`                 |
| `Types.*`                             | `Types.*`                             |

The `Classes` (concrete non-abstract) and `Types` (everything) split works the same way here.

### Filters

| Windsor                                     | ZCrew                                                      |
|---------------------------------------------|------------------------------------------------------------|
| `.Where(t => t.Name.EndsWith("Service"))`   | `.NameEndsWith("Service")`                                 |
| `.If` / `.Unless` chains                    | Repeated `.Where(...)`                                     |
| `.InNamespace(ns, true)`                    | `.InNamespace(ns, includeSubnamespaces: true)`             |
| A predicate on `IsGenericTypeDefinition`    | `.GenericTypeDefinitions()` / `.ConstructedGenericTypes()` |

### Service selection

| Windsor                                      | ZCrew                                                                              |
|----------------------------------------------|------------------------------------------------------------------------------------|
| `.WithService.AllInterfaces()`               | `.AsAllInterfaces()`, or `.AsAllNonSystemInterfaces()` to skip `IDisposable` etc.  |
| `.WithService.DefaultInterfaces()`           | `.AsDefaultInterfaces()`                                                           |
| `.WithService.FromInterface()`               | `.AsInterface()`                                                                   |
| `.WithService.FromInterface(typeof(IFoo))`   | `.AsInterface<IFoo>()`                                                             |
| `.WithService.Self()`                        | `.AsSelf()`                                                                        |
| `.WithService.Base()`                        | `.AsBase()`                                                                        |
| `.WithService.Select((t, baseTypes) => ...)` | `.As((t, baseTypes) => ...)`                                                       |

Selectors chain the same way Windsor's `WithService` calls do. `.AsSelf().AsAllInterfaces()` registers the union of every selected service type, in the order they were first seen.

### Lifetime

| Windsor                 | ZCrew                                                              |
|-------------------------|--------------------------------------------------------------------|
| `.LifestyleSingleton()` | `.AsSingleton()`                                                   |
| `.LifestyleScoped()`    | `.AsScoped()`                                                      |
| `.LifestyleTransient()` | `.AsTransient()`                                                   |
| `.LifestyleCustom<T>()` | No equivalent. MS DI only has singleton, scoped and transient.     |

### Keyed services

```csharp
// Windsor
Component.For<IPaymentGateway>().ImplementedBy<StripePaymentGateway>().Named("stripe");
// Resolved with: container.Resolve<IPaymentGateway>("stripe")

// ZCrew
services.AddSingleton(
    Classes.From(typeof(StripePaymentGateway))
        .AsInterface<IPaymentGateway>()
        .Keyed("stripe")
);
// Resolved with: [FromKeyedServices("stripe")] IPaymentGateway gateway
```

## Decorators

Windsor does this with [DynamicProxy interceptors](https://github.com/castleproject/Core/blob/master/docs/dynamicproxy.md), which is AOP through generated proxies. ZCrew's [`AddDecorator`](decorators.md) just wraps one instance in another, so it isn't a drop-in replacement.

## Where behavior differs

- **Instance sharing is automatic.** Map one class to several service types that include the class itself and they share one instance under `AsSingleton()` / `AsScoped()`, which is what Windsor does by default. Plain MS DI gives you separate instances. Select only interfaces (`AsAllInterfaces()`) and they stay independent. See [shared-services.md](shared-services.md).
- **Open generics can't be forwarded.** MS DI can't resolve open generics through a factory ([dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)), so the shared path fails at registration when an open generic would have to be forwarded. Selecting only interfaces registers each one independently instead. See [Open generic limitation](shared-services.md#open-generic-limitation).
- **`Dispose()` needs to be idempotent.** See below.

### Idempotent `Dispose()`

Windsor disposes each instance exactly once. MS DI disposes per `ServiceDescriptor`, and a shared service produces several descriptors pointing at the same object, so `Dispose()` can be reached more than once while a scope is torn down.

If a class is shared across several service types under `AsSingleton()` / `AsScoped()` (that is, whenever it's selected alongside its other service types), make `Dispose()` safe to call twice:

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
