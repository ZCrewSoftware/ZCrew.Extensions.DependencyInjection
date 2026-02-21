using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ZCrew.Extensions.DependencyInjection;

public static class ServiceCollectionEnumerableExtensions
{
    extension(IEnumerable<ServiceDescriptor> descriptors)
    {
        public IServiceCollection AsServiceCollection()
        {
            return new ServiceCollection { descriptors };
        }
    }
}
