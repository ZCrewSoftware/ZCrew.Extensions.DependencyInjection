using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public partial interface IKeyedServiceSelector
{
    IServiceCollection IServiceSource.AsLifetime(ServiceLifetime lifetime)
    {
        return Unkeyed().AsLifetime(lifetime);
    }

    IServiceCollection IServiceSource.Collect()
    {
        return Unkeyed().Collect();
    }
}
