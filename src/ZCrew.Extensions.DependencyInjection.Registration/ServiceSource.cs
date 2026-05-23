using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

internal class ServiceSource : IServiceSource
{
    private readonly IEnumerable<ServiceComponent> components;

    internal ServiceSource(IEnumerable<ServiceComponent> components)
    {
        this.components = components;
    }

    /// <inheritdoc />
    public IServiceCollection AsLifetime(ServiceLifetime lifetime, SharingMode sharingMode)
    {
        if (lifetime == ServiceLifetime.Transient && sharingMode != SharingMode.Independent)
        {
            throw new ArgumentException(
                "Transient services can only be registered with SharingMode.Independent. "
                    + "Sharing only adds value for Singleton or Scoped services. "
                    + "This exception was thrown to immediately surface this mismatch instead of silently ignoring it"
            );
        }

        return ToServiceCollection(new ServiceCollection(), lifetime, sharingMode);
    }

    /// <inheritdoc />
    public IServiceCollection ToServiceCollection(IServiceCollection serviceCollection)
    {
        return ToServiceCollection(serviceCollection, ServiceLifetime.Singleton, SharingMode.SharedComponent);
    }

    private IServiceCollection ToServiceCollection(
        IServiceCollection serviceCollection,
        ServiceLifetime lifetime,
        SharingMode sharingMode
    )
    {
        foreach (var component in this.components)
        {
            var lifetimeComponent = component.WithLifetime(lifetime);

            foreach (var descriptor in lifetimeComponent.GetServiceDescriptors(sharingMode))
            {
                serviceCollection.Add(descriptor);
            }
        }
        return serviceCollection;
    }
}
