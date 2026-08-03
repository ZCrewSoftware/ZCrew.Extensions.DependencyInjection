# Introduction

This repo ships two libraries that extend `Microsoft.Extensions.DependencyInjection`.

## Decorators

`ZCrew.Extensions.DependencyInjection` adds support for the [decorator pattern](https://refactoring.guru/design-patterns/decorator). You wrap a service that is already registered with extra behavior (logging, caching, validation, retries) without touching the original class.

- Register a decorator by type, or build it yourself with a factory
- Works with keyed services
- Pick the decorator's lifetime with `AddSingletonDecorator`, `AddScopedDecorator` or `AddTransientDecorator`, or let it inherit from the service it wraps with `AddDecorator`
- Stack as many decorators as you want on one service
- Mismatched lifetimes are caught up front, like a singleton decorator wrapping a transient service

See [decorators.md](decorators.md).

## Convention-based registration

`ZCrew.Extensions.DependencyInjection.Registration` adds Castle Windsor style registration. Instead of registering services one at a time, you scan an assembly and describe the convention.

- Assembly scanning, with control over visibility (`IncludePublicTypes`, `IncludeInternalTypes`, `IncludeAllTypes`)
- Type filters: `Where`, `BasedOn`, `InNamespace`, `NameEndsWith`, and filters for generic types
- Service selection: `AsInterface`, `AsDefaultInterfaces`, `AsSelf`, `AsBase`, or your own delegate. Chain them and you get the union, like `AsSelf().AsAllInterfaces()`
- Keyed registration, either detected from the type names or from a key selector
- Lifetimes for the whole chain (`AsSingleton`, `AsScoped`, `AsTransient`) or per type from a delegate or attribute (`AsLifetime`, `AsLifetimeByAttribute<TAttribute>`)
- Automatic instance sharing. Register one class against several service types, including itself, as a singleton or scoped, and they all resolve to the same instance

Start with [registration.md](registration.md), and keep the [cheat sheet](registration-cheat-sheet.md) open when you need to look a method up.

If you would rather declare registrations on the types themselves and skip the startup reflection, there is a [`[Service]` source generator](source-generator.md) in the same package.

## Upgrading between versions

- [v1 to v2](upgrades/v1-to-v2.md)
- [v2 to v3](upgrades/v2-to-v3.md)
