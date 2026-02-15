# Introduction

`ZCrew.Extensions.DependencyInjection` adds decorator support to Microsoft's dependency injection container (`Microsoft.Extensions.DependencyInjection`).

The [decorator pattern](https://refactoring.guru/design-patterns/decorator) lets you wrap an existing service with additional behavior — logging, caching, validation, retry logic — without modifying the original implementation. Each decorator implements the same interface as the service it wraps, so consumers are unaware of the decoration.

This library provides `IServiceCollection` extension methods that make it straightforward to register decorators. It supports:

- **Type-based** and **factory-based** decorator registration
- **Keyed services** (decorating services registered with a service key)
- **Lifetime-specific** methods (`AddSingletonDecorator`, `AddScopedDecorator`, `AddTransientDecorator`) and a **lifetime-inheriting** method (`AddDecorator`)
- **Stacking** multiple decorators on the same service
- **Lifetime validation** to prevent mismatched lifetimes (e.g., a singleton decorator wrapping a transient service)