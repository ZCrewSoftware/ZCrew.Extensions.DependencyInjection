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

When scanning an assembly (via `FromAssembly`, `FromAssemblyContaining`, or `FromThisAssembly`), you get an `IAssemblyTypeSelector` that lets you control which types are included based on visibility. If you don't call any visibility method, the default behavior is to include **public types only**.

### `IncludePublicTypes()`

Includes only publicly exported types. This is the default — calling it explicitly is equivalent to not calling any visibility method:

```csharp
Classes.FromAssemblyContaining<OrderValidator>()
    .IncludePublicTypes()
```

Given:

```csharp
public class OrderValidator : IValidator<Order> { }
internal class InternalOrderValidator : IValidator<Order> { }
```

Selects:

```
OrderValidator
```

### `IncludeInternalTypes()`

Includes public types **and** top-level internal types:

```csharp
Classes.FromAssemblyContaining<OrderValidator>()
    .IncludeInternalTypes()
```

Selects:

```
OrderValidator, InternalOrderValidator
```

This is useful when your assembly exposes internals to the composition root (via `InternalsVisibleTo`) and you want those types registered too.

### `IncludeAllTypes()`

Includes all types regardless of visibility — public, internal, private nested, and so on:

```csharp
Classes.FromAssemblyContaining<OrderValidator>()
    .IncludeAllTypes()
```

Given:

```csharp
public class OrderValidator : IValidator<Order>
{
    public class Strict : IValidator<Order> { }  // nested public
}
internal class InternalOrderValidator : IValidator<Order> { }
```

Selects:

```
OrderValidator, OrderValidator.Strict, InternalOrderValidator
```

Including all types may include compiler-emitted types not meant to be directly referenced. It is not recommended to use this option without additional filtering.

## Skipping stages

Because `ITypeSelector` extends `ITypeFilter` (which extends `IServiceSelector`), you can skip directly from type selection to [filtering](6-type-filters.md) or [service selection](7-service-selectors.md). The intermediate stages apply sensible defaults — skipping the filter stage is equivalent to calling `AllTypes()`.

```csharp
// Full chain
Classes.FromAssemblyContaining<CustomerService>()
    .IncludePublicTypes()
    .AllTypes()
    .AsDefaultInterfaces()

// Skip visibility (defaults to public) and filtering (defaults to all types)
Classes.FromAssemblyContaining<CustomerService>()
    .AsDefaultInterfaces()
```

Both produce the same result.