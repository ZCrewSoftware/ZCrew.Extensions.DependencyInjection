using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection;

/// <summary>
///     Extensions for the <see cref="ServiceLifetime" /> <see langword="enum" />.
/// </summary>
public static class ServiceTimelineExtensions
{
    /// <summary>
    ///     Compares the <see cref="ServiceLifetime" /> to another lifetime, <paramref name="other" />.
    /// </summary>
    /// <param name="lifetime">The <see cref="ServiceLifetime" />.</param>
    /// <param name="other">The <see cref="ServiceLifetime" /> to compare <see langword="this" /> lifetime to.</param>
    /// <returns>
    ///     <see langword="true" /> if <see langword="this" /> lifetime is the same lifetime or exceeds the lifetime of
    ///     <paramref name="other" />. <see langword="false" />, otherwise.
    /// </returns>
    public static bool Exceeds(this ServiceLifetime lifetime, ServiceLifetime other)
    {
        return (lifetime, other) switch
        {
            (ServiceLifetime.Singleton, ServiceLifetime.Singleton) => false,

            (ServiceLifetime.Scoped, ServiceLifetime.Singleton) => false,
            (ServiceLifetime.Scoped, ServiceLifetime.Scoped) => false,

            (ServiceLifetime.Transient, ServiceLifetime.Singleton) => false,
            (ServiceLifetime.Transient, ServiceLifetime.Scoped) => false,
            (ServiceLifetime.Transient, ServiceLifetime.Transient) => false,

            _ => true,
        };
    }
}
