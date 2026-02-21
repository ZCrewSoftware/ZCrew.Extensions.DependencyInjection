using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public class ServiceCollectionSource : ServiceCollection, IServiceSource
{
    public ServiceCollectionSource(IEnumerable<ServiceDescriptor> descriptors)
    {
        ICollection<ServiceDescriptor> collection = this;
        foreach (var descriptor in descriptors)
        {
            collection.Add(descriptor);
        }
    }
}
