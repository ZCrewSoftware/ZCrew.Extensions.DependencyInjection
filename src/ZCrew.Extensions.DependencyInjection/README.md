# ZCrew.Extensions.DependencyInjection

Decorator support for `Microsoft.Extensions.DependencyInjection`. Wrap a service that's already registered with another implementation, using an API that looks like the one you already use.

## What you get

- Decorators registered by type, or built by a factory
- A lifetime of your choosing (`AddSingletonDecorator`, `AddScopedDecorator`, `AddTransientDecorator`) or the one the wrapped service already has (`AddDecorator`)
- Keyed services (`AddKeyedDecorator`, `AddKeyedSingletonDecorator`, and so on)
- Mismatched lifetimes caught at registration, like a singleton decorator around a transient service
- As many decorators on one service as you want. Call `AddDecorator` again

## Installation

```bash
dotnet add package ZCrew.Extensions.DependencyInjection
```

or in your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="ZCrew.Extensions.DependencyInjection" Version="3.0.0" />
</ItemGroup>
```

This one is plain library code. There's no analyzer or source generator involved, so there's nothing else to configure.

## Quick start

Given a service and a decorator:

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

// Inherit the lifetime of the service being wrapped (scoped here)
services.AddDecorator<IGreeter, LoggingGreeter>();

// Or set one explicitly
services.AddSingletonDecorator<IGreeter, LoggingGreeter>();
```

Use a factory when you want to build it yourself:

```csharp
services.AddDecorator<IGreeter>((provider, inner) =>
    new LoggingGreeter(inner, provider.GetRequiredService<ILogger<LoggingGreeter>>()));
```

### Keyed services

```csharp
services.AddKeyedScoped<IGreeter, Greeter>("friendly");
services.AddKeyedDecorator<IGreeter, LoggingGreeter>("friendly");
```

## License

MIT. See [LICENSE.md](https://github.com/ZCrewSoftware/ZCrew.Extensions.DependencyInjection/blob/main/LICENSE.md).
