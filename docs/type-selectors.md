# Type selectors

Type selectors are the first stage of the chain. They decide where types come from, and which ones are in play based on their kind and visibility.

## `Classes` or `Types`?

Both offer the same factory methods. The difference is what gets through:

- `Classes` only lets concrete, non-abstract classes through.
- `Types` lets everything through: interfaces, abstract classes, structs, enums, static classes.

Given these types in an assembly:

```csharp
public interface IRepository<T> { }
public abstract class RepositoryBase<T> : IRepository<T> { }
public class SqlCustomerRepository : RepositoryBase<Customer>, ICustomerRepository { }
public struct Currency { }
public enum OrderStatus { Pending, Shipped, Delivered }
public static class PricingDefaults { }
```

`Classes` picks:

```
SqlCustomerRepository
```

`Types` picks:

```
IRepository<T>, RepositoryBase<T>, SqlCustomerRepository, Currency, OrderStatus, PricingDefaults
```

## Factory methods

### `From(IEnumerable<Type>)` / `From(params Type[])`

Start from a list of types you already have:

```csharp
var types = new[] { typeof(CustomerService), typeof(OrderService), typeof(ProductService) };

services.AddSingleton(
    Classes.From(types).AsDefaultInterfaces()
);
// Registers:
//   CustomerService  → ICustomerService
//   OrderService     → IOrderService
//   ProductService   → IProductService
```

### `FromAssembly(Assembly)`

Scan one assembly:

```csharp
Classes.FromAssembly(typeof(CustomerService).Assembly)
```

### `FromAssemblyContaining(Type)` / `FromAssemblyContaining<T>()`

Scan the assembly a given type lives in. Usually the easiest way to point at a specific project:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
// Scans the assembly containing CustomerService
```

### `FromThisAssembly()`

Scan the calling assembly:

```csharp
Classes.FromThisAssembly()
// Scans the assembly where this line of code lives
```

## Visibility

Scanning an assembly (`FromAssembly`, `FromAssemblyContaining`, `FromThisAssembly`) gives you an `AssemblyTypeSelector`, which has the visibility filters. You get public types by default.

Given:

```csharp
public class OrderValidator : IValidator<Order>
{
    public class Strict : IValidator<Order> { }  // nested
}
internal class InternalOrderValidator : IValidator<Order> { }
```

| Method | Picks |
|---|---|
| `IncludePublicTypes()` (default) | `OrderValidator` |
| `IncludeInternalTypes()` | `OrderValidator`, `InternalOrderValidator` |
| `IncludeAllTypes()` | `OrderValidator`, `OrderValidator.Strict`, `InternalOrderValidator` |

`IncludeAllTypes()` can also drag in compiler-generated types, so pair it with a filter.
