# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A .NET library that adds **decorator pattern** support to `Microsoft.Extensions.DependencyInjection`. It provides `IServiceCollection` extension methods (`AddDecorator`, `AddScopedDecorator`, etc.) that wrap existing service registrations with decorator implementations, supporting type-based and factory-based registration, keyed services, and lifetime validation.

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

## Code Conventions

- **Formatting:** CSharpier (auto-runs via pre-commit hook). Run `dotnet tool run CSharpier format .` to format manually.
- **C# 14 extensions:** This codebase uses the new `extension(T)` blocks rather than traditional `static` extension method syntax.
- **Field naming:** Private instance fields use `this.fieldName` (no underscore prefix). Static fields use `camelCase`.
- **Internals visible to tests:** `src/Directory.Build.props` auto-exposes internals to `*.Tests`, `*.UnitTests`, and `*.IntegrationTests` assemblies.