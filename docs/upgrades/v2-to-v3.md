# Upgrading from v2 to v3

v3 of `ZCrew.Extensions.DependencyInjection.Registration` makes two breaking changes: it replaces the per-stage chain **interfaces** with a concrete **class hierarchy**, and it removes the configurable **`SharingMode`** option in favor of automatic instance sharing. Each stage is now a `public` class that derives from the next, so skipping a stage falls out of ordinary inheritance instead of interface bridging. Chains built through the `Classes` / `Types` entry points keep working unchanged — the breaks affect only code that named the old interfaces directly or called one of the removed sharing helpers (`AsSingletonDependent`, `AsScopedIndependent`, …).

The decorator library (`ZCrew.Extensions.DependencyInjection`) is unaffected.

## At a glance

| Change                                                        | Severity | What to do                                                                                                                        |
|---------------------------------------------------------------|----------|-----------------------------------------------------------------------------------------------------------------------------------|
| Chain stages are concrete classes, not interfaces             | Low      | Recompile. Retype any interface-typed local/field/parameter to the class (or `var`).                                              |
| The six chain interfaces were removed                         | Low      | Replace `IServiceSource` → `ServiceSource`, `IServiceSelector` → `ServiceSelector`, etc. `ITypeSelector` folds into `TypeFilter`. |
| Custom implementations of a stage interface no longer compile | Low      | The stages are `internal`-constructor classes you cannot implement; build chains through `Classes` / `Types` instead.             |
| `SharingMode` and its lifetime helpers were removed           | Medium   | Drop the `Dependent` / `Independent` suffix and use `AsSingleton` / `AsScoped` / `AsTransient`. Sharing is now automatic (see below).  |
| Assembly scanning is fully deferred until the terminal call   | None     | No action — this is a non-breaking laziness improvement.                                                                          |
| Service selectors now chain and accumulate                    | None     | Additive. Optionally combine selectors — e.g. `AsSelf().AsAllInterfaces()` — to register the distinct union of their service types. |

## Interfaces became classes

In v2 each stage of the chain was an interface, with internal implementation classes and abstract `*Base` bridge classes behind them. In v3 the interfaces and bridges are gone; the stage is the class:

| v2 interface            | v3 class                               |
|-------------------------|----------------------------------------|
| `IAssemblyTypeSelector` | `AssemblyTypeSelector` (sealed)        |
| `ITypeSelector`         | *(removed — folded into `TypeFilter`)* |
| `ITypeFilter`           | `TypeFilter`                           |
| `IServiceSelector`      | `ServiceSelector`                      |
| `IServiceKeySelector`   | `ServiceKeySelector`                   |
| `IServiceSource`        | `ServiceSource`                        |

The classes form a single inheritance chain, base last:

```csharp
// v3
public sealed class AssemblyTypeSelector : TypeFilter { }
public class TypeFilter : ServiceSelector { }
public class ServiceSelector : ServiceKeySelector { }
public class ServiceKeySelector : ServiceLifetimeSelector { }
public class ServiceLifetimeSelector : ServiceSource { }
public class ServiceSource { }
```

Skipping a stage is now inheritance rather than an interface default: the default for a skipped stage (selection → self, key → unkeyed, lifetime → `Singleton`) is installed in each stage's constructor, so terminating early produces the expected descriptors.

## Migrating call sites

**Chains keep compiling.** Anything built and terminated in one fluent expression is unaffected, because the entry points and the extension methods now return the concrete classes:

```csharp
// Compiles unchanged in v2 and v3
services.AddSingleton(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .AsInterface()
);
```

You only need to change code that **named an interface**:

```csharp
// v2
ITypeFilter filter = Classes.From(types).BasedOn<IRepository>();
IServiceSource source = filter.AsInterface().Unkeyed();

// v3 — use the class name, or var
TypeFilter filter = Classes.From(types).BasedOn<IRepository>();
ServiceLifetimeSelector source = filter.AsInterface().Unkeyed();
```

The entry-point return types changed accordingly:

- `Classes.From(...)` / `Types.From(...)` now return `TypeFilter` (were `ITypeSelector`).
- `Classes.FromAssembly*(...)` / `Types.FromAssembly*(...)` now return `AssemblyTypeSelector` (were `IAssemblyTypeSelector`).

You will hit a hard break only if:

- You wrote your own implementation of `IServiceSelector` / `ITypeFilter` / `IServiceSource` (or any of the six interfaces) — those interfaces no longer exist. The stages have `internal` constructors and cannot be subclassed; construct chains through `Classes` / `Types`.
- You used `nameof(IServiceSource)` (or another interface) or reflected over one of the interfaces — replace the reference with the corresponding class.

## Selectors now chain and accumulate

Every service selector (`AsSelf`, `AsInterface`, `AsAllInterfaces`, `AsBase`, `AsServicesFromAttribute`, …) now returns a **`ServiceSelector`** rather than a `ServiceKeySelector`. This is source-compatible — a `ServiceSelector` still exposes `Keyed` / `Unkeyed` and the lifetime helpers, so existing single-selector chains keep compiling — and it makes selectors **chainable**:

```csharp
Classes.From(typeof(SqlCustomerRepository))
    .AsSelf()          // SqlCustomerRepository
    .AsAllInterfaces() // plus every interface — SqlCustomerRepository is not repeated
```

Chained selectors accumulate: the implementation is registered against the **distinct union** of every selected service type, preserving first-occurrence order. This is the idiomatic way to include the implementation among its service types for a [shared instance](../shared-services.md) (see below).

## `SharingMode` was removed

v2 exposed a second axis alongside the lifetime — a `SharingMode` (`SharedComponent` / `Dependent` / `Independent`) — surfaced through the `AsSingletonDependent()`, `AsSingletonIndependent()`, `AsScopedDependent()`, and `AsScopedIndependent()` helpers and the `AsLifetime(ServiceLifetime, SharingMode)` overload. All of these are gone in v3, along with the `SharingMode` enum itself.

Sharing is now **automatic**: when an implementation is registered against multiple service types under a `Singleton` or `Scoped` lifetime **and the implementation type is itself one of those service types**, it is registered once and the other service types forward to it (a single shared instance). In every other case each service type is registered independently. See [shared-services.md](../shared-services.md) for the full model.

Migrate each removed helper:

| v2                                                   | v3                                                                                                                                                                                               |
|------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `AsSingleton()` / `AsScoped()`                       | Unchanged. Now shares only when the implementation is one of the selected services — include it in the selection (e.g. chain `.AsSelf().AsAllInterfaces()`) to share one instance.                |
| `AsSingletonIndependent()` / `AsScopedIndependent()` | `AsSingleton()` / `AsScoped()` selecting interfaces only (e.g. `AsAllInterfaces()`), which is now independent by default.                                                                        |
| `AsSingletonDependent()` / `AsScopedDependent()`     | Chain the implementation into the selection: `.AsSelf().AsAllInterfaces().AsSingleton()` (or `.AsScoped()`) registers the implementation once and forwards its interfaces to it — one shared instance. |
| `AsLifetime(lifetime, SharingMode)`                  | `AsLifetime(lifetime)` — the sharing argument is gone.                                                                                                                                           |

> **Behavioral change:** in v2, `AsAllInterfaces().AsSingleton()` shared one instance across every interface (the implementation was self-backed under a hidden key). In v3 the implementation is not one of the selected services, so each interface resolves to its own instance. Chain `AsSelf()` — `AsSelf().AsAllInterfaces().AsSingleton()` — to add the implementation to the selection and restore a single shared instance.

## Assembly scanning is fully deferred

A small, non-breaking improvement: `Classes.FromAssembly*` / `Types.FromAssembly*` no longer touch the assembly when the chain is built. The scan (`GetExportedTypes()` / `GetTypes()`) and every filter run only when the chain is terminated with `ToServiceCollection()` or `AddSingleton(...)`. (Lifetime helpers such as `AsSingleton()` are themselves lazy — they return a `ServiceSource` without scanning.) Building — and discarding — a chain now does no reflection work.
