using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Represents a read-only service collection produced by the registration fluent API. This is the terminal type
///     in the registration chain, providing the resulting <see cref="ServiceDescriptor"/> registrations as an
///     <see cref="IServiceCollection"/>.
/// </summary>
public interface IServiceSource : IServiceCollection
{
    /// <summary>
    ///     Returns a new <see cref="IServiceSource"/> with all descriptors set to the specified
    ///     <paramref name="lifetime"/>. Instance-based descriptors that cannot change lifetime are kept unchanged.
    /// </summary>
    /// <param name="lifetime">The target service lifetime.</param>
    IServiceSource AsLifetime(ServiceLifetime lifetime);
}
