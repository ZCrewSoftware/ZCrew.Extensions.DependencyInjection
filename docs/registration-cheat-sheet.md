# Registration Cheat Sheet

Quick reference for `ZCrew.Extensions.DependencyInjection.Registration`. For narrative and design rationale, see [registration.md](registration.md).

```
Classes/Types  →  IncludeXxxTypes  →  Where/BasedOn/InNamespace/...  →  AsXxx  →  Keyed  →  AsSingleton/AsScoped/AsTransient
   entry             visibility                   filter               service     key             lifetime + sharing
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

## Assembly visibility

Only available after `FromAssembly*` (returns `IAssemblyTypeSelector`). Default is public types.

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

`InNamespace*` returns `IServiceSelector` — namespace filtering commits to the service-selection stage.

## Service selectors

Map each impl type to one or more service types.

| Method                                                                                      | Service types                                                                                 |
|---------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------|
| `AsSelf()`                                                                                  | The implementation type                                                                       |
| `AsBase()`                                                                                  | The base types set via `BasedOn` (use with open generics: `BasedOn(typeof(IFoo<>)).AsBase()`) |
| `AsAllInterfaces()`                                                                         | Every interface implemented                                                                   |
| `AsAllNonSystemInterfaces()`                                                                | Every interface except `System.*`                                                             |
| `AsDefaultInterfaces()`                                                                     | Interfaces whose name appears in the class name (e.g. `CustomerService` → `ICustomerService`) |
| `AsDefaultNonSystemInterfaces()`                                                            | Default interfaces, excluding `System.*`                                                      |
| `AsFirstInterface()`                                                                        | The first interface in metadata order                                                         |
| `AsInterface()`                                                                             | Top-level interfaces derived from `BasedOn` types                                             |
| `AsInterface<T>()` / `AsInterface(Type)`                                                    | Top-level interfaces derived from `T`                                                         |
| `AsInterfaces(params Type[])`                                                               | Top-level interfaces derived from the given types                                             |
| `AsAllTypes()` / `AsDefaultTypes()` / `AsAllNonSystemTypes()` / `AsDefaultNonSystemTypes()` | Like the `Interfaces` variants but for base types                                             |
| `As(Func<Type, IEnumerable<Type>>)`                                                         | Custom mapping                                                                                |
| `As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>>)`                                    | Custom mapping with access to base types from `BasedOn`                                       |

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

See [keyed-service-selectors.md](keyed-service-selectors.md) for examples.

## Lifetime + sharing

Terminal calls. They produce an `IServiceCollection`.

| Method                                 | Lifetime  | `SharingMode`                                                        |
|----------------------------------------|-----------|----------------------------------------------------------------------|
| `AsSingleton()`                        | Singleton | `SharedComponent`                                                    |
| `AsSingletonDependent()`               | Singleton | `Dependent`                                                          |
| `AsSingletonIndependent()`             | Singleton | `Independent`                                                        |
| `AsScoped()`                           | Scoped    | `SharedComponent`                                                    |
| `AsScopedDependent()`                  | Scoped    | `Dependent`                                                          |
| `AsScopedIndependent()`                | Scoped    | `Independent`                                                        |
| `AsTransient()`                        | Transient | `Independent` (only valid mode)                                      |
| `AsLifetime(lifetime [, sharingMode])` | Custom    | Defaults to `Independent` for Transient, `SharedComponent` otherwise |

`SharingMode` controls what happens when one impl is registered against multiple service types:

- **`SharedComponent`** — one instance, every service type resolves to it.
- **`Dependent`** — each service type is a factory that resolves the impl from elsewhere (you must register it separately, e.g. with a second `AsSelf()` selection).
- **`Independent`** — each service type gets its own instance.

See [shared-components.md](shared-components.md) for the full model.

## Adding to the container

**Preferred** — use the bulk-add extensions. No extra `using` needed beyond `ZCrew.Extensions.DependencyInjection.Registration`:

```csharp
services.AddSingleton(Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface());
services.AddScoped(   Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface());
services.AddTransient(Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface());
```

Overloads exist for every stage of the chain (`ITypeSelector`, `ITypeFilter`, `IServiceSelector`, `IKeyedServiceSelector`, `IServiceSource`), so you can stop the chain early.

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

```csharp
services.AddSingleton(
    Classes.FromAssemblyContaining<Startup>()
        .BasedOn<IHostedService>()
        .AsAllTypes()
);
```
