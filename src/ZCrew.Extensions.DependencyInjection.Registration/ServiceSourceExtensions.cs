using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Extensions for the <see cref="IServiceSource"/> type to extend existing functionality with convenient helpers.
/// </summary>
public static class ServiceSourceExtensions
{
    extension(IServiceSource source)
    {
        /// <summary>
        ///     Returns a new <see cref="IServiceSource"/> with all descriptors set to the specified
        ///     <paramref name="lifetime"/>. Instance-based descriptors that cannot change lifetime are kept unchanged.
        /// </summary>
        /// <param name="lifetime">The target service lifetime.</param>
        public IServiceCollection AsLifetime(ServiceLifetime lifetime)
        {
            // Transient types won't benefit from sharing an instance
            var defaultSharingMode =
                lifetime == ServiceLifetime.Transient ? SharingMode.Independent : SharingMode.SharedComponent;
            return source.AsLifetime(lifetime, defaultSharingMode);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Singleton"/> using
        ///     <see cref="SharingMode.SharedComponent"/>. One singleton instance is shared across every selected
        ///     service type.
        /// </summary>
        public IServiceCollection AsSingleton()
        {
            return source.AsLifetime(ServiceLifetime.Singleton, SharingMode.SharedComponent);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Singleton"/> using
        ///     <see cref="SharingMode.Dependent"/>. Each service type is registered as a factory that resolves the
        ///     implementation, which must be registered elsewhere — either as one of the selected service types
        ///     (e.g. via a separate <c>AsSelf()</c> selection) or separately by the caller.
        /// </summary>
        public IServiceCollection AsSingletonDependent()
        {
            return source.AsLifetime(ServiceLifetime.Singleton, SharingMode.Dependent);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Singleton"/> using
        ///     <see cref="SharingMode.Independent"/>. Each service type gets its own independent singleton instance.
        /// </summary>
        public IServiceCollection AsSingletonIndependent()
        {
            return source.AsLifetime(ServiceLifetime.Singleton, SharingMode.Independent);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Scoped"/> using
        ///     <see cref="SharingMode.SharedComponent"/>. One per-scope instance is shared across every selected
        ///     service type.
        /// </summary>
        public IServiceCollection AsScoped()
        {
            return source.AsLifetime(ServiceLifetime.Scoped, SharingMode.SharedComponent);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Scoped"/> using
        ///     <see cref="SharingMode.Dependent"/>. Each service type is registered as a factory that resolves the
        ///     implementation, which must be registered elsewhere — either as one of the selected service types
        ///     (e.g. via a separate <c>AsSelf()</c> selection) or separately by the caller.
        /// </summary>
        public IServiceCollection AsScopedDependent()
        {
            return source.AsLifetime(ServiceLifetime.Scoped, SharingMode.Dependent);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Scoped"/> using
        ///     <see cref="SharingMode.Independent"/>. Each service type gets its own independent per-scope instance.
        /// </summary>
        public IServiceCollection AsScopedIndependent()
        {
            return source.AsLifetime(ServiceLifetime.Scoped, SharingMode.Independent);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Transient"/>. A new instance is constructed on
        ///     every resolution, so sharing is not meaningful for transient services.
        /// </summary>
        public IServiceCollection AsTransient()
        {
            return source.AsLifetime(ServiceLifetime.Transient, SharingMode.Independent);
        }
    }
}
