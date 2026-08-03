# Registration cheat sheet

Quick reference for `ZCrew.Extensions.DependencyInjection.Registration`. For the longer explanation of any of it, see [registration.md](registration.md).

```
Classes/Types → IncludeXxxTypes → Where/BasedOn/InNamespace/... → AsXxx  → Keyed → AsSingleton/AsScoped/AsTransient → ToServiceCollection/AddXxx
    entry           visibility                filter              service    key               lifetime                         terminal
```

Every stage after the entry point is optional. Skip one and the next call uses a sensible default.

## Entry points

| Method                                | Where types come from                                                             |
|---------------------------------------|-----------------------------------------------------------------------------------|
| `Classes.From(params Type[])`         | A list you provide, concrete non-abstract classes only                            |
| `Classes.FromAssembly(Assembly)`      | Scan an assembly                                                                  |
| `Classes.FromAssemblyContaining<T>()` | Scan the assembly containing `T`                                                  |
| `Classes.FromThisAssembly()`          | Scan the calling assembly                                                         |
| `Types.*`                             | Same factories, but include interfaces, abstracts, structs, enums, static classes |

`Classes` is what you want most of the time. Use `Types` when you need to find interface types or value types.

## Single services

`Service.From` skips the chain and registers one known type against the service types you name. It starts registered as the type itself, and everything you add forwards to it, so a `Singleton` or `Scoped` service shares one instance.

| Method                         | What it does                                              |
|--------------------------------|-----------------------------------------------------------|
| `Service.From(Type)`           | Start a service, registered against the type itself       |
| `As<T1>()` … `As<T1, …, T8>()` | Add one to eight service types, forwarded to the type     |
| `As(Type)`                     | Add one service type, forwarded to the type               |
| `As(IEnumerable<Type>)`        | Add several service types, forwarded to the type          |
| `AsLifetime(ServiceLifetime)`  | Set the lifetime. Defaults to `Singleton`                 |
| `AsLifetime(Func<Type, …>)`    | Set the lifetime from the implementation type             |
| `Keyed(object?)` / `Unkeyed()` | Add or remove a service key                               |
| `services.Add(service, …)`     | Add one or more services (`params`)                       |

```csharp
services.Add(Service.From(typeof(PayPalPaymentGateway)).As<IPaymentGateway, IDisposable>());
// Both interfaces resolve to one shared PayPalPaymentGateway
```

You can also pick service types by convention with the same selectors the chain uses. The implementation is kept, so the service stays shared.

| Method                                                     | Service types added                                        |
|------------------------------------------------------------|------------------------------------------------------------|
| `AsAllInterfaces()` / `AsAllNonSystemInterfaces()`         | Every interface, optionally skipping `System.*`             |
| `AsDefaultInterfaces()` / `AsDefaultNonSystemInterfaces()` | Interfaces whose name appears in the class name             |
| `AsFirstInterface()`                                       | The first interface in metadata order                       |
| `AsAllTypes()` / `AsDefaultTypes()` / `…NonSystemTypes()`  | As above, plus non-abstract base classes                    |
| `AsServicesFromAttribute<TAttribute>(…)`                   | Service types read from an attribute on the implementation  |

```csharp
services.Add(Service.From<PayPalPaymentGateway>().AsAllNonSystemInterfaces());
// PayPalPaymentGateway and IPaymentGateway, sharing one instance
```

There's no `AsSelf()`, `AsBase()` or `*OrSelf` here. The implementation is always in the list already, and there's no `BasedOn` stage to supply base types.

`As` throws an `ArgumentException` if the implementation isn't assignable to a service type, including ones named by an attribute. If a selector matches nothing, the implementation stays registered on its own instead of dropping out. Open generic services can't have service types added, because they can't be forwarded. More in [services.md](services.md).

## Assembly visibility

Only available after `FromAssembly*`, which returns an `AssemblyTypeSelector`. Public types by default.

| Method                   | Picks                                                                    |
|--------------------------|--------------------------------------------------------------------------|
| `IncludePublicTypes()`   | Public types only (default)                                              |
| `IncludeInternalTypes()` | Public and top-level internal types                                      |
| `IncludeAllTypes()`      | Everything, including nested and compiler-generated. Pair with a filter. |

## Type filters

| Method                                                              | What it does                                                              |
|---------------------------------------------------------------------|---------------------------------------------------------------------------|
| `AllTypes()`                                                        | No filter, everything through                                             |
| `Where(Func<Type, bool>)`                                           | Your own predicate                                                        |
| `BasedOn<T>()` / `BasedOn(params Type[])`                           | Types assignable to any of the given bases. Open generics work            |
| `NameEndsWith(string [, bool ignoreCase [, CultureInfo?]])`         | Match by name suffix. Overloads for case, culture and `StringComparison`  |
| `GenericTypes()`                                                    | Any generic type, open or closed                                          |
| `GenericTypeDefinitions()`                                          | Open generics only                                                        |
| `ConstructedGenericTypes()`                                         | Closed generics only                                                      |
| `InNamespace(string [, bool includeSubnamespaces])`                 | Restrict to a namespace                                                   |
| `InSameNamespaceAs<T>([bool])` / `InSameNamespaceAs(Type [, bool])` | Restrict to another type's namespace                                      |

`InNamespace*` returns a `ServiceSelector`, so namespace filtering commits you to the service selection stage.

## Service selectors

Map each implementation to one or more service types. Selectors return a `ServiceSelector`, so you can chain them, like `AsSelf().AsAllInterfaces()`, to register the union of their service types (see [combining selectors](service-selectors.md#combining-selectors)).

| Method                                                                                      | Service types                                                                                 |
|---------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------|
| `AsSelf()`                                                                                  | The implementation type                                                                       |
| `AsBase()`                                                                                  | The base types from `BasedOn`. Good with open generics: `BasedOn(typeof(IFoo<>)).AsBase()`    |
| `AsAllInterfaces()`                                                                         | Every interface implemented                                                                   |
| `AsAllNonSystemInterfaces()`                                                                | Every interface not from `System.*`                                                           |
| `AsDefaultInterfaces()`                                                                     | Interfaces whose name appears in the class name (`CustomerService` → `ICustomerService`)      |
| `AsDefaultNonSystemInterfaces()`                                                            | The same, not counting `System.*`                                                             |
| `AsFirstInterface()`                                                                        | The first interface in metadata order                                                         |
| `AsInterface()`                                                                             | Top-level interfaces deriving from the `BasedOn` types                                        |
| `AsInterface<T>()` / `AsInterface(Type)`                                                    | Top-level interfaces deriving from `T`                                                        |
| `AsInterfaces(params Type[])`                                                               | Top-level interfaces deriving from any of the given types                                     |
| `AsAllTypes()` / `AsDefaultTypes()` / `AsAllNonSystemTypes()` / `AsDefaultNonSystemTypes()` | Like the interface versions, but for base types                                               |
| `As(Func<Type, IEnumerable<Type>>)`                                                         | Your own mapping                                                                              |
| `As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>>)`                                    | Your own mapping, with the `BasedOn` base types                                                |
| `AsServicesFromAttribute<TAttribute>(…)` / `AsServicesFromAttributeOrSelf<TAttribute>(…)`   | Service types read from an attribute. `…OrSelf` falls back to self                            |

More in [service-selectors.md](service-selectors.md).

## Keyed registration

Optional, and applied after service selection.

| Method                             | What it does                                                                           |
|------------------------------------|----------------------------------------------------------------------------------------|
| `Unkeyed()`                        | Explicitly no key (the default)                                                        |
| `Keyed()`                          | Key from the names, stripping the service name off the implementation name (`StripePaymentGateway` → `"Stripe"`) |
| `Keyed(object?)`                   | One key for every registration (`null` means unkeyed)                                  |
| `Keyed(Func<Type, object?>)`       | Key from the implementation type                                                       |
| `Keyed(Func<Type, Type, object?>)` | Key from the implementation and service type                                           |

Examples in [service-key-selectors.md](service-key-selectors.md).

## Lifetime

The lifetime stage, on `ServiceLifetimeSelector`. Each call returns a `ServiceSource`, so finish with `.ToServiceCollection()` or a bulk add (`services.AddSingleton(...)`, `services.Add(...)`).

| Method                                    | Lifetime  | Notes                                                        |
|-------------------------------------------|-----------|--------------------------------------------------------------|
| `AsSingleton()`                           | Singleton | One instance per container                                   |
| `AsScoped()`                              | Scoped    | One instance per scope                                       |
| `AsTransient()`                           | Transient | New instance every resolve                                   |
| `AsLifetime(ServiceLifetime)`             | Whichever | One lifetime for the whole chain                             |
| `AsLifetime(Func<Type, ServiceLifetime>)` | Per type  | Worked out from the implementation type                      |
| `AsLifetimeByAttribute<TAttribute>(...)`  | Per type  | Read from an attribute, falling back to singleton            |

Skip the stage and you get `Singleton`.

**Sharing is automatic.** When one implementation maps to several service types under `Singleton` or `Scoped`, and the implementation type is one of those service types, it's registered once and the rest forward to it, so they all resolve to the same object. Otherwise (one service type, a transient lifetime, or a selection without the implementation) each service type is registered on its own. Full model in [shared-services.md](shared-services.md).

## Adding to the container

Use the bulk-add extensions. Nothing to import beyond `ZCrew.Extensions.DependencyInjection.Registration`:

```csharp
services.AddSingleton(Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface());
services.AddScoped(   Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface());
services.AddTransient(Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface());
```

There's an overload for every stage of the chain (`AssemblyTypeSelector`, `TypeFilter`, `ServiceSelector`, `ServiceKeySelector`, `ServiceLifetimeSelector`, `ServiceSource`), so you can stop the chain early.

The Windsor-style alternative is to set the lifetime on the chain and pass the result to `services.Add`:

```csharp
services.Add(
    Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface().AsScoped()
);
```

## Compile-time registration with `[Service]`

The alternative to scanning. Annotate types with `[Service]` and a Roslyn generator, shipped inside the Registration package as an analyzer, collects them into a `Services.FromThisAssembly()` list at compile time, so there's no reflection at startup. Full guide: [source-generator.md](source-generator.md).

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

| Attribute                                  | What it does                                                                              |
|--------------------------------------------|-------------------------------------------------------------------------------------------|
| `[Service]`                                | Marks the type for registration. Bare means self, singleton. One per type.                |
| `[As<T>(key?)]` / `[As(Type, key?)]`       | Adds a service type with an optional key. Repeatable. Non-generic form for open generics  |
| `[Singleton]` / `[Scoped]` / `[Transient]` | Sets the lifetime, default singleton. One at most                                         |
| `[Keyed(key)]`                             | Keys the implementation's own registration                                                |

Bracket grouping makes no difference (`[Service, Scoped]` is `[Service][Scoped]`). The rules match `Service.From(...).As(...)`: always registered against itself, one shared instance for `Singleton` and `Scoped`, independent for `Transient`. Diagnostics are `ZCDI001` through `ZCDI004`.

Consume the generated `ServiceFilter`. Its filters mirror the [type filters](#type-filters) but match on the implementation type. There's no service, key or lifetime stage, since the attributes already decided those:

```csharp
services.Add(Services.FromThisAssembly());                                 // everything
services.Add(Services.FromThisAssembly().BasedOn<IRepository>());          // filtered
Services.FromThisAssembly().InNamespace("MyApp.Infrastructure").ToServiceCollection(services);
```

Filters: `Where`, `InNamespace`, `InSameNamespaceAs`, `NameEndsWith`, `BasedOn`, `HasAttribute` / `HasAttributes`, `GenericTypes`, `GenericTypeDefinitions`, `ConstructedGenericTypes`. Terminals: `services.Add(...)` or `.ToServiceCollection(services)`. There's no `AddSingleton` / `AddScoped` / `AddTransient`, so the lifetimes you declared can't be overridden.

- **Same assembly.** The generated `Services` class is `[Embedded]` and `internal`, so the `[Service]` types and the `Services.FromThisAssembly()` call have to be in the same assembly.
- **`ZCDI001`** flags a key that is an array, since arrays compare by reference and keyed lookups would never match. The `params Type[]` constructor list is not a key and is never flagged.

The [Registration sample](../samples/RegistrationSample/README.md) runs both paths side by side and prints what each one registered.

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

**Everything ending in `Service`, registered as itself**

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

**Hosted services sharing one instance**

Put the implementation in the selection so the host and anything injecting an interface get the same object:

```csharp
services.AddSingleton(
    Classes.FromAssemblyContaining<Startup>()
        .BasedOn<IHostedService>()
        .As(type => type.AsAllTypes())
);
```
