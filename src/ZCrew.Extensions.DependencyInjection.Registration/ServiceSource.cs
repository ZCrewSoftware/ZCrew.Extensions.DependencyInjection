using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Represents a read-only service collection produced by the registration fluent API. This is the terminal stage
///     in the registration chain, providing the resulting <see cref="ServiceDescriptor"/> registrations as an
///     <see cref="IServiceCollection"/> via <see cref="ToServiceCollection(IServiceCollection)"/>.
/// </summary>
public class ServiceSource
{
    private readonly IEnumerable<ServiceComponent> components;
    private readonly SharingMode? sharingMode;

    internal ServiceSource(IEnumerable<ServiceComponent> components, SharingMode? sharingMode)
    {
        this.components = components;
        this.sharingMode = sharingMode;
    }

    /// <summary>
    ///     Collects all the services into the <paramref name="serviceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add the descriptors to.</param>
    /// <returns>The resulting service collection.</returns>
    public IServiceCollection ToServiceCollection(IServiceCollection serviceCollection)
    {
        foreach (var component in this.components)
        {
            component.AddServiceDescriptors(serviceCollection, this.sharingMode);
        }
        return serviceCollection;
    }
}
