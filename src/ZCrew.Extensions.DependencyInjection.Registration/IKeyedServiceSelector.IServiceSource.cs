using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public partial interface IKeyedServiceSelector
{
    /// <inheritdoc />
    IServiceCollection IServiceSource.AsLifetime(ServiceLifetime lifetime, SharingMode sharingMode)
    {
        return Unkeyed().AsLifetime(lifetime, sharingMode);
    }

    /// <inheritdoc />
    IServiceCollection IServiceSource.ToServiceCollection(IServiceCollection serviceCollection)
    {
        return Unkeyed().ToServiceCollection(serviceCollection);
    }
}
