# Service selectors

Service selectors decide what service type each implementation is registered as. This stage comes after [type selection](type-selectors.md) and [type filtering](type-filters.md).

Every selector returns a `ServiceSelector`, so you can chain them (see [Combining selectors](#combining-selectors)) and then carry on into [keyed selection](service-key-selectors.md) with `Keyed`, and [lifetime selection](shared-services.md) with `AsSingleton`, `AsScoped` and the rest. Finish with `ToServiceCollection()` or a bulk add like `services.AddSingleton(...)`.

You can also declare service types with an attribute on the implementation. See [Selecting services from attributes](#selecting-services-from-attributes).

## Combining selectors

Since every selector returns a `ServiceSelector`, you can chain them, and each one adds to the running selection. The implementation ends up registered against the union of everything selected, in the order the types were first seen. A service type picked by two selectors is only registered once:

```csharp
Classes.From(typeof(SqlCustomerRepository))
    .AsSelf()          // SqlCustomerRepository
    .AsAllInterfaces() // ICustomerRepository, IRepository<Customer>, ... (SqlCustomerRepository is not repeated)
```

This is how you get the implementation into its own service list so everything resolves to a single [shared instance](shared-services.md). `AsSelf().AsAllInterfaces().AsSingleton()` registers `SqlCustomerRepository` once and points every interface at it. Selectors that only map interfaces, without `AsSelf()`, register each service type on its own.

So the selectors below are not either-or. The [table at the bottom](#choosing-the-right-selector) lists the individual strategies, and you can combine as many as you need.

## `AsAllInterfaces()`

Registers each type against every interface it implements, inherited and system interfaces included:

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

`SqlCustomerRepository` registers as:

```
ICustomerRepository
IRepository<Customer>
IReadOnlyRepository<Customer>
IDisposable
IAsyncDisposable
```

## `AsAllNonSystemInterfaces()`

The same, minus anything in the `System` namespace or below it. This is usually the one you want, since it keeps `IDisposable`, `IAsyncDisposable` and `IEquatable<T>` out of your container:

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .BasedOn(typeof(IRepository<>))
    .AsAllNonSystemInterfaces()
```

With the same types as above, `SqlCustomerRepository` registers as:

```
ICustomerRepository
IRepository<Customer>
IReadOnlyRepository<Customer>
```

`IDisposable` and `IAsyncDisposable` are both in `System`, so they're dropped.

## `AsDefaultInterfaces()`

Registers each type against interfaces whose name matches it. The rule: strip the leading `I` from the interface name, and it has to appear in the class name (case sensitive).

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

You get:

```
CustomerService       → ICustomerService        ("CustomerService" contains "CustomerService")
OrderService          → IOrderService           ("OrderService" contains "OrderService")
AuditService          → IAuditService           ("AuditService" contains "AuditService")
AuditServiceDecorator → IAuditService           ("AuditServiceDecorator" contains "AuditService")
```

Watch that last one. `AuditServiceDecorator` matches `IAuditService` because the name contains it. Use `Where` to keep decorators out if that isn't what you want.

## `AsDefaultNonSystemInterfaces()`

Name matching plus the system filter:

```csharp
Classes.FromAssemblyContaining<EmailNotificationSender>()
    .AsDefaultNonSystemInterfaces()
```

Given:

```csharp
public class EmailNotificationSender : INotificationSender { }
// INotificationSender : IDisposable
```

You get:

```
EmailNotificationSender → INotificationSender
```

`IDisposable` was never going to match on name anyway, but the system filter is a useful backstop.

## `AsFirstInterface()`

Registers each type against the first interface it implements. Types with no interfaces are skipped:

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

You get:

```
CustomerService → ICustomerService
OrderService    → IOrderService
```

"First" comes from reflection ordering, which usually follows declaration order but isn't guaranteed by the CLR spec.

## `AsInterface()`

Registers each type against its top-level interfaces that derive from the base types you set with `BasedOn`. Top-level means the most derived interface, so it picks the leaf and not the root.

You have to call [`BasedOn`](type-filters.md#basedont--basedontype--basedonparams-type) first, since that's what sets the base type:

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

You get:

```
SqlCustomerRepository → ICustomerRepository
SqlOrderRepository    → IOrderRepository
```

`AsInterface()` picks `ICustomerRepository` and not `IRepository<Customer>`, because it's the most derived interface below the `BasedOn` type.

## `AsInterface<T>()` / `AsInterface(Type)`

Same idea, but you name the base interface inline instead of relying on `BasedOn`:

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .AsInterface<IRepository<object>>()
// That won't match. Use the open generic form:

Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .AsInterface(typeof(IRepository<>))
```

With the same hierarchy as above:

```
SqlCustomerRepository → ICustomerRepository
SqlOrderRepository    → IOrderRepository
```

Handy when you want to filter and select in one call instead of adding a separate `BasedOn`.

## `AsInterfaces(params Type[])`

Like `AsInterface(Type)` but takes several base interfaces. Each type is registered against its top-level interfaces deriving from any of them:

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

You get:

```
SqlCustomerRepository → ICustomerRepository   (top-level of IRepository<>)
OrderValidator        → IValidator<Order>     (top-level of IValidator<>)
```

## `As(Func<Type, Type[]>)`

Full control through a delegate. It gets the implementation type and returns the service types:

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

You get:

```
CustomerService → ICustomerService
AuditService    → IAuditService
```

## `As(Func<Type, Type[], Type[]>)`

Same, but the delegate also gets the resolved base types from `BasedOn`, which is useful when the service types depend on them:

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

The `baseTypes` for `SqlCustomerRepository` are the resolved `BasedOn` types, here `IRepository<Customer>`. So you get:

```
SqlCustomerRepository → IRepository<Customer>
SqlOrderRepository    → IRepository<Order>
```

That's what `AsBase()` does in this case. The delegate is there for when you need something more involved.

## `AsSelf()`

Registers each type as itself:

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

You get:

```
OrderValidator    → OrderValidator
CustomerValidator → CustomerValidator
```

Use it when callers depend on the concrete type rather than an interface.

## `AsBase()`

Registers each type against the base types set with `BasedOn`, resolved to their closed form where that applies:

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

You get:

```
OrderValidator    → IValidator<Order>
CustomerValidator → IValidator<Customer>
```

The open `IValidator<>` from `BasedOn` is closed per implementation, giving `IValidator<Order>` and `IValidator<Customer>`.

## Selecting services from attributes

Rather than working the service types out from interfaces or a delegate, `AsServicesFromAttribute` reads them from an attribute on the implementation. That keeps the declaration next to the class it describes. The rules are the same across all the overloads:

- Inherited attributes count by default. Each overload has a twin that takes a leading `bool inherited`. Pass `false` to only look at attributes declared on the type itself.
- No match means no registration. A type without a matching attribute, or whose attribute yields no service types, isn't registered at all. Use the `...OrSelf()` twin to fall back to registering it as itself.
- Exactly one match. Two matching attributes on a type throws an `AmbiguousMatchException` when the chain is enumerated.
- No assignability check. The service types are used as given, same as the `As(delegate)` form. Name a service type the implementation doesn't satisfy and it fails when you resolve it, not when you register it.

### `AsServicesFromAttribute<TAttribute>(Func<TAttribute, IEnumerable<Type>>)`

Reads a specific attribute. `TAttribute` can be a concrete attribute type, or an interface that one or more attributes implement:

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

That registers `CustomerService → ICustomerService`. Types without the attribute, or where the selector comes back empty, aren't registered. Use `AsServicesFromAttributeOrSelf<TAttribute>(...)` if you want them registered as themselves instead. There is also an `inherited` overload, `AsServicesFromAttribute<TAttribute>(bool inherited, Func<TAttribute, IEnumerable<Type>>)`.

### `AsServicesFromAttribute(Type, Func<Attribute, IEnumerable<Type>>)`

The non-generic form, for when you only know the attribute type at runtime. The selector gets an `Attribute`, so cast it before reading the service types:

```csharp
Classes.FromThisAssembly()
    .AsServicesFromAttribute(typeof(ContractAttribute), attribute => ((ContractAttribute)attribute).Contracts)
```

Same result as the generic overload above. There is an `inherited` overload, `AsServicesFromAttribute(Type, bool inherited, Func<Attribute, IEnumerable<Type>>)`, and an `AsServicesFromAttributeOrSelf(Type, ...)` fallback.

## Choosing the right selector

These aren't either-or. [Chain](#combining-selectors) as many as you like and the implementation is registered against the union of their service types.

| What you want                      | Selector                     | Example                                         |
|------------------------------------|------------------------------|-------------------------------------------------|
| Every interface                    | `AsAllInterfaces()`          | Every interface a type implements               |
| Every interface not from `System.*`  | `AsAllNonSystemInterfaces()` | Skips `IDisposable`, `IEquatable<T>`, etc.      |
| Naming convention (`Foo` → `IFoo`) | `AsDefaultInterfaces()`      | `CustomerService` → `ICustomerService`          |
| Most derived interface from a base | `AsInterface()`              | `SqlCustomerRepository` → `ICustomerRepository` |
| The base type itself               | `AsBase()`                   | `OrderValidator` → `IValidator<Order>`          |
| The concrete type                  | `AsSelf()`                   | `OrderValidator` → `OrderValidator`             |
| Something else entirely            | `As(delegate)`               | Full control through a function                 |
| Service types from an attribute    | `AsServicesFromAttribute<TAttribute>(…)` | `[Contract(typeof(ICustomerService))]` → `a.Contracts` |

## Type-based variants

`AsAllTypes()`, `AsAllNonSystemTypes()`, `AsDefaultTypes()` and `AsDefaultNonSystemTypes()` work like the interface versions, but match base types instead of interfaces.
