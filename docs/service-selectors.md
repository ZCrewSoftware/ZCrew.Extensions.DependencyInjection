# Service Selectors

Service selectors determine **what service type** each implementation type is registered as. This stage follows [type selection](type-selectors.md) and [type filtering](type-filters.md) in the registration chain. Each service selector method returns a `ServiceKeySelector`, which can optionally be chained with [service key selection](service-key-selectors.md) via `Keyed` and [lifetime selection](shared-components.md) (`AsSingleton`, `AsScoped`, …). Terminate the chain with `ToServiceCollection()` (or a bulk-add such as `services.AddSingleton(...)`) to produce the `IServiceCollection` of `ServiceDescriptor`s ready to be added to your container.

Service types can also be declared with **attributes on the implementation type** via `AsServicesFromAttribute` (see [Selecting services from attributes](#selecting-services-from-attributes)).

## `AsAllInterfaces()`

Registers each type against **every** interface it implements, including inherited and system interfaces:

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .BasedOn(typeof(IRepository<>))
    .AsAllInterfaces()
```

Given:

```csharp
public interface IReadOnlyRepository<T> : IDisposable, IAsyncDisposable { }
public interface IRepository<T> : IReadOnlyRepository<T> { }
public interface ICustomerRepository : IRepository<Customer> { }
public class SqlCustomerRepository : RepositoryBase<Customer>, ICustomerRepository { }
```

Registers `SqlCustomerRepository` as:

```
ICustomerRepository
IRepository<Customer>
IReadOnlyRepository<Customer>
IDisposable
IAsyncDisposable
```

## `AsAllNonSystemInterfaces()`

Like `AsAllInterfaces()`, but excludes interfaces in the `System` namespace and its sub-namespaces. This is typically what you want — it avoids polluting the container with `IDisposable`, `IAsyncDisposable`, `IEquatable<T>`, and similar framework interfaces:

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .BasedOn(typeof(IRepository<>))
    .AsAllNonSystemInterfaces()
```

Using the same types as above, registers `SqlCustomerRepository` as:

```
ICustomerRepository
IRepository<Customer>
IReadOnlyRepository<Customer>
```

`IDisposable` and `IAsyncDisposable` (both in `System`) are excluded.

## `AsDefaultInterfaces()`

Registers each type against interfaces whose name matches the type name by convention. The matching rule: the interface name (minus the `I` prefix) must appear as a case-sensitive substring in the class name.

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .InSameNamespaceAs<CustomerService>()
    .AsDefaultInterfaces()
```

Given:

```csharp
public class CustomerService : ICustomerService { }
public class OrderService : IOrderService { }
public class AuditService : IAuditService { }
public class AuditServiceDecorator : IAuditService { }
```

Registers:

```
CustomerService       → ICustomerService        ("CustomerService" contains "CustomerService")
OrderService          → IOrderService           ("OrderService" contains "OrderService")
AuditService          → IAuditService           ("AuditService" contains "AuditService")
AuditServiceDecorator → IAuditService           ("AuditServiceDecorator" contains "AuditService")
```

Note that `AuditServiceDecorator` also matches `IAuditService` because "AuditServiceDecorator" contains "AuditService". Use `Where` to exclude decorators if needed.

## `AsDefaultNonSystemInterfaces()`

Combines convention matching with system interface exclusion — equivalent to `AsDefaultInterfaces()` but also strips out interfaces from the `System` namespace:

```csharp
Classes.FromAssemblyContaining<EmailNotificationSender>()
    .AsDefaultNonSystemInterfaces()
```

Given:

```csharp
public class EmailNotificationSender : INotificationSender { }
// INotificationSender : IDisposable
```

Registers:

```
EmailNotificationSender → INotificationSender
```

`IDisposable` is excluded even though "Disposable" does not appear in the class name anyway. The system filter provides an extra safety net.

## `AsFirstInterface()`

Registers each type against the **first** interface it implements. Types with no interfaces are skipped:

```csharp
Classes.From(
    typeof(CustomerService),
    typeof(OrderService)
).AsFirstInterface()
```

Given:

```csharp
public class CustomerService : ICustomerService { }
public class OrderService : IOrderService { }
```

Registers:

```
CustomerService → ICustomerService
OrderService    → IOrderService
```

The "first" interface is determined by the runtime's reflection ordering, which typically follows declaration order but is not guaranteed by the CLR specification.

## `AsInterface()`

Registers each type against its **top-level interfaces that derive from the base types** set via `BasedOn`. "Top-level" means the most-derived interface in the hierarchy — it picks the leaf, not the root.

This method requires [`BasedOn`](type-filters.md#basedont--basedontype--basedonparams-type) to be called first to set the base type context:

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .BasedOn(typeof(IRepository<>))
    .AsInterface()
```

Given:

```
IRepository<T>
├── ICustomerRepository
│   └── SqlCustomerRepository
└── IOrderRepository
    └── SqlOrderRepository
```

Registers:

```
SqlCustomerRepository → ICustomerRepository
SqlOrderRepository    → IOrderRepository
```

`AsInterface()` picks `ICustomerRepository` (not `IRepository<Customer>`) because it's the most-derived interface descending from the `BasedOn` type.

## `AsInterface<T>()` / `AsInterface(Type)`

Like `AsInterface()`, but specifies the base interface type inline instead of relying on `BasedOn`:

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .AsInterface<IRepository<object>>()
// Won't match — use the open generic form:

Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .AsInterface(typeof(IRepository<>))
```

Given the same hierarchy as above, registers:

```
SqlCustomerRepository → ICustomerRepository
SqlOrderRepository    → IOrderRepository
```

This is convenient when you want to filter and select in one call without a separate `BasedOn` step.

## `AsInterfaces(params Type[])`

Like `AsInterface(Type)`, but accepts multiple base interface types. Each type is registered against its top-level interfaces that derive from **any** of the specified types:

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .BasedOn(typeof(IRepository<>), typeof(IValidator<>))
    .AsInterfaces(typeof(IRepository<>), typeof(IValidator<>))
```

Given:

```csharp
public class SqlCustomerRepository : RepositoryBase<Customer>, ICustomerRepository { }
public class OrderValidator : IValidator<Order> { }
```

Registers:

```
SqlCustomerRepository → ICustomerRepository   (top-level of IRepository<>)
OrderValidator        → IValidator<Order>     (top-level of IValidator<>)
```

## `As(Func<Type, Type[]>)`

Full control over service type selection via a delegate. The function receives the implementation type and returns the service types to register:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .InSameNamespaceAs<CustomerService>()
    .As(type => type.GetInterfaces()
        .Where(i => i.Name.EndsWith("Service"))
        .ToArray())
```

Given:

```csharp
public class CustomerService : ICustomerService { }
public class AuditService : IAuditService { }
```

Registers:

```
CustomerService → ICustomerService
AuditService    → IAuditService
```

## `As(Func<Type, Type[], Type[]>)`

Like the single-parameter `As`, but the delegate also receives the resolved base types from `BasedOn`. This is useful when you want to compute service types relative to the base type context:

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .BasedOn(typeof(IRepository<>))
    .As((type, baseTypes) => baseTypes)
```

Given:

```csharp
public class SqlCustomerRepository : RepositoryBase<Customer>, ICustomerRepository { }
public class SqlOrderRepository : RepositoryBase<Order>, IOrderRepository { }
```

The `baseTypes` for `SqlCustomerRepository` are the resolved forms of the `BasedOn` types — in this case `IRepository<Customer>`. Registers:

```
SqlCustomerRepository → IRepository<Customer>
SqlOrderRepository    → IRepository<Order>
```

This is equivalent to `AsBase()` in this scenario, but the delegate form allows more complex logic.

## `AsSelf()`

Registers each type as itself — the implementation type is also the service type:

```csharp
Classes.FromAssemblyContaining<OrderValidator>()
    .BasedOn(typeof(IValidator<>))
    .AsSelf()
```

Given:

```csharp
public class OrderValidator : IValidator<Order> { }
public class CustomerValidator : IValidator<Customer> { }
```

Registers:

```
OrderValidator    → OrderValidator
CustomerValidator → CustomerValidator
```

This is useful when consumers depend on the concrete type directly rather than an interface.

## `AsBase()`

Registers each type against the base types set via `BasedOn`. The base types are resolved to their closed generic forms when applicable:

```csharp
Classes.FromAssemblyContaining<OrderValidator>()
    .BasedOn(typeof(IValidator<>))
    .AsBase()
```

Given:

```csharp
public class OrderValidator : IValidator<Order> { }
public class CustomerValidator : IValidator<Customer> { }
```

Registers:

```
OrderValidator    → IValidator<Order>
CustomerValidator → IValidator<Customer>
```

The open generic `IValidator<>` in `BasedOn` is resolved to the closed form (`IValidator<Order>`, `IValidator<Customer>`) for each implementation.

## Selecting services from attributes

Instead of computing service types from interfaces or delegates, `AsServicesFromAttribute` reads the target service types from an **attribute applied to the implementation type**. This keeps the service-type declaration next to the implementation it belongs to. All overloads share the same rules:

- **Inherited attributes are inspected by default.** Each overload has a companion that takes a leading `bool inherited` parameter; pass `false` to consider only attributes declared directly on the implementation type.
- **No match means no registration.** An implementation type without a matching attribute — or whose attribute yields no service types — is **not registered at all**. Use the `…OrSelf()` companion to register such a type against itself instead.
- **A single match is required.** If a type carries more than one matching attribute, an `AmbiguousMatchException` is thrown when the chain is enumerated.
- **No assignability check.** The declared service types are used verbatim, exactly like the `As(delegate)` form. Declaring a service type the implementation does not satisfy fails at resolution time, not registration time.

### `AsServicesFromAttribute()`

Reads the service types from any attribute that implements the library's `IServiceTypesProvider` interface. The library ships a ready-made one — `[Services]` — so the common case needs no custom attribute:

```csharp
Classes.FromThisAssembly()
    .AsServicesFromAttribute()
```

Given:

```csharp
[Services(typeof(ICustomerService), typeof(IAuditService))]
public class CustomerService : ICustomerService, IAuditService { }
```

Registers:

```
CustomerService → ICustomerService
CustomerService → IAuditService
```

When a single implementation is mapped to multiple service types, the default [shared-component](shared-components.md) sharing applies, so both service types resolve to the *same* instance.

`[Services]` is declared `Inherited = false`, so its service types are **not** inherited by subclasses. Types with no `IServiceTypesProvider` attribute are not registered — unless you use the fallback:

```csharp
Classes.FromThisAssembly()
    .AsServicesFromAttributeOrSelf()
// A type without [Services] is registered as itself instead of being skipped.
```

To declare service types with your own attribute instead, implement `IServiceTypesProvider`:

```csharp
public interface IServiceTypesProvider
{
    IEnumerable<Type> ServiceTypes { get; }
}
```

Whether such a custom attribute is picked up on derived types follows *its* own `[AttributeUsage(Inherited = …)]`; pass `AsServicesFromAttribute(inherited: false)` to ignore inherited attributes.

### `AsServicesFromAttribute<TAttribute>(Func<TAttribute, IEnumerable<Type>>)`

Projects a specific attribute — one that need not know anything about `IServiceTypesProvider` — through a selector. `TAttribute` may be a concrete attribute type or an interface implemented by one or more attributes (marker-interface matching):

```csharp
Classes.FromThisAssembly()
    .AsServicesFromAttribute<ContractAttribute>(attribute => attribute.Contracts)
```

Given:

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class ContractAttribute(params Type[] contracts) : Attribute
{
    public Type[] Contracts => contracts;
}

[Contract(typeof(ICustomerService))]
public class CustomerService : ICustomerService { }
```

Registers `CustomerService → ICustomerService`. Types without the attribute, or for which the selector yields no service types, are not registered (use `AsServicesFromAttributeOrSelf<TAttribute>(…)` to fall back to self). An `inherited` overload — `AsServicesFromAttribute<TAttribute>(bool inherited, Func<TAttribute, IEnumerable<Type>>)` — controls whether inherited attributes are inspected.

### `AsServicesFromAttribute(Type, Func<Attribute, IEnumerable<Type>>)`

The non-generic form, for when the attribute type is only known at runtime. The selector receives the matching attribute as `Attribute`, so it is cast before the service types are read:

```csharp
Classes.FromThisAssembly()
    .AsServicesFromAttribute(typeof(ContractAttribute), attribute => ((ContractAttribute)attribute).Contracts)
```

This registers the same services as the generic overload above. An `inherited` overload — `AsServicesFromAttribute(Type, bool inherited, Func<Attribute, IEnumerable<Type>>)` — and an `AsServicesFromAttributeOrSelf(Type, …)` fallback are also available.

## Choosing the right selector

| Scenario                           | Selector                     | Example                                         |
|------------------------------------|------------------------------|-------------------------------------------------|
| Register against all interfaces    | `AsAllInterfaces()`          | Every interface a type implements               |
| Same, but skip `IDisposable` etc.  | `AsAllNonSystemInterfaces()` | All non-`System` interfaces                     |
| Naming convention (`Foo` → `IFoo`) | `AsDefaultInterfaces()`      | `CustomerService` → `ICustomerService`          |
| Most-derived interface from a base | `AsInterface()`              | `SqlCustomerRepository` → `ICustomerRepository` |
| Register as the base type itself   | `AsBase()`                   | `OrderValidator` → `IValidator<Order>`          |
| Register as the concrete type      | `AsSelf()`                   | `OrderValidator` → `OrderValidator`             |
| Custom logic                       | `As(delegate)`               | Full control via a function                     |
| Service types from an attribute    | `AsServicesFromAttribute()`  | `[Services(typeof(ICustomerService))]`          |

## Type-based variants

`AsAllTypes()`, `AsAllNonSystemTypes()`, `AsDefaultTypes()`, and `AsDefaultNonSystemTypes()` mirror the interface methods but match against base types instead of interfaces.
