using Microsoft.Extensions.DependencyInjection;
using ZCrew.Extensions.DependencyInjection.Registration;

namespace RegistrationSample.Services;

/// <summary>
///     Builds a greeting for a name.
/// </summary>
public interface IGreetingService
{
    /// <summary>Returns a greeting addressed to <paramref name="name"/>.</summary>
    /// <param name="name">The name to greet.</param>
    string Greet(string name);
}

/// <summary>
///     Registered against itself and <see cref="IGreetingService"/> as a single shared, scoped instance. Because
///     the lifetime is <see cref="ServiceLifetime.Scoped"/> with more than one service type, the interface forwards
///     to the concrete registration (shown as a factory in the printed output).
/// </summary>
[Service, Scoped, As<IGreetingService>]
public sealed class GreetingService : IGreetingService
{
    /// <inheritdoc />
    public string Greet(string name) => $"Hello, {name}!";
}
