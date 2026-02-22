# ZCrew.Extensions.DependencyInjection.Registration

Convention-based service registration for `Microsoft.Extensions.DependencyInjection`, inspired by Castle Windsor's registration API. Scan assemblies, filter types, and bulk-register services using a fluent interface.

## Features

- **Assembly scanning** — scan entire assemblies or provide explicit type lists
- **Type filtering** — filter by base type (`BasedOn`), namespace (`InNamespace`), or predicate (`Where`)
- **Flexible service mapping** — register as interface (`AsInterface`), all interfaces (`AsAllInterfaces`), self (`AsSelf`), base type (`AsBase`), or custom mapping
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

The API flows through four stages — each step narrows or transforms the set of registrations:

```
Entry Point → Type Filtering → Service Selection → Registration
```

```csharp
services.AddSingleton(
    Classes.FromThisAssembly()          // scan the calling assembly
        .IncludeInternalTypes()         // include internal types (optional)
        .BasedOn<IHandler>()            // filter to IHandler implementations
        .Where(t => !t.IsAbstract)      // additional predicate filtering
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

## License

This project is licensed under the MIT License - see the [LICENSE.md](../../LICENSE.md) file for details.
