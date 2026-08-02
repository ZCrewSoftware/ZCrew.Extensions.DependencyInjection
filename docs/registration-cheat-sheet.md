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

## Single services

`Service.From` skips the whole chain and registers **one known type** against services you name. The service starts registered as the type itself and every added service is forwarded to it, so a `Singleton` or `Scoped` service shares one instance.

| Method                         | Effect                                                     |
|--------------------------------|------------------------------------------------------------|
| `Service.From(Type)`           | Start a service, registered against the type itself        |
| `As<T1>()` … `As<T1, …, T8>()` | Add one to eight services, forwarded to the implementation |
| `As(Type)`                     | Add one service, forwarded to the implementation           |
| `As(IEnumerable<Type>)`        | Add several services, forwarded to the implementation      |
| `AsLifetime(ServiceLifetime)`  | Set the lifetime — defaults to `Singleton`                 |
| `AsLifetime(Func<Type, …>)`    | Set the lifetime from the implementation type              |
| `Keyed(object?)` / `Unkeyed()` | Assign or remove a service key                             |
| `services.Add(service, …)`     | Add one or more services (`params`)                        |

```csharp
services.Add(Service.From(typeof(PayPalPaymentGateway)).As<IPaymentGateway, IDisposable>());
// Both interfaces resolve to one shared PayPalPaymentGateway
```

Services can also be picked by convention, using the same selectors as the chain — the implementation is kept and the service stays shared.

| Method                                                     | Service types added                                             |
|------------------------------------------------------------|-----------------------------------------------------------------|
| `AsAllInterfaces()` / `AsAllNonSystemInterfaces()`         | Every interface, optionally excluding `System.*`                |
| `AsDefaultInterfaces()` / `AsDefaultNonSystemInterfaces()` | Interfaces whose name appears in the class name                 |
| `AsFirstInterface()`                                       | The first interface in metadata order                           |
| `AsAllTypes()` / `AsDefaultTypes()` / `…NonSystemTypes()`  | As above, plus non-abstract base classes                        |
| `AsServicesFromAttribute<TAttribute>(…)`                   | Service types projected from an attribute on the implementation |

```csharp
services.Add(Service.From<PayPalPaymentGateway>().AsAllNonSystemInterfaces());
// PayPalPaymentGateway + IPaymentGateway, sharing one instance
```

There is no `AsSelf()`, `AsBase()`, or `*OrSelf` on a service — the implementation is always seeded, and there is no `BasedOn` stage to supply base types.

`As` throws `ArgumentException` if the implementation isn't based on a service — including services named by an attribute. When a selector matches nothing, the implementation stays registered on its own rather than dropping out. Open generic services can't have services added — they can't be forwarded. For more on services see [services.md](services.md).

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

| Method                                                                                      | Service types                                                                                               |
|---------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------|
| `AsSelf()`                                                                                  | The implementation type                                                                                     |
| `AsBase()`                                                                                  | The base types set via `BasedOn` (use with open generics: `BasedOn(typeof(IFoo<>)).AsBase()`)               |
| `AsAllInterfaces()`                                                                         | Every interface implemented                                                                                 |
| `AsAllNonSystemInterfaces()`                                                                | Every interface except `System.*`                                                                           |
| `AsDefaultInterfaces()`                                                                     | Interfaces whose name appears in the class name (e.g. `CustomerService` → `ICustomerService`)               |
| `AsDefaultNonSystemInterfaces()`                                                            | Default interfaces, excluding `System.*`                                                                    |
| `AsFirstInterface()`                                                                        | The first interface in metadata order                                                                       |
| `AsInterface()`                                                                             | Top-level interfaces derived from `BasedOn` types                                                           |
| `AsInterface<T>()` / `AsInterface(Type)`                                                    | Top-level interfaces derived from `T`                                                                       |
| `AsInterfaces(params Type[])`                                                               | Top-level interfaces derived from the given types                                                           |
| `AsAllTypes()` / `AsDefaultTypes()` / `AsAllNonSystemTypes()` / `AsDefaultNonSystemTypes()` | Like the `Interfaces` variants but for base types                                                           |
| `As(Func<Type, IEnumerable<Type>>)`                                                         | Custom mapping                                                                                              |
| `As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>>)`                                    | Custom mapping with access to base types from `BasedOn`                                                     |
| `AsServicesFromAttribute<TAttribute>(…)` / `AsServicesFromAttributeOrSelf<TAttribute>(…)`   | Service types projected from an attribute (`…OrSelf` falls back to self)                                    |

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

| Method                                    | Lifetime  | Notes                                                                                           |
|-------------------------------------------|-----------|-------------------------------------------------------------------------------------------------|
| `AsSingleton()`                           | Singleton | One instance per container                                                                      |
| `AsScoped()`                              | Scoped    | One instance per scope                                                                          |
| `AsTransient()`                           | Transient | New instance per resolution                                                                     |
| `AsLifetime(ServiceLifetime)`             | Custom    | Explicit lifetime for the whole chain                                                           |
| `AsLifetime(Func<Type, ServiceLifetime>)` | Per type  | Lifetime computed from the implementation type                                                  |
| `AsLifetimeByAttribute<TAttribute>(...)`  | Per type  | Lifetime projected from an attribute (falls back to Singleton)                                  |

Skipping the stage defaults to `Singleton`.

**Sharing is automatic.** When one implementation is mapped to multiple service types under a `Singleton` or `Scoped` lifetime **and the implementation type is itself one of those service types**, it is registered once and the other service types forward to it, so they resolve to a single shared instance. Otherwise — a single service type, a transient lifetime, or a selection that excludes the implementation — each service type is registered independently. See [shared-services.md](shared-services.md) for the full model.

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

## Compile-time registration: `[Service]` source generator

The attribute-driven alternative to scanning. Annotate types with `[Service]`; a Roslyn generator — shipped **inside** the Registration package as an analyzer — collects them into a compile-time `Services.FromThisAssembly()` list, so there is no startup reflection. Full guide: [source-generator.md](source-generator.md).

```csharp
[Service]                                                    // self, singleton
public class Clock;

[Service, Scoped]
[As<IFoo>, As<IBar>]                                         // self + IFoo + IBar, one shared scoped instance
public class FooBar : IFoo, IBar;

[Service]
[As<IEmailSender>("smtp"), As<IEmailSender>("ses")]          // one instance, two keyed registrations of one type
public class Emailer : IEmailSender;
```

| Attribute                                  | Meaning                                                                                      |
|--------------------------------------------|----------------------------------------------------------------------------------------------|
| `[Service]`                                | Marks the type for registration (bare = self, singleton). One per type.                      |
| `[As<T>(key?)]` / `[As(Type, key?)]`       | Adds a service type with an optional per-type key. Repeatable; non-generic for open generics |
| `[Singleton]` / `[Scoped]` / `[Transient]` | Sets the lifetime; defaults to `Singleton`. At most one                                      |
| `[Keyed(key)]`                             | Keys the implementation's own registration                                                   |

Bracket grouping is cosmetic (`[Service, Scoped]` == `[Service][Scoped]`). Semantics match `Service.From(...).As(...)`: self-backing, one shared instance for `Singleton`/`Scoped`, `Transient` independent. Diagnostics: `ZCDI001`–`ZCDI004`.

**Consume** the generated `ServiceFilter`. Its filters mirror the [type filters](#type-filters) but match on the **implementation type**; there is no service/key/lifetime stage (the attribute already decided those):

```csharp
services.Add(Services.FromThisAssembly());                                 // add every [Service]
services.Add(Services.FromThisAssembly().BasedOn<IRepository>());          // filtered
Services.FromThisAssembly().InNamespace("MyApp.Infrastructure").ToServiceCollection(services);
```

Filters: `Where`, `InNamespace`, `InSameNamespaceAs`, `NameEndsWith`, `BasedOn`, `HasAttribute`/`HasAttributes`, `GenericTypes`, `GenericTypeDefinitions`, `ConstructedGenericTypes`. Terminal: `services.Add(...)` or `.ToServiceCollection(services)` — no `AddSingleton/AddScoped/AddTransient`, so attribute-chosen lifetimes are never overridden.

- **Same assembly.** The generated `Services` class is `[Embedded]` + `internal`: the `[Service]` types and the `Services.FromThisAssembly()` call site must live in the **same** assembly.
- **`ZCDI001`** — a `Key` that is an array is flagged (arrays compare by reference, so keyed lookups never match). The `params Type[]` constructor list is not a key and is never flagged.

See the [Registration Sample](../samples/RegistrationSample/README.md) for a runnable comparison of this path and the reflection scan.

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
