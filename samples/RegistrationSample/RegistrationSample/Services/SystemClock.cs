using ZCrew.Extensions.DependencyInjection.Registration;

namespace RegistrationSample.Services;

/// <summary>
///     A plain service with no separate contract. <c>[Service]</c> with no arguments registers the concrete type
///     against itself as a singleton.
/// </summary>
[Service]
public sealed class SystemClock
{
    /// <summary>
    ///     The current UTC time.
    /// </summary>
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
