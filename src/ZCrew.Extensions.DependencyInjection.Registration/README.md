# ZCrew.Extensions.DependencyInjection.Registration

Convention-based service registration for `Microsoft.Extensions.DependencyInjection`, inspired by Castle Windsor's registration API. Scan assemblies, filter types, and bulk-register services using a fluent interface.

> [!TIP]
> **One-page API reference: [Registration Cheat Sheet](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/registration-cheat-sheet.md).**
> Every entry point, filter, selector, keyed overload, and lifetime helper in one place — plus copy-paste recipes. The fastest way to find the method you need.

## Features

- **Assembly scanning** — scan entire assemblies or provide explicit type lists
- **Type filtering** — filter by base type (`BasedOn`), namespace (`InNamespace`), or predicate (`Where`)
- **Flexible service mapping** — register as interface (`AsInterface`), all interfaces (`AsAllInterfaces`), self (`AsSelf`), base type (`AsBase`), or custom mapping; chain selectors to combine them (e.g. `AsSelf().AsAllInterfaces()`)
- **Convention-based defaults** — `AsDefaultInterfaces` matches types to interfaces by naming convention (e.g., `OrderService` to `IOrderService`)
- **Keyed services** — assign service keys statically, by convention, or with a custom selector
- **Visibility control** — include or exclude internal types when scanning assemblies

## Installation

```bash
dotnet add package ZCrew.Extensions.DependencyInjection.Registration
```

## Quick Start

Register all repository implementations from the current assembly:

```csharp
services.AddSingleton(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .AsInterface()
);
```

Register all services by naming convention (e.g., `OrderService` registers as `IOrderService`):

```csharp
services.AddSingleton(
    Classes.FromThisAssembly()
        .InNamespace("MyApp.Services")
        .AsDefaultInterfaces()
);
```

### Fluent Chain

The API flows through the following stages — each step narrows or transforms the set of registrations:

```
Entry Point → Type Filtering → Service Selection → Keyed Selection → Lifetime Selection → Terminal
```

```csharp
services.AddSingleton(
    Classes.FromThisAssembly()          // scan the calling assembly
        .IncludeInternalTypes()         // include internal types (optional)
        .BasedOn<IHandler>()            // filter to IHandler implementations
        .Where(t => !t.IsNested)        // additional predicate filtering
        .AsInterface()                  // register each as its IHandler interface
        .Keyed()                        // auto-detect service keys by convention
);
```

### Entry Points

| Method                           | Description                                                     |
|----------------------------------|-----------------------------------------------------------------|
| `Classes.FromThisAssembly()`     | Concrete classes from the calling assembly                      |
| `Classes.FromAssembly(assembly)` | Concrete classes from a specific assembly                       |
| `Classes.From(types)`            | Concrete classes from an explicit type list                     |
| `Types.FromThisAssembly()`       | All types (interfaces, structs, etc.) from the calling assembly |

### Service Mapping

Selectors return `ServiceSelector` and can be chained — e.g. `AsSelf().AsAllInterfaces()` — to register the distinct union of their service types.

| Method                  | Description                                           |
|-------------------------|-------------------------------------------------------|
| `AsInterface()`         | Top-level interfaces deriving from the `BasedOn` type |
| `AsAllInterfaces()`     | All implemented interfaces                            |
| `AsDefaultInterfaces()` | Interfaces matching by naming convention              |
| `AsSelf()`              | The implementation type itself                        |
| `AsBase()`              | The `BasedOn` base type(s)                            |
| `As(selector)`          | Custom mapping function                               |

### Keyed Services

```csharp
// Auto-detect keys by convention (PayPalGateway → key "PayPal" for IPaymentGateway)
.Keyed()

// Static key for all registrations
.Keyed("myKey")

// Custom key selector
.Keyed(implType => implType.Name)
```

## Compile-Time `[Service]` Registration

The package also bundles a source generator (as an analyzer — nothing extra to install). Annotate a type with
`[Service]` and it is collected at compile time into an assembly-local `Services.FromThisAssembly()` (a `ServiceFilter`),
no reflection at startup:

```csharp
using Microsoft.Extensions.DependencyInjection;
using ZCrew.Extensions.DependencyInjection.Registration;

[Service, Scoped, As<IEmailSender>]
public class Emailer : IEmailSender;

// then, in the same assembly:
services.Add(Services.FromThisAssembly());                          // add all [Service] registrations
services.Add(Services.FromThisAssembly().BasedOn<IEmailSender>());  // or narrow with ServiceFilter filters
```

See **[Compile-Time Registration with `[Service]`](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/source-generator.md)** for the full attribute model
(`[As<T>]`, `[Singleton]`/`[Scoped]`/`[Transient]`, `[Keyed]`), semantics, and the `ZCDI001`–`ZCDI004` diagnostics.

## Full API reference

The summaries above cover the common cases. For a complete one-page reference covering every method, overload, and recipe, see the **[Registration Cheat Sheet](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/registration-cheat-sheet.md)**. For deeper narrative guides see the [docs folder](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/tree/main/docs).

## License

This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/LICENSE.md) file for details.
