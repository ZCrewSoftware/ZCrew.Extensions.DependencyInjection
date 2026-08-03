# ZCrew.Extensions.DependencyInjection

Extensions for `Microsoft.Extensions.DependencyInjection` that add decorators and convention-based registration.

## Packages

| Package                                             | What it does                                       |
|-----------------------------------------------------|----------------------------------------------------|
| `ZCrew.Extensions.DependencyInjection`              | Decorator support for `IServiceCollection`         |
| `ZCrew.Extensions.DependencyInjection.Registration` | Castle Windsor style convention-based registration |

## Installation

```xml
<ItemGroup>
  <!-- Decorators -->
  <PackageReference Include="ZCrew.Extensions.DependencyInjection" Version="3.0.0" />

  <!-- Convention-based registration, plus the [Service] source generator -->
  <PackageReference Include="ZCrew.Extensions.DependencyInjection.Registration" Version="3.0.0" />
</ItemGroup>
```

The two are independent, so take whichever you need. The Registration package carries the `[Service]` source generator inside it as an analyzer, which means a normal `PackageReference` is the whole setup: no separate generator package, and no `OutputItemType="Analyzer"`.

## Decorators

Wrap a service that's already registered with extra behavior (logging, caching, validation, retries) without touching the original class.

```csharp
using ZCrew.Extensions.DependencyInjection;

services.AddSingleton<IEmailService, EmailService>();
services.AddSingletonDecorator<IEmailService, LoggingEmailService>();
```

The decorator takes the service it wraps through its `IEmailService` constructor parameter, and the container wires that up for you.

### Lifetimes

| Method                  | Decorator lifetime           |
|-------------------------|------------------------------|
| `AddSingletonDecorator` | Singleton                    |
| `AddScopedDecorator`    | Scoped                       |
| `AddTransientDecorator` | Transient                    |
| `AddDecorator`          | Same as the service it wraps |

Each one has an `AddKeyed*` variant for keyed services, and both type-based and factory-based overloads.

### Stacking decorators

Decorators are applied in registration order, so the last one you register ends up on the outside:

```csharp
services.AddSingleton<IEmailService, EmailService>();
services.AddDecorator<IEmailService, FilteredEmailService>();
services.AddDecorator<IEmailService, LoggingEmailService>();

// Call chain: LoggingEmailService → FilteredEmailService → EmailService
```

### Lifetime validation

A decorator that outlives the service it wraps is a captive dependency, so registering one throws an `InvalidOperationException` (a singleton decorator around a transient service, for example). Use `AddDecorator` and it takes the lifetime of the service it wraps.

## Convention-based registration

Scan assemblies and register services by convention, with a fluent API modelled on Castle Windsor.

> [!TIP]
> Need to look something up? The **[registration cheat sheet](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/registration-cheat-sheet.md)** has every entry point, filter, selector, keyed overload and lifetime helper on one page, plus recipes you can paste. Bookmark it.

```csharp
using ZCrew.Extensions.DependencyInjection.Registration;

services.AddSingleton(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .AsInterface()
);
```

### Entry points

| Entry point                           | Where types come from                                              |
|---------------------------------------|--------------------------------------------------------------------|
| `Classes.From(types)`                 | A list you provide, filtered to concrete non-abstract classes      |
| `Classes.FromAssembly(assembly)`      | An assembly, concrete non-abstract classes only                    |
| `Classes.FromAssemblyContaining<T>()` | The assembly containing `T`                                        |
| `Classes.FromThisAssembly()`          | The calling assembly                                               |
| `Types.From(types)`                   | A list you provide, all type kinds                                 |
| `Types.FromAssembly(assembly)`        | An assembly, all type kinds                                        |

### Visibility

When you scan an assembly you choose what's included:

```csharp
Classes.FromAssembly(assembly).IncludePublicTypes()    // public only (default)
Classes.FromAssembly(assembly).IncludeInternalTypes()  // public and internal
Classes.FromAssembly(assembly).IncludeAllTypes()       // everything, nested types included
```

### Filtering types

Narrow things down with `Where`, `BasedOn` or a namespace filter:

```csharp
// By predicate
Classes.FromThisAssembly()
    .Where(t => !t.Name.StartsWith("Legacy"))
    .AsSelf()

// By base type
Classes.FromThisAssembly()
    .BasedOn<IRepository>()
    .AsInterface()

// By namespace
Classes.FromThisAssembly()
    .InNamespace("MyApp.Services")
    .AsDefaultInterfaces()
```

### Service selection

Decide what each class registers as. Selectors chain, so `AsSelf().AsAllInterfaces()` registers the union of both:

| Method                           | Registers as                                                                          |
|----------------------------------|---------------------------------------------------------------------------------------|
| `AsSelf()`                       | The implementation type                                                               |
| `AsAllInterfaces()`              | Every interface the type implements                                                   |
| `AsAllNonSystemInterfaces()`     | Every interface not from `System.*`                                                   |
| `AsDefaultInterfaces()`          | Interfaces matching the type by name (`CustomerService` → `ICustomerService`)         |
| `AsDefaultNonSystemInterfaces()` | The same, not counting `System.*`                                                     |
| `AsFirstInterface()`             | The first interface the type implements                                               |
| `AsInterface()`                  | Top-level interfaces deriving from the base types set with `BasedOn`                  |
| `AsInterface<T>()`               | Top-level interfaces deriving from `T`                                                |
| `AsBase()`                       | The base types set with `BasedOn`                                                     |
| `As(type => ...)`                | Whatever your delegate returns                                                        |

### Keyed services

Add service keys after selection with `Keyed`:

```csharp
services.AddSingleton(
    Classes.FromAssemblyContaining<Startup>()
        .BasedOn<IPaymentGateway>()
        .AsInterface()
        .Keyed()  // PayPalPaymentGateway → key "PayPal", StripePaymentGateway → key "Stripe"
);
```

| Method                             | What it does                                                                |
|------------------------------------|-----------------------------------------------------------------------------|
| `Keyed()`                          | Works the key out by stripping the service name off the implementation name |
| `Keyed(object?)`                   | One key for every registration (`null` means no key)                        |
| `Keyed(Func<Type, object?>)`       | Key per implementation type (`null` means no key)                           |
| `Keyed(Func<Type, Type, object?>)` | Key from the implementation and service type together                       |

### Adding to `IServiceCollection`

Pass the chain to `services.AddSingleton`, `AddScoped` or `AddTransient`:

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

There's a Windsor-style `services.Add(chain.AsSingleton())` form too. It's mostly useful for the per-type lifetime helpers that have no bulk-add equivalent, like `chain.AsLifetimeByAttribute<TAttribute>(...)`.

### Compile-time `[Service]` registration

Would you rather declare registrations on the type itself? The package bundles a source generator as an analyzer, so there's nothing extra to install. Annotate a type with `[Service]` and it's collected at compile time into an assembly-local `Services.FromThisAssembly()` (a `ServiceFilter`), with no reflection at startup:

```csharp
[Service, Scoped, As<IEmailSender>]
public class Emailer : IEmailSender;

// then, in the same assembly (narrow it first with .Where(...) / .BasedOn<T>() if you like):
services.Add(Services.FromThisAssembly());
```

See [Compile-time registration with `[Service]`](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/source-generator.md) for the whole picture.

## Documentation

The [docs](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/tree/main/docs) folder has the longer guides:

- [Introduction](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/introduction.md)
- [Decorators](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/decorators.md)
- [Convention-based registration](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/registration.md)
- [Compile-time registration with `[Service]`](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/source-generator.md)
- [Services](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/services.md)
- **[Registration cheat sheet](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/registration-cheat-sheet.md)**, the one-page reference. Start here when you need to look something up
- [Type selectors](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/type-selectors.md)
- [Type filters](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/type-filters.md)
- [Service selectors](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/service-selectors.md)
- [Service key selectors](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/service-key-selectors.md)
- [Shared services](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/shared-services.md)
- [Coming from Castle Windsor](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/castle-windsor-comparison.md)
