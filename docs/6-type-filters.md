# Type Filters

Type filters narrow down the set of [selected types](5-type-selectors.md) before [service selection](7-service-selectors.md). Each filter method returns a new instance, so filters can be chained without mutating previous state.

## `AllTypes()`

Accepts all remaining types without further filtering and transitions to service selection. Calling this explicitly is equivalent to skipping the filter stage entirely:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .AllTypes()
    .AsDefaultInterfaces()

// Equivalent to:
Classes.FromAssemblyContaining<CustomerService>()
    .AsDefaultInterfaces()
```

## `Where(Func<Type, bool>)`

Filters types using a custom predicate. Multiple `Where` calls can be chained — each one further narrows the set:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .Where(type => !type.Name.StartsWith("Legacy"))
    .AsDefaultInterfaces()
```

Given:

```csharp
public class OrderService : IOrderService { }
public class LegacyOrderProcessor : IOrderService { }
public class CustomerService : ICustomerService { }
```

The `Where` predicate excludes `LegacyOrderProcessor`, so the result is:

```
OrderService     → IOrderService
CustomerService  → ICustomerService
```

### Chaining `Where`

Each chained `Where` further restricts the set:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .Where(type => type.Namespace?.Contains("Application") == true)
    .Where(type => !type.Name.Contains("Caching"))
    .AsDefaultInterfaces()
```

Given types in `Fixtures.SmallProject.Application.Services`:

```csharp
public class CustomerService : ICustomerService { }
public class OrderService : IOrderService { }
public class CachingCustomerService : ICustomerService { }
```

Selects `CustomerService` and `OrderService` (both pass both predicates), excludes `CachingCustomerService` (fails the second predicate).

## `BasedOn<T>()` / `BasedOn(Type)` / `BasedOn(params Type[])`

Restricts to types that implement or inherit from the specified base type. Also sets the **base type context** used later by several service selectors including `AsInterface()`, `AsInterfaces()`, `AsBase()`, and the two-parameter `As` delegate.

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .BasedOn<IRepository<object>>()
```

This won't match anything — `BasedOn` uses exact type matching for non-generic types. For generic base types, use the open generic form:

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .BasedOn(typeof(IRepository<>))
```

Given:

```csharp
public interface IRepository<T> : IReadOnlyRepository<T> { }
public interface ICustomerRepository : IRepository<Customer> { }
public class SqlCustomerRepository : RepositoryBase<Customer>, ICustomerRepository { }
public class SqlOrderRepository : RepositoryBase<Order>, IOrderRepository { }
public class InMemoryRepository<T> : RepositoryBase<T> { }
public class CustomerService : ICustomerService { }
```

`BasedOn(typeof(IRepository<>))` selects:

```
SqlCustomerRepository, SqlOrderRepository, InMemoryRepository<T>
```

`CustomerService` is excluded because it does not implement `IRepository<>`.

### Multiple base types

`BasedOn(params Type[])` accepts multiple types. A type is included if it matches **any** of them:

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .BasedOn(typeof(IRepository<>), typeof(IValidator<>))
```

Given:

```csharp
public class SqlCustomerRepository : RepositoryBase<Customer>, ICustomerRepository { }
public class OrderValidator : IValidator<Order> { }
public class CustomerService : ICustomerService { }
```

Selects:

```
SqlCustomerRepository  (matches IRepository<>)
OrderValidator         (matches IValidator<>)
```

`CustomerService` is excluded because it matches neither base type.

### Chaining `BasedOn` with `Where`

`BasedOn` returns an `ITypeFilter`, so it can be combined with `Where`:

```csharp
Classes.FromAssemblyContaining<OrderValidator>()
    .BasedOn(typeof(IValidator<>))
    .Where(type => type.IsPublic)
    .AsBase()
```

Given:

```csharp
public class OrderValidator : IValidator<Order> { }
public class CustomerValidator : IValidator<Customer> { }
internal class InternalOrderValidator : IValidator<Order> { }
```

`BasedOn` matches all three, then `Where` excludes `InternalOrderValidator`. Result:

```
OrderValidator     → IValidator<Order>
CustomerValidator  → IValidator<Customer>
```

## `InNamespace(string)` / `InNamespace(string, bool)`

Filters to types in the specified namespace. The two-parameter overload optionally includes sub-namespaces:

```csharp
// Exact namespace match
Classes.FromAssemblyContaining<CustomerService>()
    .InNamespace("Fixtures.SmallProject.Application.Services")
    .AsDefaultInterfaces()
```

Given:

```csharp
namespace Fixtures.SmallProject.Application.Services;
public class CustomerService : ICustomerService { }
public class OrderService : IOrderService { }

namespace Fixtures.SmallProject.Infrastructure.Persistence;
public class SqlCustomerRepository : RepositoryBase<Customer>, ICustomerRepository { }
```

Selects:

```
CustomerService  → ICustomerService
OrderService     → IOrderService
```

With sub-namespaces:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .InNamespace("Fixtures.SmallProject.Application", includeSubnamespaces: true)
    .AsDefaultInterfaces()
```

This includes types in `Application.Services`, `Application.Ports`, `Application.Caching`, and `Application.Pipelines`.

## `InSameNamespaceAs(Type)` / `InSameNamespaceAs<T>()`

A convenience alternative to `InNamespace` — uses the namespace of a given type instead of a hardcoded string:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .InSameNamespaceAs<CustomerService>()
    .AsDefaultInterfaces()
```

This is equivalent to `InNamespace("Fixtures.SmallProject.Application.Services")` but avoids the magic string.

### Including sub-namespaces

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .InSameNamespaceAs<CustomerService>(includeSubnamespaces: true)
    .AsDefaultInterfaces()
```

Includes types in the same namespace as `CustomerService` and all its child namespaces.

## Combining filters

Because `BasedOn` and `Where` both return `ITypeFilter`, they compose naturally. `InNamespace` and `InSameNamespaceAs` have a declared return type of `IServiceSelector`, so the compiler won't let you chain further `Where`/`BasedOn` calls after them. Place them **after** `BasedOn`/`Where` in the chain:

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .BasedOn(typeof(IRepository<>))
    .Where(type => !type.IsGenericTypeDefinition)
    .AsInterface()
```

Given:

```csharp
public class SqlCustomerRepository : RepositoryBase<Customer>, ICustomerRepository { }
public class SqlOrderRepository : RepositoryBase<Order>, IOrderRepository { }
public class InMemoryRepository<T> : RepositoryBase<T> { }
```

`BasedOn` matches all three, `Where` excludes the open generic `InMemoryRepository<T>`. Result:

```
SqlCustomerRepository  → ICustomerRepository
SqlOrderRepository     → IOrderRepository
```