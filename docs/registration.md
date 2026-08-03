# Convention-based registration

`ZCrew.Extensions.DependencyInjection.Registration` lets you register services by convention instead of one at a time. The API is modelled on [Castle Windsor's registration API](https://github.com/castleproject/Windsor/blob/master/docs/registering-services-by-conventions.md).

Instead of this:

```csharp
services.AddSingleton<ICustomerRepository, CustomerRepository>();
services.AddSingleton<IOrderRepository, OrderRepository>();
services.AddSingleton<IProductRepository, ProductRepository>();
// ...and so on, for every service you own
```

you describe the rule once:

```csharp
using ZCrew.Extensions.DependencyInjection.Registration;

services.AddSingleton(
    Classes.FromAssemblyContaining<CustomerRepository>()
        .BasedOn<IRepository>()
        .AsInterface()
);
```

That scans the assembly, picks every non-abstract class implementing something that derives from `IRepository`, and registers each one against its most derived interface. Write a new repository later and it gets picked up on its own.

## How it works

The chain has six stages:

1. **Entry point.** Where the types come from. `Classes` for non-abstract classes, `Types` for everything.
2. **Type selection.** Assembly visibility, if you are scanning (`IncludeInternalTypes`, `IncludeAllTypes`).
3. **Type filtering.** Which of those types you actually want (`Where`, `BasedOn`, `InNamespace`, `HasAttribute`).
4. **Service selection.** What each class registers as (`AsInterface`, `AsDefaultInterfaces`, `AsSelf`, and so on). Selectors chain, so `AsSelf().AsAllInterfaces()` gives you both.
5. **Keyed selection.** Service keys, via `Keyed`.
6. **Lifetime.** `AsSingleton`, `AsScoped`, `AsTransient`, or per type with `AsLifetime` / `AsLifetimeByAttribute`. Defaults to singleton.

Everything after the entry point is optional. Pass the chain to `services.AddSingleton`, `AddScoped` or `AddTransient` (there is an overload for every stage, so you can stop wherever you like), or call `.ToServiceCollection()` if you want the `IServiceCollection` yourself.

## Common patterns

### Register by naming convention

The one you will reach for most: register each class against the interface whose name matches it. `CustomerService` goes to `ICustomerService`, `OrderService` to `IOrderService`, and so on.

```csharp
services.AddScoped(
    Classes.FromAssemblyContaining<CustomerService>()
        .InSameNamespaceAs<CustomerService>(includeSubnamespaces: true)
        .AsDefaultInterfaces()
);
```

`AsDefaultInterfaces` matches a class to an interface when the interface name, minus the leading `I`, shows up in the class name. It works well for an application service layer where the naming is consistent.

### Register by base type

When your types share a base interface, filter with `BasedOn` and register with `AsInterface`:

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

`SqlCustomerRepository` registers as `ICustomerRepository` and `SqlOrderRepository` as `IOrderRepository`. `IRepository` itself is never used as the service type. `AsInterface` picks the most derived interface below the type you passed to `BasedOn`.

### Register by closed generic interface

For types implementing a generic interface, pass the open generic to `BasedOn` and use `AsBase`:

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

`OrderValidator` registers as `IValidator<Order>` and `CustomerValidator` as `IValidator<Customer>`. The open `IValidator<>` matches any closed form, and `AsBase` uses the closed form it resolved.

Add a `Where` if you need to leave some of them out:

```csharp
services.AddTransient(
    Classes.FromAssemblyContaining<OrderValidator>()
        .BasedOn(typeof(IValidator<>))
        .Where(type => !type.HasAttribute<ObsoleteAttribute>())
        .AsBase()
);
```

### Filter by attribute

`HasAttribute` narrows to types carrying a given attribute, which is handy when you want registration to be opt-in:

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

The attribute type can also be an interface that several attributes implement. Matching is by assignability, so this catches any of them and lets you filter on a property they share:

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

// Matches types carrying either attribute, filtered on the shared Region property:
services.AddSingleton(
    Classes.FromThisAssembly()
        .HasAttribute<IRegionAware>(a => a.Region == "customers")
        .AsSelf()
);
```

When a type can carry the same attribute more than once (`AllowMultiple = true`), use `HasAttributes` to look at the whole set at once:

```csharp
services.AddSingleton(
    Classes.FromThisAssembly()
        .HasAttributes<TagAttribute>(tags => tags.Any(t => t.Name == "public"))
        .AsSelf()
);
```

Both have a non-generic `Type` overload, and an optional `inherited` flag that decides whether attributes on base types count.

## `Classes` or `Types`?

Both offer the same factory methods (`From`, `FromAssembly`, `FromAssemblyContaining`, `FromThisAssembly`). The difference is what gets through:

- `Classes` only lets concrete, non-abstract classes through. This is what you want almost all of the time.
- `Types` lets everything through: interfaces, abstract classes, structs, enums, static classes. Useful when you need to find interface types or work with value types.

```csharp
// Only concrete classes
Classes.FromAssemblyContaining<Startup>()

// All types, including interfaces and structs
Types.FromAssemblyContaining<Startup>()
```
