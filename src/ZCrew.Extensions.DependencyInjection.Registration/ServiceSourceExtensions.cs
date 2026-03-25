using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Extension methods on <see cref="IServiceSource"/> for changing service lifetimes.
/// </summary>
/// <example>
///     <code>
///     var services = Classes
///         .FromThisAssembly()
///         .Where(t => t.Name.EndsWith("Service"))
///         .AsInterface()
///         .AsSingleton();
///     </code>
/// </example>
public static class ServiceSourceExtensions
{
    extension(IServiceSource services)
    {
        /// <summary>
        ///     Returns a new <see cref="IServiceSource"/> with all descriptors set to
        ///     <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public IServiceSource AsSingleton()
        {
            return services.AsLifetime(ServiceLifetime.Singleton);
        }

        /// <summary>
        ///     Returns a new <see cref="IServiceSource"/> with all descriptors set to
        ///     <see cref="ServiceLifetime.Scoped"/>.
        /// </summary>
        public IServiceSource AsScoped()
        {
            return services.AsLifetime(ServiceLifetime.Scoped);
        }

        /// <summary>
        ///     Returns a new <see cref="IServiceSource"/> with all descriptors set to
        ///     <see cref="ServiceLifetime.Scoped"/>.
        /// </summary>
        /// <param name="ignoreSingletonImplementations">
        ///     When <see langword="true"/>, instance-based descriptors that can only be singletons are kept unchanged
        ///     instead of throwing.
        /// </param>
        public IServiceSource AsScoped(bool ignoreSingletonImplementations)
        {
            return services.AsLifetime(ServiceLifetime.Scoped, ignoreSingletonImplementations);
        }

        /// <summary>
        ///     Returns a new <see cref="IServiceSource"/> with all descriptors set to
        ///     <see cref="ServiceLifetime.Transient"/>.
        /// </summary>
        public IServiceSource AsTransient()
        {
            return services.AsLifetime(ServiceLifetime.Transient);
        }

        /// <summary>
        ///     Returns a new <see cref="IServiceSource"/> with all descriptors set to
        ///     <see cref="ServiceLifetime.Transient"/>.
        /// </summary>
        /// <param name="ignoreSingletonImplementations">
        ///     When <see langword="true"/>, instance-based descriptors that can only be singletons are kept unchanged
        ///     instead of throwing.
        /// </param>
        public IServiceSource AsTransient(bool ignoreSingletonImplementations)
        {
            return services.AsLifetime(ServiceLifetime.Transient, ignoreSingletonImplementations);
        }

        /// <summary>
        ///     Returns a new <see cref="IServiceSource"/> with all descriptors set to the specified
        ///     <paramref name="lifetime"/>. Instance-based descriptors that cannot change lifetime are kept unchanged.
        /// </summary>
        /// <param name="lifetime">The target service lifetime.</param>
        public IServiceSource AsLifetime(ServiceLifetime lifetime)
        {
            return services.AsLifetime(lifetime, ignoreSingletonImplementations: true);
        }

        /// <summary>
        ///     Returns a new <see cref="IServiceSource"/> with all descriptors set to the specified
        ///     <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="lifetime">The target service lifetime.</param>
        /// <param name="ignoreSingletonImplementations">
        ///     When <see langword="true"/>, instance-based descriptors that can only be singletons are kept unchanged
        ///     instead of throwing.
        /// </param>
        public IServiceSource AsLifetime(ServiceLifetime lifetime, bool ignoreSingletonImplementations)
        {
            var modifiedServices = new ServiceDescriptor[services.Count];
            for (var i = 0; i < services.Count; i++)
            {
                modifiedServices[i] = services[i].WithLifetime(lifetime, ignoreSingletonImplementations);
            }
            return new ServiceCollectionSource(modifiedServices);
        }
    }
}
