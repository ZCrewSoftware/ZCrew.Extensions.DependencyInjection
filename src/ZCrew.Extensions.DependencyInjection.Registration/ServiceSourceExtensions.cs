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
        ///     <see cref="ServiceLifetime.Transient"/>.
        /// </summary>
        public IServiceSource AsTransient()
        {
            return services.AsLifetime(ServiceLifetime.Transient);
        }
    }
}
