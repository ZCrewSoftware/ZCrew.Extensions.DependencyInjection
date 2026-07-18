# Convention-Based Registration

`ZCrew.Extensions.DependencyInjection.Registration` adds convention-based service registration to Microsoft's dependency injection container, inspired by [Castle Windsor's registration API](https://github.com/castleproject/Windsor/blob/master/docs/registering-services-by-conventions.md).

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

The API is a fluent chain with six stages:

1. **Entry point** — Choose where types come from (`Classes` for non-abstract classes, `Types` for everything)
2. **Type selection** — Optionally control assembly visibility (`IncludeInternalTypes`, `IncludeAllTypes`)
3. **Type filtering** — Narrow down which types to register (`Where`, `BasedOn`, `InNamespace`, `HasAttribute`)
4. **Service selection** — Decide what service type each implementation registers as (`AsInterface`, `AsDefaultInterfaces`, `AsSelf`, etc.); selectors can be chained (e.g. `AsSelf().AsAllInterfaces()`) to accumulate the distinct union of their service types
5. **Keyed service selection** — Optionally assign service keys via `Keyed`
6. **Lifetime selection** — Optionally choose a lifetime (`AsSingleton`, `AsScoped`, `AsTransient`, or per type via `AsLifetime` / `AsLifetimeByAttribute`); defaults to `Singleton`

Pass the chain to `services.AddSingleton`, `AddScoped`, or `AddTransient` — overloads exist for every stage of the chain — or call `.ToServiceCollection()` to produce an `IServiceCollection` directly.

## Quick patterns

### Register by interface convention

The most common pattern: register each class against the interface whose name matches by convention — `CustomerService` maps to `ICustomerService`, `OrderService` to `IOrderService`, and so on:

```csharp
services.AddScoped(
    Classes.FromAssemblyContaining<CustomerService>()
        .InSameNamespaceAs<CustomerService>(includeSubnamespaces: true)
        .AsDefaultInterfaces()
);
```

`AsDefaultInterfaces` matches each class to interfaces where the interface name (minus the `I` prefix) appears in the class name. This is useful for application service layers where the naming convention is consistent.

### Register by base type

When your types share a common base interface, use `BasedOn` to filter and `AsInterface` to register against the most-derived interface:

```csharp
services.AddSingleton(
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
services.AddTransient(
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
services.AddTransient(
    Classes.FromAssemblyContaining<OrderValidator>()
        .BasedOn(typeof(IValidator<>))
        .Where(type => !type.HasAttribute<ObsoleteAttribute>())
        .AsBase()
);
```

### Filter by attribute

`HasAttribute` narrows to types decorated with a given attribute — handy for opt-in registration:

```csharp
services.AddSingleton(
    Classes.FromThisAssembly()
        .HasAttribute<ServiceAttribute>()
        .AsInterface()
);
```

Pass a condition to filter on the attribute's data:

```csharp
services.AddSingleton(
    Classes.FromThisAssembly()
        .HasAttribute<CacheableAttribute>(attr => attr.Region == "customers")
        .AsSelf()
);
```

The attribute type may also be a **marker interface** that several attributes implement. Matching is by
assignability, so this catches any attribute assignable to the interface — and lets you filter on a property
the interface defines:

```csharp
public interface IRegionAware { string Region { get; } }

[AttributeUsage(AttributeTargets.Class)]
public class CacheableAttribute(string region) : Attribute, IRegionAware
{
    public string Region => region;
}

[AttributeUsage(AttributeTargets.Class)]
public class PartitionedAttribute(string region) : Attribute, IRegionAware
{
    public string Region => region;
}

// Matches types carrying *either* attribute, filtered by the shared Region property:
services.AddSingleton(
    Classes.FromThisAssembly()
        .HasAttribute<IRegionAware>(a => a.Region == "customers")
        .AsSelf()
);
```

When a type can carry **multiple** instances of an attribute (`AllowMultiple = true`), use `HasAttributes`
(plural) to evaluate the whole set at once:

```csharp
services.AddSingleton(
    Classes.FromThisAssembly()
        .HasAttributes<TagAttribute>(tags => tags.Any(t => t.Name == "public"))
        .AsSelf()
);
```

Both `HasAttribute` and `HasAttributes` also have a non-generic `Type` overload and an optional `inherited`
flag that controls whether attributes inherited from base types are considered.

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
