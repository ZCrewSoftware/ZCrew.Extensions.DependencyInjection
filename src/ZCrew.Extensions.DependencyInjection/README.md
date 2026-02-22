# ZCrew.Extensions.DependencyInjection

Decorator pattern support for `Microsoft.Extensions.DependencyInjection`. Wrap existing service registrations with decorator implementations using a simple, familiar API.

## Features

- **Type-based and factory-based** decorator registration
- **Lifetime-specific** methods (`AddSingletonDecorator`, `AddScopedDecorator`, `AddTransientDecorator`) or lifetime-inheriting (`AddDecorator`)
- **Keyed service** support (`AddKeyedDecorator`, `AddKeyedSingletonDecorator`, etc.)
- **Lifetime validation** — prevents invalid combinations (e.g., a singleton decorator wrapping a transient service)
- **Multiple decorators** — stack decorators by calling `AddDecorator` multiple times for the same service

## Installation

```bash
dotnet add package ZCrew.Extensions.DependencyInjection
```

## Quick Start

Given a service and decorator:

```csharp
public interface IGreeter
{
    string Greet(string name);
}

public class Greeter : IGreeter
{
    public string Greet(string name) => $"Hello, {name}!";
}

public class LoggingGreeter(IGreeter inner, ILogger<LoggingGreeter> logger) : IGreeter
{
    public string Greet(string name)
    {
        logger.LogInformation("Greeting {Name}", name);
        return inner.Greet(name);
    }
}
```

Register the decorator:

```csharp
services.AddScoped<IGreeter, Greeter>();

// Inherit the delegate's lifetime (scoped)
services.AddDecorator<IGreeter, LoggingGreeter>();

// Or specify an explicit lifetime
services.AddSingletonDecorator<IGreeter, LoggingGreeter>();
```

Use a factory for more control:

```csharp
services.AddDecorator<IGreeter>((provider, inner) =>
    new LoggingGreeter(inner, provider.GetRequiredService<ILogger<LoggingGreeter>>()));
```

### Keyed Services

```csharp
services.AddKeyedScoped<IGreeter, Greeter>("friendly");
services.AddKeyedDecorator<IGreeter, LoggingGreeter>("friendly");
```

## License

This project is licensed under the MIT License - see the [LICENSE.md](../../LICENSE.md) file for details.
