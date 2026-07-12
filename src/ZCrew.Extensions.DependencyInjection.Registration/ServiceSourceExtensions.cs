using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Extensions for the <see cref="ServiceSource"/> type to extend existing functionality with convenient helpers.
/// </summary>
public static class ServiceSourceExtensions
{
    extension(ServiceSource source)
    {
        /// <summary>
        ///     Collects all the services into a <see cref="IServiceCollection"/>.
        /// </summary>
        /// <returns>The resulting service collection.</returns>
        public IServiceCollection ToServiceCollection()
        {
            return source.ToServiceCollection(new ServiceCollection());
        }
    }
}
