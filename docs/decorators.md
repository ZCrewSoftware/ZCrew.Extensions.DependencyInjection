# Decorators

## Type-based decorators

The easiest way to register a decorator is to name the service interface and the decorator type. Give the decorator a constructor parameter of the service interface and the container passes in the service being wrapped:

```csharp
services.AddSingleton<IEmailService, EmailService>();
services.AddSingletonDecorator<IEmailService, LoggingEmailService>();
```

There are non-generic overloads for when you only have the types at runtime:

```csharp
services.AddSingletonDecorator(typeof(IEmailService), typeof(LoggingEmailService));
```

### Picking a lifetime

| Method                  | Decorator lifetime                |
|-------------------------|-----------------------------------|
| `AddSingletonDecorator` | Singleton                         |
| `AddScopedDecorator`    | Scoped                            |
| `AddTransientDecorator` | Transient                         |
| `AddDecorator`          | Same as the service it wraps      |

## Factory-based decorators

Use a factory when the decorator needs something the container can't hand it, or when you want to construct it yourself. The factory gets the `IServiceProvider` and the service being wrapped:

```csharp
services.AddSingleton<IEmailService, EmailService>();
services.AddSingletonDecorator<IEmailService>((IServiceProvider sp, IEmailService next) =>
{
    var blockedDomain = "@contoso.com";
    return new FilteredEmailService(next, blockedDomain);
});
```

The non-generic overloads take a `Func<IServiceProvider, object, object>`:

```csharp
services.AddSingletonDecorator(typeof(IEmailService), (IServiceProvider sp, object next) =>
{
    return new FilteredEmailService((IEmailService)next, "@contoso.com");
});
```

## Keyed decorators

A keyed decorator only wraps registrations that match both the service type and the key.

### By type

```csharp
services.AddKeyedSingleton<IEmailService, EmailService>("notifications");
services.AddKeyedSingletonDecorator<IEmailService, LoggingEmailService>("notifications");
```

Non-generic:

```csharp
services.AddKeyedSingletonDecorator(typeof(IEmailService), typeof(LoggingEmailService), "notifications");
```

### By factory

The factory gets the `IServiceProvider`, the service being wrapped, and the service key:

```csharp
services.AddKeyedSingleton<IEmailService, EmailService>("notifications");
services.AddKeyedSingletonDecorator<IEmailService>(
    "notifications",
    (IServiceProvider sp, IEmailService next, object? serviceKey) =>
    {
        return new FilteredEmailService(next, "@contoso.com");
    });
```

Non-generic:

```csharp
services.AddKeyedSingletonDecorator(
    typeof(IEmailService),
    "notifications",
    (IServiceProvider sp, object next, object? serviceKey) =>
    {
        return new FilteredEmailService((IEmailService)next, "@contoso.com");
    });
```

## Stacking decorators

You can put as many decorators as you like on one service. They are applied in registration order, so the last one you register ends up on the outside:

```csharp
services.AddSingleton<IEmailService, EmailService>();
services.AddDecorator<IEmailService, FilteredEmailService>();
services.AddDecorator<IEmailService, LoggingEmailService>();
```

Resolving `IEmailService` gives you this call chain:

```
LoggingEmailService → FilteredEmailService → EmailService
```

## Lifetime validation

A decorator that outlives the service it wraps is a captive dependency: it holds on to an instance that should have been thrown away and rebuilt. Rather than let that slip through, the library throws an `InvalidOperationException` when you register it.

If you don't want to think about it, use `AddDecorator` and the decorator takes whatever lifetime the service it wraps has.
