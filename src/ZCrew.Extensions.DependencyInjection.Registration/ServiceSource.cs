using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ZCrew.Extensions.DependencyInjection.Registration;

internal class ServiceSource : IServiceSource
{
    private readonly IEnumerable<ServiceComponent> components;

    internal ServiceSource(IEnumerable<ServiceComponent> components)
    {
        this.components = components;
    }

    /// <inheritdoc />
    public IServiceCollection AsLifetime(ServiceLifetime lifetime)
    {
        return Collect(lifetime);
    }

    /// <inheritdoc />
    public virtual IServiceCollection Collect()
    {
        return Collect(ServiceLifetime.Singleton);
    }

    private IServiceCollection Collect(ServiceLifetime lifetime)
    {
        var serviceCollection = new ServiceCollection();
        foreach (var component in this.components)
        {
            var lifetimeComponent = component.WithLifetime(lifetime);

            foreach (var descriptor in lifetimeComponent.GetServiceDescriptors())
            {
                serviceCollection.Add(descriptor);
            }
        }
        return serviceCollection;
    }
}
