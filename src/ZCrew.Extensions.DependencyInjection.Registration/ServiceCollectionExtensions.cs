using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Extension methods on <see cref="IServiceCollection"/> for changing service lifetimes and bulk-adding, trying,
///     or replacing service descriptors using a <see cref="IServiceSource"/>.
/// </summary>
/// <remarks>
///     These exist mainly to prevent using the
///     <see cref="ServiceCollectionServiceExtensions.AddSingleton{TService}(IServiceCollection,TService)"/>
///     method by providing a better match.
/// </remarks>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(IServiceSource descriptors)
        {
            return services.AddSingleton((IEnumerable<ServiceDescriptor>)descriptors);
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(IKeyedServiceSelector descriptors)
        {
            return services.AddSingleton((IEnumerable<ServiceDescriptor>)descriptors);
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(IServiceSelector descriptors)
        {
            return services.AddSingleton((IEnumerable<ServiceDescriptor>)descriptors);
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(ITypeFilter descriptors)
        {
            return services.AddSingleton((IEnumerable<ServiceDescriptor>)descriptors);
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(ITypeSelector descriptors)
        {
            return services.AddSingleton((IEnumerable<ServiceDescriptor>)descriptors);
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(IAssemblyTypeSelector descriptors)
        {
            return services.AddSingleton((IEnumerable<ServiceDescriptor>)descriptors);
        }
    }
}
