# Type Selectors

Type selectors are the first stage of the registration chain. They determine **where types come from** and which types are included based on their kind and visibility.

## Entry points: `Classes` vs `Types`

Both `Classes` and `Types` offer the same factory methods. The difference is what passes through:

- **`Classes`** — only concrete, non-abstract classes.
- **`Types`** — everything: interfaces, abstract classes, structs, enums, static classes.

Given these types in an assembly:

```csharp
public interface IRepository<T> { }
public abstract class RepositoryBase<T> : IRepository<T> { }
public class SqlCustomerRepository : RepositoryBase<Customer>, ICustomerRepository { }
public struct Currency { }
public enum OrderStatus { Pending, Shipped, Delivered }
public static class PricingDefaults { }
```

`Classes` selects:

```
SqlCustomerRepository
```

`Types` selects:

```
IRepository<T>, RepositoryBase<T>, SqlCustomerRepository, Currency, OrderStatus, PricingDefaults
```

## Factory methods

### `From(IEnumerable<Type>)` / `From(params Type[])`

Begins registration from an explicit set of types:

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

Scans the specified assembly:

```csharp
Classes.FromAssembly(typeof(CustomerService).Assembly)
```

### `FromAssemblyContaining(Type)` / `FromAssemblyContaining<T>()`

Scans the assembly that contains the given type — typically the most convenient way to target a specific project:

```csharp
Classes.FromAssemblyContaining<CustomerService>()
// Scans the assembly containing CustomerService
```

### `FromThisAssembly()`

Scans the calling assembly:

```csharp
Classes.FromThisAssembly()
// Scans the assembly where this line of code lives
```

## Assembly type visibility

When scanning an assembly (via `FromAssembly`, `FromAssemblyContaining`, or `FromThisAssembly`), the returned `IAssemblyTypeSelector` exposes visibility filters. Default is **public only**.

Given:

```csharp
public class OrderValidator : IValidator<Order>
{
    public class Strict : IValidator<Order> { }  // nested
}
internal class InternalOrderValidator : IValidator<Order> { }
```

| Method | Selects |
|---|---|
| `IncludePublicTypes()` (default) | `OrderValidator` |
| `IncludeInternalTypes()` | `OrderValidator`, `InternalOrderValidator` |
| `IncludeAllTypes()` | `OrderValidator`, `OrderValidator.Strict`, `InternalOrderValidator` |

`IncludeAllTypes()` may also surface compiler-emitted types — pair it with `Where` or another filter.