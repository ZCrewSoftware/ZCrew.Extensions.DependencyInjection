# ZCrew.Extensions.DependencyInjection.Registration

Convention-based service registration for `Microsoft.Extensions.DependencyInjection`, modelled on Castle Windsor's registration API. Scan assemblies, filter types, and register the lot through a fluent chain.

> [!TIP]
> One-page reference: the **[registration cheat sheet](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/registration-cheat-sheet.md)**.
> Every entry point, filter, selector, keyed overload and lifetime helper in one place, plus recipes you can paste. The fastest way to find the method you're after.

## What you get

- **Assembly scanning.** Scan whole assemblies, or hand it an explicit list of types
- **Type filters.** By base type (`BasedOn`), namespace (`InNamespace`) or your own predicate (`Where`)
- **Service mapping.** Register as an interface (`AsInterface`), every interface (`AsAllInterfaces`), the type itself (`AsSelf`), the base type (`AsBase`), or whatever your delegate says. Chain selectors to combine them, like `AsSelf().AsAllInterfaces()`
- **Naming conventions.** `AsDefaultInterfaces` matches types to interfaces by name, so `OrderService` goes to `IOrderService`
- **Keyed services.** Set keys directly, work them out from the names, or use your own selector
- **Visibility control.** Include or skip internal types when scanning

## Installation

```bash
dotnet add package ZCrew.Extensions.DependencyInjection.Registration
```

or in your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="ZCrew.Extensions.DependencyInjection.Registration" Version="3.0.0" />
</ItemGroup>
```

That single reference brings both the runtime API and the `[Service]` source generator. The generator ships inside the package as an analyzer, so NuGet wires it into the compiler for you: no second package, and none of the `OutputItemType="Analyzer"` or `PrivateAssets="all"` wiring you'd write to pull a generator in from a project reference.

## Quick start

Register every repository in the current assembly:

```csharp
services.AddSingleton(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .AsInterface()
);
```

Register services by naming convention, so `OrderService` registers as `IOrderService`:

```csharp
services.AddSingleton(
    Classes.FromThisAssembly()
        .InNamespace("MyApp.Services")
        .AsDefaultInterfaces()
);
```

### The chain

Each step narrows or transforms what gets registered:

```
Entry point → Type filtering → Service selection → Keyed selection → Lifetime → Terminal
```

```csharp
services.AddSingleton(
    Classes.FromThisAssembly()          // scan the calling assembly
        .IncludeInternalTypes()         // internal types too (optional)
        .BasedOn<IHandler>()            // only IHandler implementations
        .Where(t => !t.IsNested)        // and not nested ones
        .AsInterface()                  // register each as its IHandler interface
        .Keyed()                        // keys worked out from the type names
);
```

### Entry points

| Method                           | Where types come from                                           |
|----------------------------------|-----------------------------------------------------------------|
| `Classes.FromThisAssembly()`     | Concrete classes in the calling assembly                        |
| `Classes.FromAssembly(assembly)` | Concrete classes in a specific assembly                         |
| `Classes.From(types)`            | Concrete classes from a list you provide                        |
| `Types.FromThisAssembly()`       | All types (interfaces, structs, etc.) in the calling assembly   |

### Service mapping

Selectors return a `ServiceSelector`, so you can chain them. `AsSelf().AsAllInterfaces()` registers the union of both.

| Method                  | Registers as                                          |
|-------------------------|-------------------------------------------------------|
| `AsInterface()`         | Top-level interfaces deriving from the `BasedOn` type |
| `AsAllInterfaces()`     | Every interface implemented                           |
| `AsDefaultInterfaces()` | Interfaces matching by name                           |
| `AsSelf()`              | The implementation type                               |
| `AsBase()`              | The `BasedOn` base types                              |
| `As(selector)`          | Whatever your delegate returns                        |

### Keyed services

```csharp
// Work the key out from the names (PayPalGateway → key "PayPal" for IPaymentGateway)
.Keyed()

// One key for everything
.Keyed("myKey")

// Your own selector
.Keyed(implType => implType.Name)
```

## Compile-time `[Service]` registration

The package also bundles a source generator as an analyzer, so there's nothing extra to install. Annotate a type with `[Service]` and it's collected at compile time into an assembly-local `Services.FromThisAssembly()` (a `ServiceFilter`), with no reflection at startup:

```csharp
using Microsoft.Extensions.DependencyInjection;
using ZCrew.Extensions.DependencyInjection.Registration;

[Service, Scoped, As<IEmailSender>]
public class Emailer : IEmailSender;

// then, in the same assembly:
services.Add(Services.FromThisAssembly());                          // everything
services.Add(Services.FromThisAssembly().BasedOn<IEmailSender>());  // or narrow it down first
```

See **[Compile-time registration with `[Service]`](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/source-generator.md)** for the rest of the attributes (`[As<T>]`, `[Singleton]` / `[Scoped]` / `[Transient]`, `[Keyed]`), what they do, and the `ZCDI001` to `ZCDI004` diagnostics.

## Trimming and native AOT

The package is marked `IsAotCompatible`, but only the compile-time path is trim-safe:

| Path                                            | Trimming and AOT                                                       |
|-------------------------------------------------|------------------------------------------------------------------------|
| `[Service]` + `Services.FromThisAssembly()`     | ✅ Safe. The registrations are baked in at compile time.                |
| `Service.From<T>()` / `Service.From(typeof(T))` | ✅ Safe. The implementation keeps its constructors and interfaces.      |
| `Classes` / `Types` assembly scanning           | ⚠️ Not supported. The entry points are `[RequiresUnreferencedCode]`.   |

Assembly scanning can't be made trim-safe. The trimmer removes unreferenced types before the scan runs, so services would quietly vanish from the container. Rather than let that fail at runtime, every `Classes` and `Types` entry point warns at compile time (one `IL2026` per chain) and points you at the generator. If you publish trimmed or AOT, use `[Service]`.

> One thing that's out of this package's hands: registering open generic service types goes through `MakeGenericType`, which Microsoft's container can't make AOT-safe.

## Full API reference

The summaries above cover the common cases. For every method, overload and recipe on one page, see the **[registration cheat sheet](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/docs/registration-cheat-sheet.md)**. The longer guides live in the [docs folder](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/tree/main/docs).

## License

MIT. See [LICENSE.md](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/LICENSE.md).
