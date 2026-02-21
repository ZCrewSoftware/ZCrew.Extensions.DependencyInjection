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

```csharp
using ZCrew.Extensions.DependencyInjection.Registration;

services.Add(
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
Classes.FromAssembly(assembly).IncludePublicTypes()   // Only public types (default)
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

Choose how implementation types map to service types:

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

### Adding to `IServiceCollection`

The result of the fluent chain is an `IServiceCollection`, so pass it directly to `services.Add()`:

```csharp
services.Add(
    Classes.FromAssemblyContaining<Startup>()
        .BasedOn<IRepository>()
        .AsInterface()
);

services.Add(
    Classes.FromAssemblyContaining<Startup>()
        .InSameNamespaceAs<CustomerService>()
        .AsDefaultInterfaces()
);
```

## Documentation

See the [docs](docs) folder for detailed guides:

- [Introduction](docs/1-introduction.md)
- [Getting Started](docs/2-getting-started.md)
- [Decorators](docs/3-decorators.md)
- [Convention-Based Registration](docs/4-registration.md)
