# Decorators

## Type-based decorators

The simplest way to register a decorator is by specifying the service interface and the decorator type. The decorator must accept the service interface as a constructor parameter so the container can inject the inner (delegate) service.

```csharp
services.AddSingleton<IEmailService, EmailService>();
services.AddSingletonDecorator<IEmailService, LoggingEmailService>();
```

Non-generic overloads are also available:

```csharp
services.AddSingletonDecorator(typeof(IEmailService), typeof(LoggingEmailService));
```

### Lifetime-specific methods

| Method                  | Decorator lifetime               |
|-------------------------|----------------------------------|
| `AddSingletonDecorator` | Singleton                        |
| `AddScopedDecorator`    | Scoped                           |
| `AddTransientDecorator` | Transient                        |
| `AddDecorator`          | Inherits the delegate's lifetime |

## Factory-based decorators

When you need to pass additional arguments to the decorator or perform custom construction, use a factory delegate.
The factory receives the `IServiceProvider` and the inner service:

```csharp
services.AddSingleton<IEmailService, EmailService>();
services.AddSingletonDecorator<IEmailService>((IServiceProvider sp, IEmailService next) =>
{
    var blockedDomain = "@contoso.com";
    return new FilteredEmailService(next, blockedDomain);
});
```

Non-generic overloads accept `Func<IServiceProvider, object, object>`:

```csharp
services.AddSingletonDecorator(typeof(IEmailService), (IServiceProvider sp, object next) =>
{
    return new FilteredEmailService((IEmailService)next, "@contoso.com");
});
```

## Keyed decorators

Keyed decorators target services registered with a specific service key.
They only decorate registrations that match both the service type and the key.

### Keyed type-based

```csharp
services.AddKeyedSingleton<IEmailService, EmailService>("notifications");
services.AddKeyedSingletonDecorator<IEmailService, LoggingEmailService>("notifications");
```

Non-generic:

```csharp
services.AddKeyedSingletonDecorator(typeof(IEmailService), typeof(LoggingEmailService), "notifications");
```

### Keyed factory-based

The factory receives the `IServiceProvider`, the inner service, and the service key:

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

Multiple decorators can be applied to the same service.
They are resolved in registration order, meaning the **last registered decorator is the outermost wrapper**:

```csharp
services.AddSingleton<IEmailService, EmailService>();
services.AddDecorator<IEmailService, FilteredEmailService>();
services.AddDecorator<IEmailService, LoggingEmailService>();
```

When `IEmailService` is resolved, the call chain is:

```
LoggingEmailService → FilteredEmailService → EmailService
```

## Lifetime validation

A decorator with a longer lifetime than its delegate creates a captive dependency — the decorator holds a stale reference to an instance that should have been recreated. The library throws `InvalidOperationException` at registration time when this happens. Use `AddDecorator` (without a lifetime prefix) to inherit the delegate's lifetime automatically.
