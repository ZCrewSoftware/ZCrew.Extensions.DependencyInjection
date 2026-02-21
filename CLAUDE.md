# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A set of .NET libraries extending `Microsoft.Extensions.DependencyInjection`:

- **ZCrew.Extensions.DependencyInjection** — Adds **decorator pattern** support. Provides `IServiceCollection` extension methods (`AddDecorator`, `AddScopedDecorator`, etc.) that wrap existing service registrations with decorator implementations, supporting type-based and factory-based registration, keyed services, and lifetime validation.
- **ZCrew.Extensions.DependencyInjection.Registration** — Adds **Castle Windsor-style convention-based registration**. Provides a fluent API (`Classes`, `Types`) for scanning assemblies, filtering types, and bulk-registering services by convention (e.g., `Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface()`).

## Build & Test Commands

```bash
dotnet build                    # Build everything
dotnet test                     # Run all tests
dotnet tool run CSharpier format .  # Format code (CSharpier)
```

## Tech Stack

- **.NET 10** / C# 14 (uses `extension(T)` syntax for extension methods)
- **Central package management** via `Directory.Packages.props`
- **xUnit v3** (not v2) with `Microsoft.Testing.Platform` runner
- **NSubstitute** for mocking
- **CSharpier** for formatting (pre-commit hook)

## Architecture

### Decorator Library

The library is a single project with one public API surface: `DecoratorServiceCollectionExtensions`, a `partial class` split across files by lifetime:

| File | Methods |
|------|---------|
| `DecoratorServiceCollectionExtensions.Any.cs` | `AddDecorator`, `AddKeyedDecorator` (inherit delegate lifetime) |
| `DecoratorServiceCollectionExtensions.Singleton.cs` | `AddSingletonDecorator`, `AddKeyedSingletonDecorator` |
| `DecoratorServiceCollectionExtensions.Scoped.cs` | `AddScopedDecorator`, `AddKeyedScopedDecorator` |
| `DecoratorServiceCollectionExtensions.Transient.cs` | `AddTransientDecorator`, `AddKeyedTransientDecorator` |
| `DecoratorServiceCollectionExtensions.cs` | Core `AddDecorator`/`TryAddDecorator` logic (internal) |

**How decoration works:** The core algorithm in `DecoratorServiceCollectionExtensions.cs` scans the `IServiceCollection` for matching services, reassigns each to a unique `Guid` service key (via `ServiceDescriptorExtensions.WithServiceKey`), then adds a new `ServiceDescriptor` whose factory resolves the original via that key and wraps it with the decorator.

**Key internal types:**
- `DecoratorServiceDescriptor` — describes a decorator registration (service type, decorator type/factory, optional lifetime, optional service key). Its `ToServiceDescriptor` method produces the actual `ServiceDescriptor` added to the container.
- `ServiceTimelineExtensions.Exceeds` — enforces lifetime validation (e.g., a singleton decorator cannot wrap a transient service).

### Registration Library

A fluent API for convention-based service registration, modeled after Castle Windsor's registration API. The chain flows through four stages:

**Entry points** (`Classes` / `Types`) → **Type selection** (`ITypeSelector`) → **Type filtering** (`ITypeFilter`) → **Service selection** (`IServiceSelector`) → **Terminal** (`IServiceSource` / `IServiceCollection`)

| Stage | Interface | Implementations | Purpose |
|-------|-----------|----------------|---------|
| Entry | — | `Classes`, `Types` | Static factories: `From(types)`, `FromAssembly()`, `FromThisAssembly()` |
| Type selection | `ITypeSelector`, `IAssemblyTypeSelector` | `EnumerableTypeSelector`, `AssemblyTypeSelector` | Select source types, optionally by visibility (`IncludePublicTypes`, `IncludeInternalTypes`) |
| Type filtering | `ITypeFilter` | `TypeFilter` | Filter by namespace, predicate (`Where`), or base type (`BasedOn`) |
| Service selection | `IServiceSelector` | `ServiceSelector` | Map impl→service type: `AsInterface()`, `AsAllInterfaces()`, `AsDefaultInterfaces()`, `AsSelf()`, `AsBase()`, etc. |
| Terminal | `IServiceSource` | `ServiceCollectionSource`, `ServiceSource` (lazy) | The resulting `IServiceCollection` of `ServiceDescriptor`s |

**Key design details:**
- `Classes` filters to concrete, non-abstract classes; `Types` includes all type kinds (interfaces, structs, enums, etc.).
- `ITypeSelector` has default interface implementations that bridge directly to `ITypeFilter` methods (in `ITypeSelector.ITypeFilter.cs`), and `ITypeFilter` similarly bridges to `IServiceSelector` (in `ITypeFilter.IServiceSelector.cs`). This allows callers to skip stages in the chain.
- `TypeFilter` maintains an immutable chain — each call returns a new instance. It tracks `baseTypes` set via `BasedOn`, which default to `[typeof(object)]` (match everything) until explicitly overridden.
- `LazyServiceCollection` defers evaluation until first access and is read-only (mutations throw `InvalidOperationException`).
- `TypeExtensions` (in the base DI project) provides helpers used by the registration API: `IsInNamespace`, `IsInSameNamespaceAs`, `GetInterfaceName` (strips leading `I`), and `GetTopLevelInterfaces` (most-derived interfaces only).

### Fixtures

The `fixtures/` directory contains projects that mirror real-world code for integration testing:

- **Fixtures.SmallProject** — A domain-driven design fixture with three layers:
  - `Domain/` — Entities (`Customer`, `Order`, etc.), repository interfaces (`IRepository<T>`, `IOrderRepository`), validators (`OrderValidator`, `CustomerValidator`), and value types (`Currency` struct, `OrderStatus` enum)
  - `Application/` — Service interfaces and implementations (`CustomerService`, `OrderService`), port interfaces (`IEventPublisher`, `IPaymentGateway`), and decorator examples (`AuditServiceDecorator`)
  - `Infrastructure/` — Repository implementations (`SqlCustomerRepository`, `InMemoryRepository<T>`), external integrations (`StripePaymentGateway`), notification senders
  - Includes internal types, nested classes, static classes, and generic types for thorough visibility and type-filtering test coverage

## Code Conventions

- **Formatting:** CSharpier (auto-runs via pre-commit hook). Run `dotnet tool run CSharpier format .` to format manually.
- **C# 14 extensions:** This codebase uses the new `extension(T)` blocks rather than traditional `static` extension method syntax.
- **Field naming:** Private instance fields use `this.fieldName` (no underscore prefix). Static fields use `camelCase`.
- **Internals visible to tests:** `src/Directory.Build.props` auto-exposes internals to `*.Tests`, `*.UnitTests`, and `*.IntegrationTests` assemblies.