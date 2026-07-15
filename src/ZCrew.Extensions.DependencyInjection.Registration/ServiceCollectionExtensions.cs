using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Extension methods on <see cref="IServiceCollection"/> for changing service lifetimes and bulk-adding, trying,
///     or replacing service descriptors produced by a registration chain (a <see cref="ServiceSource"/> or any of the
///     stages leading to it).
/// </summary>
/// <remarks>
///     These exist mainly to prevent using the
///     <see cref="ServiceCollectionServiceExtensions.AddSingleton{TService}(IServiceCollection,TService)"/>
///     method by providing a better match. There is one overload per concrete stage class so that a chain stopped at
///     any stage binds here instead of to the generic instance-registration overload.
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
        ///     Adds the descriptors produced by a registration chain, unchanged, into this service collection.
        /// </summary>
        /// <param name="descriptors">The registration chain to add.</param>
        /// <remarks>
        ///     Provides a better overload match than
        ///     <see cref="ServiceCollectionServiceExtensions.AddSingleton{TService}(IServiceCollection,TService)"/>
        ///     for a <see cref="ServiceSource"/>, so a chain terminated with a lifetime helper
        ///     (for example <c>chain.AsSingleton()</c> or <c>chain.AsLifetimeByAttribute()</c>) can be added directly.
        /// </remarks>
        public IServiceCollection Add(ServiceSource descriptors)
        {
            return services.Add(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors produced by a registration chain, unchanged, into this service collection.
        /// </summary>
        /// <param name="descriptors">The registration chain to add.</param>
        /// <remarks>
        ///     Provides a better overload match than
        ///     <see cref="ServiceCollectionServiceExtensions.AddSingleton{TService}(IServiceCollection,TService)"/>
        ///     for a <see cref="ServiceSource"/>, so a chain terminated with a lifetime helper
        ///     (for example <c>chain.AsSingleton()</c> or <c>chain.AsLifetimeByAttribute()</c>) can be added directly.
        /// </remarks>
        public IServiceCollection AddServices(ServiceSource descriptors)
        {
            return services.AddServices(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(ServiceSource descriptors)
        {
            return services.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(ServiceLifetimeSelector descriptors)
        {
            return services.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(ServiceKeySelector descriptors)
        {
            return services.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(ServiceSelector descriptors)
        {
            return services.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(TypeFilter descriptors)
        {
            return services.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(AssemblyTypeSelector descriptors)
        {
            return services.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(ServiceSource descriptors)
        {
            return services.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(ServiceLifetimeSelector descriptors)
        {
            return services.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(ServiceKeySelector descriptors)
        {
            return services.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(ServiceSelector descriptors)
        {
            return services.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(TypeFilter descriptors)
        {
            return services.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(AssemblyTypeSelector descriptors)
        {
            return services.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(ServiceSource descriptors)
        {
            return services.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(ServiceLifetimeSelector descriptors)
        {
            return services.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(ServiceKeySelector descriptors)
        {
            return services.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(ServiceSelector descriptors)
        {
            return services.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(TypeFilter descriptors)
        {
            return services.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(AssemblyTypeSelector descriptors)
        {
            return services.AddTransient(descriptors.ToServiceCollection());
        }
    }
}
