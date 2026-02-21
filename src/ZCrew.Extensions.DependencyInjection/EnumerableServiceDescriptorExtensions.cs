using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ZCrew.Extensions.DependencyInjection;

/// <summary>
///     Extension methods on <see cref="IEnumerable{ServiceDescriptor}"/>.
/// </summary>
public static class EnumerableServiceDescriptorExtensions
{
    extension(IEnumerable<ServiceDescriptor> descriptors)
    {
        /// <summary>
        ///     Converts the descriptor sequence into a new <see cref="IServiceCollection"/>.
        /// </summary>
        public IServiceCollection AsServiceCollection()
        {
            return new ServiceCollection { descriptors };
        }
    }
}
