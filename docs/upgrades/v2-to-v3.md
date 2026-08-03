# Upgrading from v2 to v3

v3 of `ZCrew.Extensions.DependencyInjection.Registration` makes two breaking changes. The per-stage chain interfaces are now concrete classes, and the `SharingMode` option is gone in favor of automatic sharing.

Each stage is a `public` class deriving from the next, so skipping a stage falls out of ordinary inheritance instead of interface bridging. Chains built through `Classes` or `Types` keep working as they are. The breaks only hit code that named the old interfaces directly, or called one of the removed sharing helpers (`AsSingletonDependent`, `AsScopedIndependent`, and so on).

The decorator library, `ZCrew.Extensions.DependencyInjection`, is unaffected.

## At a glance

| Change                                                        | Severity | What to do                                                                                                                        |
|---------------------------------------------------------------|----------|-----------------------------------------------------------------------------------------------------------------------------------|
| Chain stages are classes, not interfaces                      | Low      | Recompile. Retype any interface-typed local, field or parameter to the class, or use `var`.                                       |
| The six chain interfaces were removed                         | Low      | `IServiceSource` → `ServiceSource`, `IServiceSelector` → `ServiceSelector`, and so on. `ITypeSelector` folds into `TypeFilter`.   |
| You can no longer implement a stage yourself                  | Low      | The stages have internal constructors. Build chains through `Classes` / `Types` instead.                                          |
| `SharingMode` and its lifetime helpers were removed           | Medium   | Drop the `Dependent` / `Independent` suffix and use `AsSingleton` / `AsScoped` / `AsTransient`. Sharing is automatic now.         |
| Assembly scanning is fully deferred to the terminal call      | None     | Nothing. It just does less work than it used to.                                                                                 |
| Service selectors chain and accumulate                        | None     | Nothing. Optionally combine selectors, like `AsSelf().AsAllInterfaces()`, to register the union of their service types.           |

## Interfaces became classes

In v2 each stage was an interface, with internal implementations and abstract `*Base` bridge classes behind it. In v3 the interfaces and bridges are gone and the stage is the class:

| v2 interface            | v3 class                               |
|-------------------------|----------------------------------------|
| `IAssemblyTypeSelector` | `AssemblyTypeSelector` (sealed)        |
| `ITypeSelector`         | *(removed, folded into `TypeFilter`)*  |
| `ITypeFilter`           | `TypeFilter`                           |
| `IServiceSelector`      | `ServiceSelector`                      |
| `IServiceKeySelector`   | `ServiceKeySelector`                   |
| `IServiceSource`        | `ServiceSource`                        |

They form one inheritance chain, base last:

```csharp
// v3
public sealed class AssemblyTypeSelector : TypeFilter { }
public class TypeFilter : ServiceSelector { }
public class ServiceSelector : ServiceKeySelector { }
public class ServiceKeySelector : ServiceLifetimeSelector { }
public class ServiceLifetimeSelector : ServiceSource { }
public class ServiceSource { }
```

Skipping a stage is now inheritance rather than an interface default. Each stage's constructor installs the default for the stage after it (selection to self, key to unkeyed, lifetime to `Singleton`), so stopping early gives you the descriptors you'd expect.

## Migrating call sites

Chains keep compiling. Anything built and finished in one fluent expression is fine, because the entry points and extension methods return the concrete classes now:

```csharp
// Compiles unchanged in v2 and v3
services.AddSingleton(
    Classes.FromThisAssembly()
        .BasedOn<IRepository>()
        .AsInterface()
);
```

What you need to change is code that named an interface:

```csharp
// v2
ITypeFilter filter = Classes.From(types).BasedOn<IRepository>();
IServiceSource source = filter.AsInterface().Unkeyed();

// v3, use the class name or var
TypeFilter filter = Classes.From(types).BasedOn<IRepository>();
ServiceLifetimeSelector source = filter.AsInterface().Unkeyed();
```

The entry point return types changed to match:

- `Classes.From(...)` / `Types.From(...)` now return `TypeFilter`, previously `ITypeSelector`.
- `Classes.FromAssembly*(...)` / `Types.FromAssembly*(...)` now return `AssemblyTypeSelector`, previously `IAssemblyTypeSelector`.

You'll only hit a hard break if:

- You wrote your own implementation of `IServiceSelector`, `ITypeFilter`, `IServiceSource` or any of the other three. Those interfaces are gone, and the stages have internal constructors so they can't be subclassed. Build chains through `Classes` / `Types`.
- You used `nameof(IServiceSource)` or reflected over one of the interfaces. Point it at the class instead.

## Selectors chain and accumulate

Every service selector (`AsSelf`, `AsInterface`, `AsAllInterfaces`, `AsBase`, `AsServicesFromAttribute`, and the rest) now returns a `ServiceSelector` rather than a `ServiceKeySelector`.

That's source compatible, since a `ServiceSelector` still has `Keyed` / `Unkeyed` and the lifetime helpers, so existing single-selector chains keep compiling. What it buys you is chaining:

```csharp
Classes.From(typeof(SqlCustomerRepository))
    .AsSelf()          // SqlCustomerRepository
    .AsAllInterfaces() // plus every interface. SqlCustomerRepository is not repeated
```

Chained selectors accumulate. The implementation is registered against the union of every selected service type, in the order the types were first seen. This is how you get the implementation into its own service list for a [shared instance](../shared-services.md), which matters for the next section.

## `SharingMode` was removed

v2 had a second axis alongside the lifetime: a `SharingMode` of `SharedComponent`, `Dependent` or `Independent`, surfaced through `AsSingletonDependent()`, `AsSingletonIndependent()`, `AsScopedDependent()`, `AsScopedIndependent()` and the `AsLifetime(ServiceLifetime, SharingMode)` overload. All of it is gone in v3, including the `SharingMode` enum.

Sharing is automatic now. When a class is registered against several service types under `Singleton` or `Scoped`, and the class itself is one of those service types, it's registered once and the rest forward to it, giving you one shared instance. In every other case each service type is registered on its own. See [shared-services.md](../shared-services.md) for the full model.

Migrating each removed helper:

| v2                                                   | v3                                                                                                                                                    |
|------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------|
| `AsSingleton()` / `AsScoped()`                       | Unchanged, but they only share when the implementation is one of the selected service types. Chain `.AsSelf().AsAllInterfaces()` to put it there.      |
| `AsSingletonIndependent()` / `AsScopedIndependent()` | `AsSingleton()` / `AsScoped()` selecting interfaces only (`AsAllInterfaces()`), which is independent by default now.                                   |
| `AsSingletonDependent()` / `AsScopedDependent()`     | Put the implementation in the selection: `.AsSelf().AsAllInterfaces().AsSingleton()` registers it once and forwards its interfaces to it.              |
| `AsLifetime(lifetime, SharingMode)`                  | `AsLifetime(lifetime)`. The sharing argument is gone.                                                                                                 |

> **Watch out:** in v2, `AsAllInterfaces().AsSingleton()` shared one instance across every interface, because the implementation was self-backed under a hidden key. In v3 the implementation isn't one of the selected service types, so each interface gets its own instance. Chain `AsSelf()` to get the shared instance back.

## Assembly scanning is fully deferred

A small, non-breaking improvement: `Classes.FromAssembly*` and `Types.FromAssembly*` no longer touch the assembly when you build the chain. The scan (`GetExportedTypes()` / `GetTypes()`) and every filter run only when the chain is finished with `ToServiceCollection()` or `AddSingleton(...)`. The lifetime helpers are lazy too, so `AsSingleton()` returns a `ServiceSource` without scanning anything. Building a chain and throwing it away now does no reflection at all.
