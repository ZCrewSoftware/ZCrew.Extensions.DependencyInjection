# Type Filters

Type filters narrow down the set of [selected types](type-selectors.md) before [service selection](service-selectors.md). Each filter method returns a new instance, so filters can be chained without mutating previous state.

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

Chained `Where` calls further restrict the set — each predicate must pass.

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

`BasedOn(params Type[])` accepts multiple base types — a type is included if it matches **any** of them. `BasedOn` returns `ITypeFilter`, so it composes with `Where` and other filters.

## `NameEndsWith(string)` and overloads

Filters to types whose name ends with the given suffix. Generic arity is stripped before matching, so `Repository<T>` is treated as `Repository` and `IEnumerable<T>` ends with `"able"`.

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .NameEndsWith("Service")
    .AsInterface()
```

Given:

```csharp
public class CustomerService : ICustomerService { }
public class OrderService : IOrderService { }
public class SqlCustomerRepository : ICustomerRepository { }
```

Selects:

```
CustomerService  → ICustomerService
OrderService     → IOrderService
```

`SqlCustomerRepository` is excluded — its name ends with `Repository`, not `Service`.

Overloads accept `ignoreCase`/`CultureInfo` or a `StringComparison` for explicit control — typically `StringComparison.Ordinal` for assembly scanning.

## `GenericTypes()` / `GenericTypeDefinitions()` / `ConstructedGenericTypes()`

Filters for generic types. The three methods are mutually exclusive in the way most callers want:

| Method                      | Selects                                                | Example match                                                  |
|-----------------------------|--------------------------------------------------------|----------------------------------------------------------------|
| `GenericTypes()`            | Any generic type — both open and closed forms          | `Repository<T>`, `Repository<Customer>`, `Cache<TKey, TValue>` |
| `GenericTypeDefinitions()`  | Open generics only (`Type.IsGenericTypeDefinition`)    | `Repository<T>`, `Validator<>`                                 |
| `ConstructedGenericTypes()` | Closed generics only (`Type.IsConstructedGenericType`) | `Repository<Customer>`, `Validator<Order>`                     |

```csharp
// Register only closed generic repositories — open generics handled separately
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .BasedOn(typeof(IRepository<>))
    .ConstructedGenericTypes()
    .AsBase()
```

Given:

```csharp
public class SqlCustomerRepository : RepositoryBase<Customer>, IRepository<Customer> { }
public class SqlOrderRepository : RepositoryBase<Order>, IRepository<Order> { }
public class InMemoryRepository<T> : IRepository<T> { }
```

Selects `SqlCustomerRepository` and `SqlOrderRepository` (both closed). `InMemoryRepository<T>` is excluded because it is an open generic.

> Open and closed generic registrations behave differently under shared-component forwarding. See [Open generic limitation](shared-components.md#open-generic-limitation).

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
