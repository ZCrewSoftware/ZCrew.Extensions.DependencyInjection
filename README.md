# ZCrew.Extensions.DependencyInjection

Extensions for `Microsoft.Extensions.DependencyInjection` that add **decorator support** and **convention-based registration**.

## Packages

| Package                                             | Description                                        |
|-----------------------------------------------------|----------------------------------------------------|
| `ZCrew.Extensions.DependencyInjection`              | Decorator pattern support for `IServiceCollection` |
| `ZCrew.Extensions.DependencyInjection.Registration` | Castle Windsor-style convention-based registration |

## Decorators

Register decorators that wrap existing services with additional behavior — logging, caching, validation, retry logic — without modifying the original implementation.

```csharp
using ZCrew.Extensions.DependencyInjection;

services.AddSingleton<IEmailService, EmailService>();
services.AddSingletonDecorator<IEmailService, LoggingEmailService>();
```

The decorator constructor receives the inner service via its `IEmailService` parameter. The container wires this automatically.

### Lifetime methods

| Method                  | Decorator lifetime               |
|-------------------------|----------------------------------|
| `AddSingletonDecorator` | Singleton                        |
| `AddScopedDecorator`    | Scoped                           |
| `AddTransientDecorator` | Transient                        |
| `AddDecorator`          | Inherits the delegate's lifetime |

All methods have `AddKeyed*` variants for keyed services, and both type-based and factory-based overloads.

### Stacking decorators

Multiple decorators are resolved in registration order — the last registered is the outermost wrapper:

```csharp
services.AddSingleton<IEmailService, EmailService>();
services.AddDecorator<IEmailService, FilteredEmailService>();
services.AddDecorator<IEmailService, LoggingEmailService>();

// Resolved chain: LoggingEmailService → FilteredEmailService → EmailService
```

### Lifetime validation

The library throws `InvalidOperationException` at registration time if a decorator's lifetime exceeds its delegate's (e.g., a singleton decorator wrapping a transient service). Use `AddDecorator` to automatically inherit the delegate's lifetime.

## Convention-Based Registration

Scan assemblies and register services by convention using a fluent API inspired by Castle Windsor.

> [!TIP]
> **Looking for a quick lookup?** The **[Registration Cheat Sheet](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/registration-cheat-sheet.md)** is a single-page reference covering every entry point, filter, selector, keyed overload, and lifetime helper — with copy-paste recipes. Bookmark it.

```csharp
using ZCrew.Extensions.DependencyInjection.Registration;

services.AddSingleton(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .AsInterface()
);
```

### Entry points

| Entry point                           | Behavior                                                                      |
|---------------------------------------|-------------------------------------------------------------------------------|
| `Classes.From(types)`                 | Select from a collection of types, filtering to concrete non-abstract classes |
| `Classes.FromAssembly(assembly)`      | Scan an assembly for concrete non-abstract classes                            |
| `Classes.FromAssemblyContaining<T>()` | Scan the assembly containing `T`                                              |
| `Classes.FromThisAssembly()`          | Scan the calling assembly                                                     |
| `Types.From(types)`                   | Select from a collection of types (all type kinds)                            |
| `Types.FromAssembly(assembly)`        | Scan an assembly for all types                                                |

### Assembly visibility

When scanning assemblies, control which types are included:

```csharp
Classes.FromAssembly(assembly).IncludePublicTypes()    // Only public types (default)
Classes.FromAssembly(assembly).IncludeInternalTypes()  // Public + internal types
Classes.FromAssembly(assembly).IncludeAllTypes()       // All types including nested
```

### Filtering types

Filter which types are registered using `Where`, `BasedOn`, or namespace filters:

```csharp
// Filter by predicate
Classes.FromThisAssembly()
    .Where(t => !t.Name.StartsWith("Legacy"))
    .AsSelf()

// Filter by base type
Classes.FromThisAssembly()
    .BasedOn<IRepository>()
    .AsInterface()

// Filter by namespace
Classes.FromThisAssembly()
    .InNamespace("MyApp.Services")
    .AsDefaultInterfaces()
```

### Service selection

Choose how implementation types map to service types (selectors chain — e.g. `AsSelf().AsAllInterfaces()` — to register the distinct union of their service types):

| Method                           | Registers as                                                                                        |
|----------------------------------|-----------------------------------------------------------------------------------------------------|
| `AsSelf()`                       | The implementation type itself                                                                      |
| `AsAllInterfaces()`              | All interfaces the type implements                                                                  |
| `AsAllNonSystemInterfaces()`     | All interfaces except those in `System.*`                                                           |
| `AsDefaultInterfaces()`          | Interfaces whose name matches the type by convention (e.g., `CustomerService` → `ICustomerService`) |
| `AsDefaultNonSystemInterfaces()` | Default interfaces, excluding `System.*`                                                            |
| `AsFirstInterface()`             | The first interface the type implements                                                             |
| `AsInterface()`                  | Top-level interfaces derived from base types set via `BasedOn`                                      |
| `AsInterface<T>()`               | Top-level interfaces derived from `T`                                                               |
| `AsBase()`                       | The base types set via `BasedOn`                                                                    |
| `As(type => ...)`                | Custom selection via delegate                                                                       |

### Keyed services

Optionally assign service keys after service selection using `Keyed`:

```csharp
services.AddSingleton(
    Classes.FromAssemblyContaining<Startup>()
        .BasedOn<IPaymentGateway>()
        .AsInterface()
        .Keyed()  // PayPalPaymentGateway → key "PayPal", StripePaymentGateway → key "Stripe"
);
```

| Method                             | Behavior                                                                   |
|------------------------------------|----------------------------------------------------------------------------|
| `Keyed()`                          | Auto-detect key by stripping the service name from the implementation name |
| `Keyed(object?)`                   | Same key for all registrations (`null` = no service key)                   |
| `Keyed(Func<Type, object?>)`       | Key per implementation type (`null` return = no service key)               |
| `Keyed(Func<Type, Type, object?>)` | Key from both implementation and service type                              |

### Adding to `IServiceCollection`

Pass the chain to `services.AddSingleton`, `AddScoped`, or `AddTransient`:

```csharp
services.AddSingleton(
    Classes.FromAssemblyContaining<Startup>()
        .BasedOn<IRepository>()
        .AsInterface()
);

services.AddScoped(
    Classes.FromAssemblyContaining<Startup>()
        .InSameNamespaceAs<CustomerService>()
        .AsDefaultInterfaces()
);
```

A Windsor-style `services.Add(chain.AsSingleton())` form is also supported. It's most useful for per-type lifetime helpers that have no bulk-add equivalent — e.g. `chain.AsLifetimeByAttribute<TAttribute>(...)`.

### Compile-Time `[Service]` Registration

Prefer declaring registrations on the type itself? The package bundles a source generator (as an analyzer — nothing extra to install). Annotate a type with `[Service]` and it's collected at compile time into an assembly-local `Services.FromThisAssembly()` (a `ServiceFilter`) — no startup reflection:

```csharp
[Service, Scoped, As<IEmailSender>]
public class Emailer : IEmailSender;

// then, in the same assembly (optionally narrowed via ServiceFilter filters like .Where(...) / .BasedOn<T>()):
services.Add(Services.FromThisAssembly());
```

See [Compile-Time Registration with `[Service]`](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/source-generator.md) for the full model.

## Documentation

See the [docs](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/tree/main/docs) folder for detailed guides:

- [Introduction](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/introduction.md)
- [Decorators](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/decorators.md)
- [Convention-Based Registration](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/registration.md)
- [Compile-Time Registration with `[Service]`](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/source-generator.md)
- [Services](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/services.md)
- **[Registration Cheat Sheet](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/registration-cheat-sheet.md)** — one-page API reference (start here when you need to look something up)
- [Type Selectors](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/type-selectors.md)
- [Type Filters](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/type-filters.md)
- [Service Selectors](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/service-selectors.md)
- [Keyed Service Selectors](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/service-key-selectors.md)
- [Shared Services](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/shared-services.md)
- [Castle Windsor Comparison](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/castle-windsor-comparison.md)
