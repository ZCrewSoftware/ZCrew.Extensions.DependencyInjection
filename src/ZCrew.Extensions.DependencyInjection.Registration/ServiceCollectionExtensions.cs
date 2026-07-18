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
    extension(IServiceCollection serviceCollection)
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
            return serviceCollection.Add((IEnumerable<ServiceDescriptor>)descriptors);
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
            return serviceCollection.Add((IEnumerable<ServiceDescriptor>)descriptors);
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
            return serviceCollection.Add(descriptors.ToServiceCollection());
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
            return serviceCollection.AddServices(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors represented by the <paramref name="service"/> into this service collection.
        /// </summary>
        /// <param name="service">The service to add.</param>
        /// <remarks>
        ///     A service with no lifetime set is registered as <see cref="ServiceLifetime.Singleton"/>.
        /// </remarks>
        public IServiceCollection Add(Service service)
        {
            service.AddServiceDescriptors(serviceCollection);
            return serviceCollection;
        }

        /// <summary>
        ///     Adds the descriptors represented by each of the <paramref name="services"/> into this service
        ///     collection.
        /// </summary>
        /// <param name="services">The services to add.</param>
        /// <remarks>
        ///     A service with no lifetime set is registered as <see cref="ServiceLifetime.Singleton"/>.
        /// </remarks>
        public IServiceCollection Add(params Service[] services)
        {
            foreach (var service in services)
            {
                service.AddServiceDescriptors(serviceCollection);
            }
            return serviceCollection;
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(ServiceSource descriptors)
        {
            return serviceCollection.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(ServiceLifetimeSelector descriptors)
        {
            return serviceCollection.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(ServiceKeySelector descriptors)
        {
            return serviceCollection.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(ServiceSelector descriptors)
        {
            return serviceCollection.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(TypeFilter descriptors)
        {
            return serviceCollection.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(AssemblyTypeSelector descriptors)
        {
            return serviceCollection.AddSingleton(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(ServiceSource descriptors)
        {
            return serviceCollection.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(ServiceLifetimeSelector descriptors)
        {
            return serviceCollection.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(ServiceKeySelector descriptors)
        {
            return serviceCollection.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(ServiceSelector descriptors)
        {
            return serviceCollection.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(TypeFilter descriptors)
        {
            return serviceCollection.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(AssemblyTypeSelector descriptors)
        {
            return serviceCollection.AddScoped(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(ServiceSource descriptors)
        {
            return serviceCollection.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(ServiceLifetimeSelector descriptors)
        {
            return serviceCollection.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(ServiceKeySelector descriptors)
        {
            return serviceCollection.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(ServiceSelector descriptors)
        {
            return serviceCollection.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(TypeFilter descriptors)
        {
            return serviceCollection.AddTransient(descriptors.ToServiceCollection());
        }

        /// <summary>Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.</summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(AssemblyTypeSelector descriptors)
        {
            return serviceCollection.AddTransient(descriptors.ToServiceCollection());
        }
    }
}
