# Service Selectors

Service selectors determine **what service type** each implementation type is registered as. This stage follows [type selection](5-type-selectors.md) and [type filtering](6-type-filters.md) in the registration chain. Each service selector method returns an `IKeyedServiceSelector`, which can optionally be chained with [keyed service selection](8-keyed-service-selectors.md) via `WithServiceKey`, or used directly as an `IServiceCollection` of `ServiceDescriptor`s ready to be added to your container.

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

This method requires [`BasedOn`](6-type-filters.md#basedont--basedontype--basedonparams-type) to be called first to set the base type context:

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

## Side-by-side comparison

The following scenario is designed so that every selector produces a distinct set of registrations, making the differences easy to compare at a glance.

### Setup

```csharp
public interface IAuditable { }

public interface IHandler { }
public interface IOrderHandler : IHandler { }

public class DisposableOrderHandler : IAuditable, IOrderHandler, IDisposable
{
    public void Dispose() { }
}
```

The type hierarchy at play:

```
DisposableOrderHandler
├── IAuditable
├── IOrderHandler
│   └── IHandler
└── IDisposable (System)
```

`DisposableOrderHandler` implements four interfaces in total: `IAuditable`, `IOrderHandler`, `IHandler` (inherited via `IOrderHandler`), and `IDisposable`.
The top level interfaces are `IAuditable`, `IOrderHandler`, and `IDisposable`.

### Registration

All examples below assume:

```csharp
Classes.From(new[] { typeof(DisposableOrderHandler) }).BasedOn<IHandler>()
```

`AsInterface<T>()`, `AsInterface(Type)`, and `AsInterfaces(params Type[])` specify their base types as parameters instead of relying on `BasedOn`. The table notes where this applies.

### Results

| Selector                                             | `DisposableOrderHandler` registered as                   | Why                                                                                           |
|------------------------------------------------------|----------------------------------------------------------|-----------------------------------------------------------------------------------------------|
| `AsAllInterfaces()`                                  | `IAuditable`, `IOrderHandler`, `IHandler`, `IDisposable` | Every interface, including inherited and `System` interfaces                                  |
| `AsAllNonSystemInterfaces()`                         | `IAuditable`, `IOrderHandler`, `IHandler`                | Same as above, minus `IDisposable` (`System` namespace)                                       |
| `AsDefaultInterfaces()`                              | `IOrderHandler`, `IHandler`, `IDisposable`               | Convention match: name contains "OrderHandler", "Handler", and "Disposable" (not "Auditable") |
| `AsDefaultNonSystemInterfaces()`                     | `IOrderHandler`, `IHandler`                              | Same convention match, minus `IDisposable` (`System` namespace)                               |
| `AsFirstInterface()`                                 | `IAuditable`                                             | First declared interface only                                                                 |
| `AsInterface()`                                      | `IOrderHandler`                                          | Most-derived interface descending from `IHandler` (the `BasedOn` type)                        |
| `AsInterface(typeof(IAuditable))`                    | `IAuditable`                                             | Most-derived interface descending from `IAuditable`                                           |
| `AsInterfaces(typeof(IHandler), typeof(IAuditable))` | `IOrderHandler`, `IAuditable`                            | Most-derived interface from each specified base type                                          |
| `AsBase()`                                           | `IHandler`                                               | The `BasedOn` type itself                                                                     |
| `AsSelf()`                                           | `DisposableOrderHandler`                                 | The concrete implementation type                                                              |
