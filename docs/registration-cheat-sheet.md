# Registration Cheat Sheet

Quick reference for `ZCrew.Extensions.DependencyInjection.Registration`. For narrative and design rationale, see [registration.md](registration.md).

```
Classes/Types → IncludeXxxTypes → Where/BasedOn/InNamespace/... → AsXxx  → Keyed → AsSingleton/AsScoped/AsTransient → ToServiceCollection/AddXxx
    entry           visibility                filter              service    key               lifetime                         terminal
```

Each stage is optional after the entry point. Skip any stage and the next call applies sensible defaults.

## Entry points

| Method                                | Source                                                                            |
|---------------------------------------|-----------------------------------------------------------------------------------|
| `Classes.From(params Type[])`         | Explicit list — concrete non-abstract classes only                                |
| `Classes.FromAssembly(Assembly)`      | Scan an assembly                                                                  |
| `Classes.FromAssemblyContaining<T>()` | Scan the assembly containing `T`                                                  |
| `Classes.FromThisAssembly()`          | Scan the calling assembly                                                         |
| `Types.*`                             | Same factories, but include interfaces, abstracts, structs, enums, static classes |

`Classes` is the typical choice. Use `Types` when you need to discover interface types or value types.

## Single components

`Component.From` skips the whole chain and registers **one known type** against services you name. The component starts registered as the type itself and every added service is forwarded to it, so a `Singleton` or `Scoped` component shares one instance.

| Method                         | Effect                                                     |
|--------------------------------|------------------------------------------------------------|
| `Component.From(Type)`         | Start a component, registered against the type itself      |
| `As<T1>()` … `As<T1, …, T8>()` | Add one to eight services, forwarded to the implementation |
| `As(Type)`                     | Add one service, forwarded to the implementation           |
| `As(IEnumerable<Type>)`        | Add several services, forwarded to the implementation      |
| `AsLifetime(ServiceLifetime)`  | Set the lifetime — defaults to `Singleton`                 |
| `AsLifetime(Func<Type, …>)`    | Set the lifetime from the implementation type              |
| `Keyed(object?)` / `Unkeyed()` | Assign or remove a service key                             |
| `services.Add(component, …)`   | Add one or more components (`params`)                      |

```csharp
services.Add(Component.From(typeof(PayPalPaymentGateway)).As<IPaymentGateway, IDisposable>());
// Both interfaces resolve to one shared PayPalPaymentGateway
```

`As` throws `ArgumentException` if the implementation isn't based on a service. Open generic components can't have services added — they can't be forwarded. For more on components see [components.md](components.md).

## Assembly visibility

Only available after `FromAssembly*` (returns `AssemblyTypeSelector`). Default is public types.

| Method                   | Selects                                                                 |
|--------------------------|-------------------------------------------------------------------------|
| `IncludePublicTypes()`   | Public types only (default)                                             |
| `IncludeInternalTypes()` | Public + top-level internal types                                       |
| `IncludeAllTypes()`      | All types including nested and compiler-emitted — combine with a filter |

## Type filters

| Method                                                              | Effect                                                                    |
|---------------------------------------------------------------------|---------------------------------------------------------------------------|
| `AllTypes()`                                                        | No filter — pass everything through                                       |
| `Where(Func<Type, bool>)`                                           | Custom predicate                                                          |
| `BasedOn<T>()` / `BasedOn(params Type[])`                           | Match types assignable to any of the given bases (open generics OK)       |
| `NameEndsWith(string [, bool ignoreCase [, CultureInfo?]])`         | Match by name suffix; overloads for case + culture and `StringComparison` |
| `GenericTypes()`                                                    | Any generic type (open or closed)                                         |
| `GenericTypeDefinitions()`                                          | Open generics only                                                        |
| `ConstructedGenericTypes()`                                         | Closed generics only                                                      |
| `InNamespace(string [, bool includeSubnamespaces])`                 | Restrict to a namespace                                                   |
| `InSameNamespaceAs<T>([bool])` / `InSameNamespaceAs(Type [, bool])` | Restrict to the namespace of another type                                 |

`InNamespace*` returns `ServiceSelector` — namespace filtering commits to the service-selection stage.

## Service selectors

Map each impl type to one or more service types. Selectors return `ServiceSelector`, so they can be **chained** — e.g. `AsSelf().AsAllInterfaces()` — to register the distinct union of their service types (see [combining selectors](service-selectors.md#combining-selectors)).

| Method                                                                                      | Service types                                                                                             |
|---------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------|
| `AsSelf()`                                                                                  | The implementation type                                                                                   |
| `AsBase()`                                                                                  | The base types set via `BasedOn` (use with open generics: `BasedOn(typeof(IFoo<>)).AsBase()`)             |
| `AsAllInterfaces()`                                                                         | Every interface implemented                                                                               |
| `AsAllNonSystemInterfaces()`                                                                | Every interface except `System.*`                                                                         |
| `AsDefaultInterfaces()`                                                                     | Interfaces whose name appears in the class name (e.g. `CustomerService` → `ICustomerService`)             |
| `AsDefaultNonSystemInterfaces()`                                                            | Default interfaces, excluding `System.*`                                                                  |
| `AsFirstInterface()`                                                                        | The first interface in metadata order                                                                     |
| `AsInterface()`                                                                             | Top-level interfaces derived from `BasedOn` types                                                         |
| `AsInterface<T>()` / `AsInterface(Type)`                                                    | Top-level interfaces derived from `T`                                                                     |
| `AsInterfaces(params Type[])`                                                               | Top-level interfaces derived from the given types                                                         |
| `AsAllTypes()` / `AsDefaultTypes()` / `AsAllNonSystemTypes()` / `AsDefaultNonSystemTypes()` | Like the `Interfaces` variants but for base types                                                         |
| `As(Func<Type, IEnumerable<Type>>)`                                                         | Custom mapping                                                                                            |
| `As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>>)`                                    | Custom mapping with access to base types from `BasedOn`                                                   |
| `AsServicesFromAttribute()` / `AsServicesFromAttributeOrSelf()`                             | Service types from a `[Services(...)]` / `IServiceTypesProvider` attribute (`…OrSelf` falls back to self) |

For more on selector behavior see [service-selectors.md](service-selectors.md).

## Keyed registration

Optional, applied after service selection.

| Method                             | Behavior                                                                                                   |
|------------------------------------|------------------------------------------------------------------------------------------------------------|
| `Unkeyed()`                        | Explicit no-key (default)                                                                                  |
| `Keyed()`                          | Auto-key by stripping the service-name token from the impl name (e.g. `StripePaymentGateway` → `"Stripe"`) |
| `Keyed(object?)`                   | Same key for every registration (`null` = unkeyed)                                                         |
| `Keyed(Func<Type, object?>)`       | Key from implementation type                                                                               |
| `Keyed(Func<Type, Type, object?>)` | Key from `(implementation, serviceType)`                                                                   |

See [service-key-selectors.md](service-key-selectors.md) for examples.

## Lifetime

The lifetime-selection stage (on `ServiceLifetimeSelector`). Each call returns a `ServiceSource` — finish it with `.ToServiceCollection()` or a bulk-add (`services.AddSingleton(...)`, `services.Add(...)`).

| Method                                    | Lifetime  | Notes                                                                                         |
|-------------------------------------------|-----------|-----------------------------------------------------------------------------------------------|
| `AsSingleton()`                           | Singleton | One instance per container                                                                    |
| `AsScoped()`                              | Scoped    | One instance per scope                                                                        |
| `AsTransient()`                           | Transient | New instance per resolution                                                                   |
| `AsLifetime(ServiceLifetime)`             | Custom    | Explicit lifetime for the whole chain                                                         |
| `AsLifetime(Func<Type, ServiceLifetime>)` | Per type  | Lifetime computed from the implementation type                                                |
| `AsLifetimeByAttribute(...)`              | Per type  | Lifetime from a `[Lifetime]` / `IServiceLifetimeProvider` attribute (falls back to Singleton) |

Skipping the stage defaults to `Singleton`.

**Sharing is automatic.** When one implementation is mapped to multiple service types under a `Singleton` or `Scoped` lifetime **and the implementation type is itself one of those service types**, it is registered once and the other service types forward to it, so they resolve to a single shared instance. Otherwise — a single service type, a transient lifetime, or a selection that excludes the implementation — each service type is registered independently. See [shared-components.md](shared-components.md) for the full model.

## Adding to the container

**Preferred** — use the bulk-add extensions. No extra `using` needed beyond `ZCrew.Extensions.DependencyInjection.Registration`:

```csharp
services.AddSingleton(Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface());
services.AddScoped(   Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface());
services.AddTransient(Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface());
```

Overloads exist for every stage of the chain (`AssemblyTypeSelector`, `TypeFilter`, `ServiceSelector`, `ServiceKeySelector`, `ServiceLifetimeSelector`, `ServiceSource`), so you can stop the chain early.

**Alternative (Windsor-style)** — set the lifetime on the chain and pass the result to `services.Add`.

```csharp
services.Add(
    Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface().AsScoped()
);
```

## Recipes

**Repositories by their top-level interface**

```csharp
services.AddSingleton(
    Classes.FromAssemblyContaining<SqlCustomerRepository>()
        .BasedOn<IRepository>()
        .AsInterface()
);
```

**Services by naming convention**

```csharp
services.AddScoped(
    Classes.FromAssemblyContaining<CustomerService>()
        .InSameNamespaceAs<CustomerService>(includeSubnamespaces: true)
        .AsDefaultInterfaces()
);
```

**Open generic validators**

```csharp
services.AddTransient(
    Classes.FromAssemblyContaining<OrderValidator>()
        .BasedOn(typeof(IValidator<>))
        .AsBase()
);
```

**Everything ending in `Service` as itself**

```csharp
services.AddScoped(
    Classes.FromThisAssembly()
        .NameEndsWith("Service")
        .AsSelf()
);
```

**Keyed strategies (`StripePaymentGateway` → `"Stripe"`)**

```csharp
services.AddSingleton(
    Classes.FromAssemblyContaining<Startup>()
        .BasedOn<IPaymentGateway>()
        .AsInterface()
        .Keyed()
);
```

**Hosted Services with Shared Instance**

Include the implementation itself in the selection so the host and any injected interface resolve to one instance:

```csharp
services.AddSingleton(
    Classes.FromAssemblyContaining<Startup>()
        .BasedOn<IHostedService>()
        .As(type => type.AsAllTypes())
);
```
