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
- **Lifetime selection** per chain (`AsSingleton`, `AsScoped`, `AsTransient`) or per type from a `[Lifetime]` attribute (`AsLifetimeByAttribute`)
- **Automatic instance sharing** — when one impl is registered (including itself) against multiple service types under a singleton or scoped lifetime, they all resolve to a single shared instance

See [registration.md](registration.md) for the narrative guide and [registration-cheat-sheet.md](registration-cheat-sheet.md) for a one-page API reference.

## Upgrading between versions

- [v1 → v2](upgrades/v1-to-v2.md)
- [v2 → v3](upgrades/v2-to-v3.md)
