# Convention-Based Registration

`ZCrew.Extensions.DependencyInjection.Registration` adds convention-based service registration to Microsoft's dependency injection container, inspired by [Castle Windsor's registration API](https://github.com/castleproject/Windsor/blob/master/docs/registering-components-by-conventions.md).

Instead of registering each service one-by-one:

```csharp
services.AddSingleton<ICustomerRepository, CustomerRepository>();
services.AddSingleton<IOrderRepository, OrderRepository>();
services.AddSingleton<IProductRepository, ProductRepository>();
// ... and so on for every service
```

You describe which types to register and how to register them:

```csharp
using ZCrew.Extensions.DependencyInjection.Registration;

services.AddSingleton(
    Classes.FromAssemblyContaining<CustomerRepository>()
        .BasedOn<IRepository>()
        .AsInterface()
);
```

This scans the assembly, finds every non-abstract class that implements a descendant of `IRepository`, and registers each one against its most-derived interface. New repository implementations are picked up automatically — no manual registration needed.

## How it works

The API is a fluent chain with four stages:

1. **Entry point** — Choose where types come from (`Classes` for non-abstract classes, `Types` for everything)
2. **Type selection** — Optionally control assembly visibility (`IncludeInternalTypes`, `IncludeAllTypes`)
3. **Type filtering** — Narrow down which types to register (`Where`, `BasedOn`, `InNamespace`)
4. **Service selection** — Decide what service type each implementation registers as (`AsInterface`, `AsDefaultInterfaces`, `AsSelf`, etc.)

The result is an `IServiceCollection` that you pass to `services.Add()`.

## Quick patterns

### Register by interface convention

The most common pattern: register each class against the interface whose name matches by convention — `CustomerService` maps to `ICustomerService`, `OrderService` to `IOrderService`, and so on:

```csharp
services.Add(
    Classes.FromAssemblyContaining<CustomerService>()
        .InSameNamespaceAs<CustomerService>(includeSubnamespaces: true)
        .AsDefaultInterfaces()
);
```

`AsDefaultInterfaces` matches each class to interfaces where the interface name (minus the `I` prefix) appears in the class name. This is useful for application service layers where the naming convention is consistent.

### Register by base type

When your types share a common base interface, use `BasedOn` to filter and `AsInterface` to register against the most-derived interface:

```csharp
services.Add(
    Classes.FromAssemblyContaining<SqlCustomerRepository>()
        .BasedOn<IRepository>()
        .AsInterface()
);
```

Given this hierarchy:

```
IRepository
├── ICustomerRepository
│   └── SqlCustomerRepository
└── IOrderRepository
    └── SqlOrderRepository
```

`SqlCustomerRepository` registers as `ICustomerRepository`, and `SqlOrderRepository` as `IOrderRepository`. The base `IRepository` interface is not used as the service type — `AsInterface` picks the most-derived (top-level) interface that descends from the `BasedOn` type.

### Register by closed generic interface

When your types implement a generic interface, use `BasedOn` with the open generic type and `AsBase` to register each implementation against its closed generic form:

```csharp
services.Add(
    Classes.FromAssemblyContaining<OrderValidator>()
        .BasedOn(typeof(IValidator<>))
        .AsBase()
);
```

Given:

```csharp
public interface IValidator<T> { }
public class OrderValidator : IValidator<Order> { }
public class CustomerValidator : IValidator<Customer> { }
```

`OrderValidator` registers as `IValidator<Order>` and `CustomerValidator` as `IValidator<Customer>`. The open generic `IValidator<>` in `BasedOn` matches any closed form, and `AsBase` uses the resolved closed generic as the service type.

You can combine this with `Where` to control which implementations are included:

```csharp
services.Add(
    Classes.FromAssemblyContaining<OrderValidator>()
        .BasedOn(typeof(IValidator<>))
        .Where(type => !type.HasAttribute<ObsoleteAttribute>())
        .AsBase()
);
```

## Entry points: `Classes` vs `Types`

Both `Classes` and `Types` offer the same set of factory methods (`From`, `FromAssembly`, `FromAssemblyContaining`, `FromThisAssembly`). The difference is what passes through:

- **`Classes`** filters to concrete, non-abstract classes — the typical choice for service registration.
- **`Types`** includes everything: interfaces, abstract classes, structs, enums, static classes. Useful when you need to discover interface types or work with value types.

```csharp
// Only concrete classes
Classes.FromAssemblyContaining<Startup>()

// All types including interfaces and structs
Types.FromAssemblyContaining<Startup>()
```
