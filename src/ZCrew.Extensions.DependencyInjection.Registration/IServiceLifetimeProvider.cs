using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Represents a provider for a <see cref="ServiceDescriptor.Lifetime"/>.
/// </summary>
/// <seealso cref="AsLifetimeAttribute"/>
public interface IServiceLifetimeProvider
{
    /// <summary>
    ///     The service lifetime to apply to the registration.
    /// </summary>
    ServiceLifetime Lifetime { get; }
}
