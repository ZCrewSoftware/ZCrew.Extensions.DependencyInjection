# Castle Windsor Comparison

`ZCrew.Extensions.DependencyInjection.Registration` was modeled on [Castle Windsor's registration API](https://github.com/castleproject/Windsor/blob/master/docs/registering-components-by-conventions.md). The entry points (`Classes.FromAssembly...`), the chain shape (filter → select → register), and the [shared-component model](9-shared-components.md) all map back to Windsor patterns. If you have used Windsor, the ZCrew fluent API should feel familiar.

This doc is a feature-by-feature mapping between the two libraries, followed by three behavioral nuances where ZCrew differs from Windsor in ways that matter at registration or resolution time. It is not a recommendation of one over the other — Windsor is a full DI container with its own lifecycle, interception, and resolution semantics, while ZCrew targets `Microsoft.Extensions.DependencyInjection` and is constrained by what that container can express.

The Windsor snippets below use Windsor's `Component`/`Classes`/`Types` registration API. The ZCrew snippets assume `using ZCrew.Extensions.DependencyInjection.Registration;` and a containing `services.Add(...)` call where one is needed to produce an `IServiceCollection` mutation.

## Service scanning and registration

### Assembly scanning

Both libraries expose the same set of entry points with near-identical names.

```csharp
// Castle Windsor
container.Register(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .WithService.FromInterface()
);
```

```csharp
// ZCrew
services.Add(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .AsInterface()
);
```

`FromThisAssembly()`, `FromAssembly(Assembly)`, `FromAssemblyContaining<T>()`, and `From(IEnumerable<Type>)` all exist on both sides with the same semantics. ZCrew also distinguishes `Classes` (concrete, non-abstract) from `Types` (everything — interfaces, structs, enums, abstract classes), matching the Windsor split.

### Type filtering

```csharp
// Castle Windsor
container.Register(
    Classes.FromThisAssembly()
        .Where(t => t.Name.EndsWith("Service"))
        .InNamespace("MyApp.Services", includeSubnamespaces: true)
        .WithService.DefaultInterfaces()
);
```

```csharp
// ZCrew
services.Add(
    Classes.FromThisAssembly()
        .NameEndsWith("Service")
        .InNamespace("MyApp.Services", includeSubnamespaces: true)
        .AsDefaultInterfaces()
);
```

ZCrew ships a first-class [`NameEndsWith`](6-type-filters.md#nameendswithstring-and-overloads) filter (the most common Windsor `.Where(...)` lambda), plus [generic-type filters](6-type-filters.md#generictypes--generictypedefinitions--constructedgenerictypes) (`GenericTypes`, `GenericTypeDefinitions`, `ConstructedGenericTypes`) that Windsor users typically express with raw predicates. Windsor's `If`/`Unless` chain maps onto repeated [`Where`](6-type-filters.md#wherefunctype-bool) calls in ZCrew.

### Service selection

```csharp
// Castle Windsor — most-derived interface from BasedOn
container.Register(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .WithService.FromInterface()
);

// Castle Windsor — every interface a type implements
container.Register(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .WithService.AllInterfaces()
);
```

```csharp
// ZCrew — most-derived interface from BasedOn
services.Add(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .AsInterface()
);

// ZCrew — every interface a type implements
services.Add(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .AsAllInterfaces()
);
```

The full mapping:

| Windsor                                      | ZCrew                                                                                                   |
|----------------------------------------------|---------------------------------------------------------------------------------------------------------|
| `.WithService.AllInterfaces()`               | `.AsAllInterfaces()` (or `.AsAllNonSystemInterfaces()` to exclude `IDisposable`, `IEquatable<T>`, etc.) |
| `.WithService.DefaultInterfaces()`           | `.AsDefaultInterfaces()`                                                                                |
| `.WithService.FromInterface()`               | `.AsInterface()`                                                                                        |
| `.WithService.FromInterface(typeof(IFoo))`   | `.AsInterface<IFoo>()`                                                                                  |
| `.WithService.Self()`                        | `.AsSelf()`                                                                                             |
| `.WithService.Base()`                        | `.AsBase()`                                                                                             |
| `.WithService.Select((t, baseTypes) => ...)` | `.As((t, baseTypes) => ...)`                                                                            |

See [`7-service-selectors.md`](7-service-selectors.md) for the full ZCrew reference.

### Lifetime

```csharp
// Castle Windsor
container.Register(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .WithService.FromInterface()
        .LifestyleSingleton()
);
```

```csharp
// ZCrew
services.Add(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .AsInterface()
        .AsSingleton()
);
```

| Windsor                           | ZCrew                                                                                                                                     |
|-----------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------|
| `.LifestyleSingleton()`           | `.AsSingleton()` (or `.AsSingletonIndependent()` to suppress sharing — see [nuance #1](#shared-instances-for-singleton-and-scoped) below) |
| `.LifestyleScoped()`              | `.AsScoped()` / `.AsScopedIndependent()`                                                                                                  |
| `.LifestyleTransient()`           | `.AsTransient()`                                                                                                                          |
| `.LifestyleCustom<MyLifestyle>()` | Not supported — `Microsoft.Extensions.DependencyInjection` only models singleton / scoped / transient.                                    |

### Keyed services

```csharp
// Castle Windsor
container.Register(
    Component.For<IPaymentGateway>()
        .ImplementedBy<StripePaymentGateway>()
        .Named("stripe")
);
// Resolved via: container.Resolve<IPaymentGateway>("stripe");
```

```csharp
// ZCrew
services.Add(
    Classes.From(typeof(StripePaymentGateway))
        .AsInterface<IPaymentGateway>()
        .Keyed("stripe")
        .AsSingleton()
);
// Resolved via constructor parameter: [FromKeyedServices("stripe")] IPaymentGateway gateway
```

See [`8-keyed-service-selectors.md`](8-keyed-service-selectors.md) for `Keyed()`, the convention-based auto-key overload, and per-implementation key selectors.

## Decorators

Castle Windsor does not ship a direct decorator API. The closest analogue is [interceptors via DynamicProxy](https://github.com/castleproject/Core/blob/master/docs/dynamicproxy.md), which intercept method calls at runtime through generated proxy types — an AOP pattern that is structurally and semantically different from wrapping an instance with another instance.

ZCrew's [`AddDecorator`](3-decorators.md) is a plain wrapper pattern with no proxy generation. The two are not a 1:1 substitution — if you currently rely on Windsor interceptors for cross-cutting concerns, the closest ZCrew equivalent is to register decorators explicitly for the interfaces you want to wrap.

---

## Known behavioral differences

Three behaviors where ZCrew deliberately differs from Windsor in ways that affect registration or runtime behavior.

### Shared instances for singleton and scoped

**Windsor's default:** registering one component against multiple service types yields a single shared instance — Windsor's [shared component](https://github.com/castleproject/Windsor/blob/master/docs/registering-components-one-by-one.md#components-with-multiple-services-forwarded-types) model.

**Plain MS DI's default:** the same impl registered against two interfaces yields **two separate instances**.

```csharp
services.AddSingleton<ICustomerService, CustomerService>();
services.AddSingleton<IAuditable, CustomerService>();
// Two distinct CustomerService instances are created
```

**ZCrew restores Windsor's default** via [`SharingMode.SharedComponent`](9-shared-components.md), which is the default for `AsSingleton()` and `AsScoped()`:

```csharp
services.Add(
    Classes.From(typeof(CustomerService))
        .AsAllNonSystemInterfaces()
        .AsSingleton()
);
// ICustomerService and IAuditable both resolve to the same CustomerService instance.
```

`AsSingletonIndependent()` / `AsScopedIndependent()` opt back into the MS DI default if you want it. See [`9-shared-components.md`](9-shared-components.md) for the full sharing-mode reference.

### Open generic registration with a factory

Windsor supports registering an open generic component against an open generic service and resolving it through a factory method:

```csharp
// Castle Windsor — works
container.Register(
    Component.For(typeof(IRepository<>))
        .UsingFactoryMethod((kernel, ctx) => /* construct the closed type */)
        .LifestyleSingleton()
);
```

**MS DI does not support this** — factory-based resolution of open generic services is not implemented in the container ([dotnet/runtime#41050](https://github.com/dotnet/runtime/issues/41050)). This is a limitation of `Microsoft.Extensions.DependencyInjection`, not of ZCrew. The closest thing MS DI offers is type-based open generic registration:

```csharp
// Works in MS DI — no factory, just type-to-type mapping
services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
```

Because ZCrew's shared-component forwarding is implemented as factory delegation under the hood, the same limitation propagates into the shared-component case. Rather than letting it silently break at resolve time, ZCrew detects it and **fails fast at registration**:

```csharp
services.Add(
    Classes.FromAssemblyContaining(typeof(Repository<>))
        .BasedOn(typeof(IRepository<>))
        .AsAllNonSystemInterfaces()
        .AsSingleton()
);
// Throws InvalidOperationException at registration:
//   "Open generic services can not be forwarded."
```

The check lives in `ServiceComponent.GetServiceDescriptors` in the Registration project. The recommended workaround is `AsSingletonIndependent()` / `AsScopedIndependent()`, which registers each service type as its own descriptor (matching what `services.AddSingleton(typeof(IFoo<>), typeof(Foo<>))` would produce). The [`GenericTypeDefinitions()`](6-type-filters.md#generictypes--generictypedefinitions--constructedgenerictypes) and `ConstructedGenericTypes()` filters help when you want to split a single scan into open- and closed-generic registration paths.

### Idempotent `IDisposable` is required

Windsor tracks ownership of each component instance and disposes it exactly once when the container (or scope) is released. The shared-component forwarding case is handled internally — multiple service-type registrations pointing at the same instance still produce a single `Dispose()` call.

**MS DI does not centralize this.** Its disposal tracking is per-`ServiceDescriptor`: when a scope (or the root provider) is released, every descriptor whose factory or activator produced an `IDisposable` instance has `Dispose()` called on that instance. ZCrew's shared-component mode produces multiple descriptors — one anchor descriptor for the implementation and one factory descriptor per additional service type — and MS DI can route through each one independently. The result is that the underlying instance may receive **`Dispose()` calls from multiple paths** during scope teardown.

This makes the standard `IDisposable` guard pattern **mandatory**, not merely defensive, for any shared singleton or scoped implementation:

```csharp
public class CustomerService : ICustomerService, IAuditable, IDisposable
{
    private bool disposed;

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        // cleanup here — close connections, release handles, etc.
    }
}
```

If `Dispose()` is not idempotent, the second invocation will typically throw `ObjectDisposedException` or — worse — double-release a resource. Idempotent `Dispose()` is good `.NET` style anyway; in this codebase it is a hard requirement for any implementation registered with `AsSingleton()` / `AsScoped()` against multiple service types.
