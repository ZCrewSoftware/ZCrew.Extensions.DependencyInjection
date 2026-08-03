# Type filters

Type filters narrow the [selected types](type-selectors.md) before [service selection](service-selectors.md) runs. Every filter returns a new instance, so nothing you chain modifies what came before it.

## `AllTypes()`

Takes everything and moves on to service selection. Calling it is the same as skipping the filter stage:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .AllTypes()
    .AsDefaultInterfaces()

// Same thing:
Classes.FromAssemblyContaining<CustomerService>()
    .AsDefaultInterfaces()
```

## `Where(Func<Type, bool>)`

Filter with your own predicate. Chain as many as you want and each one narrows the set further:

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

`LegacyOrderProcessor` is dropped, leaving:

```
OrderService     → IOrderService
CustomerService  → ICustomerService
```

## `BasedOn<T>()` / `BasedOn(Type)` / `BasedOn(params Type[])`

Keeps types that implement or inherit from the base type you name. It also sets the base type context that `AsInterface()`, `AsInterfaces()`, `AsBase()` and the two-parameter `As` delegate read later on.

```csharp
Classes.FromAssemblyContaining<SqlCustomerRepository>()
    .BasedOn<IRepository<object>>()
```

That matches nothing. `BasedOn` matches non-generic types exactly, so for a generic base you want the open generic form:

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

`BasedOn(typeof(IRepository<>))` picks:

```
SqlCustomerRepository, SqlOrderRepository, InMemoryRepository<T>
```

`CustomerService` is left out because it doesn't implement `IRepository<>`.

Pass several base types and a type is kept if it matches any of them. `BasedOn` returns a `TypeFilter`, so it composes with `Where` and the rest.

## `NameEndsWith(string)` and its overloads

Keeps types whose name ends with a suffix. Generic arity is stripped first, so `Repository<T>` is matched as `Repository` and `IEnumerable<T>` ends with `"able"`.

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

You get:

```
CustomerService  → ICustomerService
OrderService     → IOrderService
```

`SqlCustomerRepository` ends with `Repository`, not `Service`, so it's out.

Other overloads take `ignoreCase` and a `CultureInfo`, or a `StringComparison`. `StringComparison.Ordinal` is usually the right call when scanning an assembly.

## `GenericTypes()` / `GenericTypeDefinitions()` / `ConstructedGenericTypes()`

Three filters for generic types:

| Method                      | Keeps                                                  | Matches                                                        |
|-----------------------------|--------------------------------------------------------|----------------------------------------------------------------|
| `GenericTypes()`            | Any generic type, open or closed                       | `Repository<T>`, `Repository<Customer>`, `Cache<TKey, TValue>` |
| `GenericTypeDefinitions()`  | Open generics only (`Type.IsGenericTypeDefinition`)    | `Repository<T>`, `Validator<>`                                 |
| `ConstructedGenericTypes()` | Closed generics only (`Type.IsConstructedGenericType`) | `Repository<Customer>`, `Validator<Order>`                     |

```csharp
// Only closed generic repositories. Open generics are handled elsewhere.
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

You get `SqlCustomerRepository` and `SqlOrderRepository`. `InMemoryRepository<T>` is open, so it's excluded.

> Open and closed generics behave differently once an implementation is forwarded as a shared service. See [Open generic limitation](shared-services.md#open-generic-limitation).

## `InNamespace(string)` / `InNamespace(string, bool)`

Keeps types in a namespace. The second parameter pulls in sub-namespaces too:

```csharp
// Exact namespace
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

You get:

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

That picks up `Application.Services`, `Application.Ports`, `Application.Caching` and `Application.Pipelines`.

## `InSameNamespaceAs(Type)` / `InSameNamespaceAs<T>()`

The same thing without the magic string. It takes the namespace from a type you name:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .InSameNamespaceAs<CustomerService>()
    .AsDefaultInterfaces()
```

Here that is the same as `InNamespace("Fixtures.SmallProject.Application.Services")`, but it survives a rename.

Sub-namespaces work the same way:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
    .InSameNamespaceAs<CustomerService>(includeSubnamespaces: true)
    .AsDefaultInterfaces()
```

## Combining filters

`BasedOn` and `Where` both return a `TypeFilter`, so they compose freely. `InNamespace` and `InSameNamespaceAs` return a `ServiceSelector` instead, so you can't chain `Where` or `BasedOn` after them. Put those first:

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

`BasedOn` matches all three and `Where` drops the open generic, leaving:

```
SqlCustomerRepository  → ICustomerRepository
SqlOrderRepository     → IOrderRepository
```
