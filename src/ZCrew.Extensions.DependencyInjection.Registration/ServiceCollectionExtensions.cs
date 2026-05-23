using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        ///     Adds the descriptors, unchanged, into this service collection.
        /// </summary>
        /// <param name="descriptors">The descriptors to add.</param>
        /// <remarks>
        ///     This is to avoid accidentally using <see cref="ICollection{T}.Add"/> on the
        ///     <see cref="IServiceCollection"/> and getting an unclear error.
        /// </remarks>
        public IServiceCollection Add(IServiceCollection descriptors)
        {
            return services.Add((IEnumerable<ServiceDescriptor>)descriptors);
        }

        /// <summary>
        ///     Adds the descriptors, unchanged, into this service collection.
        /// </summary>
        /// <param name="descriptors">The descriptors to add.</param>
        /// <remarks>
        ///     This is to avoid accidentally using <see cref="ICollection{T}.Add"/> on the
        ///     <see cref="IServiceCollection"/> and getting an unclear error.
        /// </remarks>
        public IServiceCollection AddServices(IServiceCollection descriptors)
        {
            return services.Add((IEnumerable<ServiceDescriptor>)descriptors);
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(IServiceSource descriptors)
        {
            return services.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(IKeyedServiceSelector descriptors)
        {
            return services.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(IServiceSelector descriptors)
        {
            return services.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(ITypeFilter descriptors)
        {
            return services.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(ITypeSelector descriptors)
        {
            return services.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(IAssemblyTypeSelector descriptors)
        {
            return services.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(IServiceSource descriptors)
        {
            return services.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(IKeyedServiceSelector descriptors)
        {
            return services.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(IServiceSelector descriptors)
        {
            return services.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(ITypeFilter descriptors)
        {
            return services.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(ITypeSelector descriptors)
        {
            return services.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(IAssemblyTypeSelector descriptors)
        {
            return services.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(IServiceSource descriptors)
        {
            return services.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(IKeyedServiceSelector descriptors)
        {
            return services.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(IServiceSelector descriptors)
        {
            return services.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(ITypeFilter descriptors)
        {
            return services.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(ITypeSelector descriptors)
        {
            return services.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(IAssemblyTypeSelector descriptors)
        {
            return services.AddTransient(descriptors.ToServiceCollection());
        }
    }
}
