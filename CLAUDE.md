# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A set of .NET libraries extending `Microsoft.Extensions.DependencyInjection`:

- **ZCrew.Extensions.DependencyInjection** — Adds **decorator pattern** support. Provides `IServiceCollection` extension methods (`AddDecorator`, `AddScopedDecorator`, etc.) that wrap existing service registrations with decorator implementations, supporting type-based and factory-based registration, keyed services, and lifetime validation.
- **ZCrew.Extensions.DependencyInjection.Registration** — Adds **Castle Windsor-style convention-based registration**. Provides a fluent API (`Classes`, `Types`) for scanning assemblies, filtering types, and bulk-registering services by convention (e.g., `Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface()`). Also ships a **compile-time `[Service]` source generator** (the `ZCrew.Extensions.DependencyInjection.Generator` project, packed as an analyzer inside this package) that replaces reflection-based scanning with a generated `Services.FromThisAssembly()` list. See [`docs/source-generator.md`](docs/source-generator.md).

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

| File                                                | Methods                                                         |
|-----------------------------------------------------|-----------------------------------------------------------------|
| `DecoratorServiceCollectionExtensions.Any.cs`       | `AddDecorator`, `AddKeyedDecorator` (inherit delegate lifetime) |
| `DecoratorServiceCollectionExtensions.Singleton.cs` | `AddSingletonDecorator`, `AddKeyedSingletonDecorator`           |
| `DecoratorServiceCollectionExtensions.Scoped.cs`    | `AddScopedDecorator`, `AddKeyedScopedDecorator`                 |
| `DecoratorServiceCollectionExtensions.Transient.cs` | `AddTransientDecorator`, `AddKeyedTransientDecorator`           |
| `DecoratorServiceCollectionExtensions.cs`           | Core `AddDecorator`/`TryAddDecorator` logic (internal)          |

**How decoration works:** The core algorithm in `DecoratorServiceCollectionExtensions.cs` scans the `IServiceCollection` for matching services, reassigns each to a unique `Guid` service key (via `ServiceDescriptorExtensions.WithServiceKey`), then adds a new `ServiceDescriptor` whose factory resolves the original via that key and wraps it with the decorator.

**Key internal types:**
- `DecoratorServiceDescriptor` — describes a decorator registration (service type, decorator type/factory, optional lifetime, optional service key). Its `ToServiceDescriptor` method produces the actual `ServiceDescriptor` added to the container.
- `ServiceTimelineExtensions.Exceeds` — enforces lifetime validation (e.g., a singleton decorator cannot wrap a transient service).

### Registration Library

A fluent API for convention-based service registration, modeled after Castle Windsor's registration API. Each stage is a concrete `public` class that derives from the next; skipping a stage is ordinary inheritance. The chain flows:

**Entry points** (`Classes` / `Types`) → **Type selection** (`AssemblyTypeSelector`) → **Type filtering** (`TypeFilter`) → **Service selection** (`ServiceSelector`) → **Service key selection** (`ServiceKeySelector`) → **Lifetime selection** (`ServiceLifetimeSelector`) → **Terminal** (`ServiceSource` → `IServiceCollection`)

Inheritance runs base-first: `AssemblyTypeSelector : TypeFilter : ServiceSelector : ServiceKeySelector : ServiceLifetimeSelector : ServiceSource`.

| Stage                 | Class                           | Purpose                                                                                                                                                                                                                                                                                                              |
|-----------------------|---------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Entry                 | `Classes`, `Types`              | Static factories: `From(types)` returns `TypeFilter`; `FromAssembly()` / `FromThisAssembly()` return `AssemblyTypeSelector`                                                                                                                                                                                          |
| Type selection        | `AssemblyTypeSelector` (sealed) | Assembly scan with visibility scoping (`IncludePublicTypes`, `IncludeInternalTypes`, `IncludeAllTypes`), deferred until terminal enumeration                                                                                                                                                                         |
| Type filtering        | `TypeFilter`                    | Filter by namespace, predicate (`Where`), or base type (`BasedOn`); `AllTypes()` transitions on                                                                                                                                                                                                                      |
| Service selection     | `ServiceSelector`               | Map impl→service type: `As(...)`, `AsInterface()`, `AsAllInterfaces()`, `AsDefaultInterfaces()`, `AsSelf()`, `AsBase()`, etc. Each `As*` returns `ServiceSelector`, so selectors chain and **accumulate** — the impl is registered against the distinct union (first-occurrence order) of all selected service types |
| Service key selection | `ServiceKeySelector`            | Assign keys: `Keyed()`, `Keyed(key)`, `Keyed(selector)`, or `Unkeyed()` (each returns `ServiceLifetimeSelector`)                                                                                                                                                                                                     |
| Lifetime selection    | `ServiceLifetimeSelector`       | Choose lifetime: `AsLifetime(...)`, `AsSingleton()`, `AsScoped()`, `AsTransient()`, `AsLifetimeByAttribute<TAttribute>(...)`, etc. (defaults to `Singleton`); each returns `ServiceSource`. Sharing is automatic when the impl is one of multiple selected services                                                  |
| Terminal              | `ServiceSource`                 | The resulting `IServiceCollection` of `ServiceDescriptor`s via `ToServiceCollection`                                                                                                                                                                                                                                 |

**Key design details:**
- `Classes` filters to concrete, non-abstract classes; `Types` includes all type kinds (interfaces, structs, enums, etc.).
- **Skipping a stage is inheritance.** Each stage constructor computes its *default* transition and passes it to `base(...)` — selection defaults to self (`AsSelf`), key defaults to unkeyed, lifetime defaults to `Singleton`. A skipped stage is just the default the constructor already installed. Stage methods (`Where`/`BasedOn`/`Keyed`) rebuild the next instance from their own raw fields, never from what was passed to `base(...)`, so defaults are never double-applied. **Exception:** `As` (and the `As*` selector helpers) returns another `ServiceSelector` — the *same* stage — and *accumulates* onto the existing selection via `Service.AsUnchecked` (which `Concat`s), so chained selectors union their service types (deduped, first-occurrence order) rather than replacing.
- All stage classes have `internal` constructors (blocks external subclassing while letting the entry points and derived stages construct them). Only `AssemblyTypeSelector` is `sealed`.
- The chain is immutable and fully lazy — each call returns a new instance, and no type source is enumerated (nor any assembly scanned) until the terminal `ToServiceCollection` call (or an equivalent bulk-add). Lifetime helpers (`AsLifetime` / `AsSingleton` / etc.) are also lazy: they return a `ServiceSource`, not an `IServiceCollection`. `TypeFilter` tracks `baseTypes` set via `BasedOn`, which default to `[typeof(object)]` (match everything) until explicitly overridden.
- `ServiceCollectionExtensions` provides `AddSingleton`/`AddScoped`/`AddTransient` overloads — one per concrete stage class — so a chain stopped at any stage binds there instead of to MSDI's generic `AddSingleton<TService>(IServiceCollection, TService)` instance overload.
- `TypeExtensions` (in the base DI project) provides helpers used by the registration API: `IsInNamespace`, `IsInSameNamespaceAs`, `GetInterfaceName` (strips leading `I`), and `GetTopLevelInterfaces` (most-derived interfaces only).

### Registration Source Generator

`ZCrew.Extensions.DependencyInjection.Generator` is a `netstandard2.0` Roslyn incremental generator that scans for the `[Service]` attribute and emits a compile-time registration list, replacing the reflection-based `Classes`/`Types` scan for the attribute-driven path. It ships **inside the Registration NuGet package** as an analyzer (`analyzers/dotnet/cs`), not as a separate package.

**Layout** — the generator project sits flat under `src/` (no `roslyn/` subfolder); its `.csproj` layers the analyzer overrides (`TargetFramework=netstandard2.0`, `IncludeBuildOutput=false`, `CopyLocalLockFileAssemblies=true`, `IsPackable=false`, drops the inherited MEDI.Abstractions reference) on top of `src/Directory.Build.props`. It depends on the `ZCrew.Extensions.CodeAnalysis.CSharp` helper package (referenced, not vendored).

**Packing the analyzer** — `Registration.csproj` ships two files into `analyzers/dotnet/cs` with plain `<None Pack="true">` items pointed at the generator's output dir (`../…Generator/bin/$(Configuration)/netstandard2.0`): the generator DLL and its one runtime dependency, `ZCrew.Extensions.CodeAnalysis.CSharp.Abstractions.dll` (present there because the generator sets `CopyLocalLockFileAssemblies=true`). The `ProjectReference` alongside them is `ReferenceOutputAssembly="false"` with **no** `OutputItemType="Analyzer"` — it exists purely for build ordering, since the `None` items read the output but don't build it. Do not add `OutputItemType="Analyzer"` here: Registration has no `[Service]` types, so it only injects the embedded attribute definitions into `Registration.dll` and forces a `CS0436` suppression downstream.

This hardcodes the generator's TFM and both DLL names, which is only safe while the helper package keeps its [runtime-dependency contract](docs/codeanalysis-helper-contract.md) — exactly one runtime assembly, `Abstractions.dll`, at `netstandard2.0`. If that ever changes, the packed analyzer breaks silently (`CS8785` → `CS0103: 'Services' does not exist`).

| Piece                                  | Type                       | Purpose                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
|----------------------------------------|----------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `[Service]`/`[As<T>]`/`[As]`/`[Singleton]`/`[Scoped]`/`[Transient]`/`[Keyed]` | Generator (embedded) | Bare `[Service]` root marker (`AllowMultiple=false`) + modifiers. `As` adds a service type with an optional per-type key (repeatable; non-generic form for open generics); lifetime markers set the lifetime; `[Keyed]` keys the impl's own registration. Bracket grouping is cosmetic. Defined `internal [Embedded]` in the Generator project, emitted into the consumer via post-init (`Add{Name}Definition()`), so no public attributes exist in the runtime lib. See [[embedded-attribute-meta-generator]].                                                                                                                                                                                                                                                                             |
| `Service.From(Type, ServiceLifetime, object? key, params (Type,object?)[])` | Registration lib (runtime) | Public `[EditorBrowsable(Never)]` primitive bridge the generated code calls; seeds `[impl]`, lifts lifetime/impl-key, stores each `(serviceType, key)` pair (no assignability re-check). Takes primitives, not an attribute instance, because the attributes are assembly-local to the consumer.                                                                                                                                                                                                                                                                                                          |
| `ServiceFilter`                        | Registration lib (runtime) | The type `FromThisAssembly()` returns: an immutable/lazy filter over the generated `Service`s. Mirrors `TypeFilter`'s filters (`Where`, `InNamespace`, `NameEndsWith`, `BasedOn`, `HasAttribute`/`HasAttributes`, `GenericTypes`, …) over `ImplementationType` (minus base-type state), terminating in `ToServiceCollection(...)` or `services.Add(filter)`. Deliberately **not** `IEnumerable<Service>`, so raw LINQ (`Select`/`Append`/`Zip`) isn't exposed. |
| `ServiceRegistrationSourceGenerator`   | Generator                  | Concrete generator: scans metadata name `…Registration.ServiceAttribute`, registers the embedded attribute definitions, renders each type's `Service.From` args via `ServiceConstructionRenderer`, emits `Services.FromThisAssembly()`. |
| `ServiceRegistrationAnalyzer`          | Generator                  | Emits **ZCDI001** (array `[Keyed]`/`[As]` key), **ZCDI002** (modifier without `[Service]`), **ZCDI003** (`[As]` type not assignable from the impl — `As<T>` is unconstrained by design, CS0699), **ZCDI004** (multiple lifetime markers). |

**Embedded attributes** — the `[Service]` family is defined `internal [Embedded]` in the Generator project (`Attributes/`) and emitted into the consuming compilation via `RegisterPostInitializationOutput` (the package-generated `Add{Name}Definition()` helpers). There is **no** public `ServiceAttribute` in the Registration lib, so a transitive reference without the generator can't silently reference `[Service]` — it fails to compile. `AsAttribute` and `AsAttribute<T>` share one file (the meta-generator groups by name-without-arity and dedups whole-file source; separate files would concatenate two file-scoped namespaces). The generator reads them back by **metadata-name string** (FAWMN / `GetAttributes`), never as compiled symbols. See [[embedded-attribute-meta-generator]].

**Emitted shape** — `[Embedded] internal static class Services` with `FromThisAssembly()` returning a `ServiceFilter` wrapping a `Service[]`, one `Service.From(typeof(impl), lifetime, key, (typeof(svc), key), …)` element per `[Service]` type (one per type; `AllowMultiple=false`), all `global::`-qualified, entries ordinally sorted for determinism. The entry point is skipped when no `[Service]` type exists, but the embedded attribute definitions are always emitted. `[Embedded]` makes the entry point assembly-local, so the attributed types **and** the `Services.FromThisAssembly()` call site must live in the same assembly (it cannot be consumed from a fixture project).

**Consumption chain:** `Services.FromThisAssembly().Where(...).ToServiceCollection(services)` (or `services.Add(Services.FromThisAssembly().Where(...))`) — `FromThisAssembly()` returns a `ServiceFilter` whose filters each return a `ServiceFilter` and match on `ImplementationType`; zero or more filter steps, then the terminal (`ToServiceCollection` or `Add`). There is deliberately no service/key/lifetime selection on this path — the attributes already decided those, and `ServiceFilter` intentionally exposes only filters + the terminal, not raw LINQ. Only `Add(ServiceFilter)` exists (no `AddSingleton/AddScoped/AddTransient`), so lifetimes are never overridden.

**Usage:**

```csharp
[Service]                                                    // self, singleton
public class Clock;

[Service, Scoped]
[As<IFoo>, As<IBar>]                                         // self + IFoo + IBar, one shared scoped instance
public class FooBar : IFoo, IBar;

[Service]
[As<IEmailSender>("smtp"), As<IEmailSender>("ses")]          // one instance, two keyed registrations of one type
public class Emailer : IEmailSender;

// then, in the same assembly:
services.Add(Services.FromThisAssembly());                                    // no-filter bulk add
services.Add(Services.FromThisAssembly().BasedOn<IFoo>());                    // filtered (ServiceFilter) bulk add
```

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

## Preferred Registration Patterns

When writing examples in code, docs, samples, or test fixtures, **prefer the bulk-add extensions**:

```csharp
// Preferred — most concise, matches MS DI idiom (AddSingleton<T>, AddScoped<T>, AddTransient<T>):
services.AddSingleton(Classes.FromThisAssembly().BasedOn<IRepository>().AsInterface());
services.AddScoped(Classes.FromThisAssembly().InNamespace("MyApp.Services").AsDefaultInterfaces());
services.AddTransient(Classes.FromThisAssembly().BasedOn(typeof(IValidator<>)).AsBase());

// Acceptable Windsor-style alternative — for per-type lifetime helpers that have no
// bulk-add equivalent (AsLifetime(Func), AsLifetimeByAttribute<TAttribute>):
services.Add(Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>().AsLifetimeByAttribute<LifestyleAttribute>(a => a.Lifetime));
```

Both forms work without importing `Microsoft.Extensions.DependencyInjection.Extensions` — the Registration project ships its own `Add(IServiceCollection)` / `Add(ServiceSource)` extensions to keep callers from needing it.

The `Add{Singleton,Scoped,Transient}(chain)` extensions live in `ZCrew.Extensions.DependencyInjection.Registration.ServiceCollectionExtensions` and exist for every stage of the chain (`ServiceSource`, `ServiceLifetimeSelector`, `ServiceKeySelector`, `ServiceSelector`, `TypeFilter`, `AssemblyTypeSelector`). Reserve `services.Add(chain.AsXxx())` for per-type lifetime helpers (`AsLifetime(Func<Type, ServiceLifetime>)`, `AsLifetimeByAttribute<TAttribute>(...)`) that have no bulk-add equivalent — `AsXxx()` returns a `ServiceSource`, which the `Add(ServiceSource)` overload accepts directly.
