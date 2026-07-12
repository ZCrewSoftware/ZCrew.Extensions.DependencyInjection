using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Extensions for the <see cref="ServiceLifetimeSelector"/> type to extend existing functionality with convenient
///     helpers.
/// </summary>
public static class ServiceLifetimeSelectorExtensions
{
    extension(ServiceLifetimeSelector selector)
    {
        /// <summary>
        ///     Returns a new <see cref="IServiceCollection"/> with all descriptors set to the specified
        ///     <paramref name="lifetime"/>. Instance-based descriptors that cannot change lifetime are kept unchanged.
        /// </summary>
        /// <param name="lifetime">The target service lifetime.</param>
        public ServiceSource AsLifetime(ServiceLifetime lifetime)
        {
            // Transient types won't benefit from sharing an instance
            var defaultSharingMode =
                lifetime == ServiceLifetime.Transient ? SharingMode.Independent : SharingMode.SharedComponent;
            return selector.AsLifetime(lifetime, defaultSharingMode);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Singleton"/> using
        ///     <see cref="SharingMode.SharedComponent"/>. One singleton instance is shared across every selected
        ///     service type.
        /// </summary>
        public ServiceSource AsSingleton()
        {
            return selector.AsLifetime(ServiceLifetime.Singleton, SharingMode.SharedComponent);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Singleton"/> using
        ///     <see cref="SharingMode.Dependent"/>. Each service type is registered as a factory that resolves the
        ///     implementation, which must be registered elsewhere — either as one of the selected service types
        ///     (e.g. via a separate <c>AsSelf()</c> selection) or separately by the caller.
        /// </summary>
        public ServiceSource AsSingletonDependent()
        {
            return selector.AsLifetime(ServiceLifetime.Singleton, SharingMode.Dependent);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Singleton"/> using
        ///     <see cref="SharingMode.Independent"/>. Each service type gets its own independent singleton instance.
        /// </summary>
        public ServiceSource AsSingletonIndependent()
        {
            return selector.AsLifetime(ServiceLifetime.Singleton, SharingMode.Independent);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Scoped"/> using
        ///     <see cref="SharingMode.SharedComponent"/>. One per-scope instance is shared across every selected
        ///     service type.
        /// </summary>
        public ServiceSource AsScoped()
        {
            return selector.AsLifetime(ServiceLifetime.Scoped, SharingMode.SharedComponent);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Scoped"/> using
        ///     <see cref="SharingMode.Dependent"/>. Each service type is registered as a factory that resolves the
        ///     implementation, which must be registered elsewhere — either as one of the selected service types
        ///     (e.g. via a separate <c>AsSelf()</c> selection) or separately by the caller.
        /// </summary>
        public ServiceSource AsScopedDependent()
        {
            return selector.AsLifetime(ServiceLifetime.Scoped, SharingMode.Dependent);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Scoped"/> using
        ///     <see cref="SharingMode.Independent"/>. Each service type gets its own independent per-scope instance.
        /// </summary>
        public ServiceSource AsScopedIndependent()
        {
            return selector.AsLifetime(ServiceLifetime.Scoped, SharingMode.Independent);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Transient"/>. A new instance is constructed on
        ///     every resolution, so sharing is not meaningful for transient services.
        /// </summary>
        public ServiceSource AsTransient()
        {
            return selector.AsLifetime(ServiceLifetime.Transient, SharingMode.Independent);
        }
    }
}
