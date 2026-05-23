using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Represents a read-only service collection produced by the registration fluent API. This is the terminal type
///     in the registration chain, providing the resulting <see cref="ServiceDescriptor"/> registrations as an
///     <see cref="IServiceCollection"/> via <see cref="ToServiceCollection"/>.
/// </summary>
public interface IServiceSource
{
    /// <summary>
    ///     Returns a new <see cref="IServiceSource"/> with all descriptors set to the specified
    ///     <paramref name="lifetime"/>, using the supplied <paramref name="sharingMode"/> to control how a single
    ///     implementation registered against multiple service types shares its instance.
    /// </summary>
    /// <param name="lifetime">The target service lifetime.</param>
    /// <param name="sharingMode">
    ///     The sharing mode that determines whether an implementation registered against multiple service types
    ///     resolves to a single shared instance or to independent instances.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="lifetime"/> is <see cref="ServiceLifetime.Transient"/> and
    ///     <paramref name="sharingMode"/> is not <see cref="SharingMode.Independent"/>. Transient services can never
    ///     share an instance, so any other sharing mode would be silently ignored.
    /// </exception>
    IServiceCollection AsLifetime(ServiceLifetime lifetime, SharingMode sharingMode);

    /// <summary>
    ///     Collects all the services into the <paramref name="serviceCollection"/>.
    /// </summary>
    /// <returns>The resulting service collection.</returns>
    IServiceCollection ToServiceCollection(IServiceCollection serviceCollection);
}
