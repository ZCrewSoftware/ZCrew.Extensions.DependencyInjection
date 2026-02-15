# Getting Started

## Installation

Install the NuGet package:

```bash
dotnet add package ZCrew.Extensions.DependencyInjection
```

## Quick example

Given a simple service and decorator:

```csharp
public interface IGreetingService
{
    string Greet(string name);
}

public class GreetingService : IGreetingService
{
    public string Greet(string name) => $"Hello, {name}!";
}

public class LoudGreetingService : IGreetingService
{
    private readonly IGreetingService next;

    public LoudGreetingService(IGreetingService next)
    {
        this.next = next;
    }

    public string Greet(string name) => this.next.Greet(name).ToUpperInvariant();
}
```

Register the service and decorator:

```csharp
using ZCrew.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<IGreetingService, GreetingService>();
services.AddSingletonDecorator<IGreetingService, LoudGreetingService>();

var provider = services.BuildServiceProvider();
var greeting = provider.GetRequiredService<IGreetingService>();
Console.WriteLine(greeting.Greet("World")); // HELLO, WORLD!
```

The decorator constructor receives the inner service via its `IGreetingService` parameter. The container wires this automatically.

## Inheriting the delegate lifetime

If you don't want to pin the decorator to a specific lifetime, use `AddDecorator`. The decorator will inherit the lifetime of each service it wraps:

```csharp
services.AddScoped<IGreetingService, GreetingService>();
services.AddDecorator<IGreetingService, LoudGreetingService>(); // also scoped
```