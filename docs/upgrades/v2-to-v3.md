# Upgrading from v2 to v3

v3 of `ZCrew.Extensions.DependencyInjection.Registration` replaces the per-stage chain **interfaces** with a concrete **class hierarchy**. Each stage is now a `public` class that derives from the next, so skipping a stage falls out of ordinary inheritance instead of interface bridging. Chains built through the `Classes` / `Types` entry points keep working unchanged — the break only affects code that named the old interfaces directly.

The decorator library (`ZCrew.Extensions.DependencyInjection`) is unaffected.

## At a glance

| Change                                                              | Severity | What to do                                                                                                                       |
|---------------------------------------------------------------------|----------|----------------------------------------------------------------------------------------------------------------------------------|
| Chain stages are concrete classes, not interfaces                   | Low      | Recompile. Retype any interface-typed local/field/parameter to the class (or `var`).                                             |
| The six chain interfaces were removed                               | Low      | Replace `IServiceSource` → `ServiceSource`, `IServiceSelector` → `ServiceSelector`, etc. `ITypeSelector` folds into `TypeFilter`. |
| Custom implementations of a stage interface no longer compile       | Low      | The stages are `internal`-constructor classes you cannot implement; build chains through `Classes` / `Types` instead.            |
| Assembly scanning is fully deferred until the terminal call         | None     | No action — this is a non-breaking laziness improvement.                                                                          |

## Interfaces became classes

In v2 each stage of the chain was an interface, with internal implementation classes and abstract `*Base` bridge classes behind them. In v3 the interfaces and bridges are gone; the stage is the class:

| v2 interface           | v3 class                        |
|------------------------|---------------------------------|
| `IAssemblyTypeSelector`| `AssemblyTypeSelector` (sealed) |
| `ITypeSelector`        | *(removed — folded into `TypeFilter`)* |
| `ITypeFilter`          | `TypeFilter`                    |
| `IServiceSelector`     | `ServiceSelector`               |
| `IServiceKeySelector`  | `ServiceKeySelector`            |
| `IServiceSource`       | `ServiceSource`                 |

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

Skipping a stage is now inheritance rather than an interface default: the default for a skipped stage (selection → self, key → unkeyed, lifetime → `Singleton` + `SharedComponent`) is installed in each stage's constructor, so terminating early produces exactly the same descriptors it did in v2.

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

## Assembly scanning is fully deferred

A small, non-breaking improvement: `Classes.FromAssembly*` / `Types.FromAssembly*` no longer touch the assembly when the chain is built. The scan (`GetExportedTypes()` / `GetTypes()`) and every filter run only when the chain is terminated with `ToServiceCollection()` or `AddSingleton(...)`. (Lifetime helpers such as `AsSingleton()` are themselves lazy — they return a `ServiceSource` without scanning.) Building — and discarding — a chain now does no reflection work.
