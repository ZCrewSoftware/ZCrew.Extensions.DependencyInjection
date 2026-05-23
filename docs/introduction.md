# Introduction

This repo ships two libraries that extend `Microsoft.Extensions.DependencyInjection`:

## Decorators

`ZCrew.Extensions.DependencyInjection` adds [decorator pattern](https://refactoring.guru/design-patterns/decorator) support — wrap an existing service with additional behavior (logging, caching, validation, retry) without modifying the original implementation. It supports:

- **Type-based** and **factory-based** decorator registration
- **Keyed services**
- **Lifetime-specific** methods (`AddSingletonDecorator`, `AddScopedDecorator`, `AddTransientDecorator`) and a **lifetime-inheriting** method (`AddDecorator`)
- **Stacking** multiple decorators on the same service
- **Lifetime validation** to prevent mismatched lifetimes (e.g., a singleton decorator wrapping a transient service)

See [decorators.md](decorators.md).

## Convention-Based Registration

`ZCrew.Extensions.DependencyInjection.Registration` adds Castle Windsor-style fluent registration — scan assemblies and register services by convention instead of registering them one by one. It supports:

- **Assembly scanning** with visibility controls (`IncludePublicTypes`, `IncludeInternalTypes`, `IncludeAllTypes`)
- **Type filtering** via `Where`, `BasedOn`, `InNamespace`, `NameEndsWith`, and generic-type filters
- **Service selection** via `AsInterface`, `AsDefaultInterfaces`, `AsSelf`, `AsBase`, and custom delegates
- **Keyed registration** with auto-detection or custom key selectors
- **Sharing modes** that control whether one impl registered against multiple service types resolves to a shared instance, a factory-resolved dependent, or independent instances per service type

See [registration.md](registration.md) for the narrative guide and [registration-cheat-sheet.md](registration-cheat-sheet.md) for a one-page API reference.

## Upgrading between versions

- [v1 → v2](upgrades/v1-to-v2.md)
