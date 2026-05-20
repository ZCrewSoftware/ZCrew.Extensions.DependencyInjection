using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Represents a read-only service collection produced by the registration fluent API. This is the terminal type
///     in the registration chain, providing the resulting <see cref="ServiceDescriptor"/> registrations as an
///     <see cref="IServiceCollection"/> via <see cref="Collect"/>.
/// </summary>
public interface IServiceSource
{
    /// <summary>
    ///     Returns a new <see cref="IServiceSource"/> with all descriptors set to the specified
    ///     <paramref name="lifetime"/>. Instance-based descriptors that cannot change lifetime are kept unchanged.
    /// </summary>
    /// <param name="lifetime">The target service lifetime.</param>
    IServiceCollection AsLifetime(ServiceLifetime lifetime);

    /// <summary>
    ///     Returns a new <see cref="IServiceSource"/> with all descriptors set to the specified
    ///     <paramref name="lifetime"/>, using the supplied <paramref name="sharingMode"/> to control how a single
    ///     implementation registered against multiple service types shares its instance.
    /// </summary>
    /// <param name="lifetime">The target service lifetime.</param>
    /// <param name="sharingMode">
    ///     The sharing mode that determines whether an implementation registered against multiple service types
    ///     resolves to a single shared instance or to independent instances.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="lifetime"/> is <see cref="ServiceLifetime.Transient"/> and
    ///     <paramref name="sharingMode"/> is not <see cref="SharingMode.Independent"/>. Transient services can never
    ///     share an instance, so any other sharing mode would be silently ignored.
    /// </exception>
    IServiceCollection AsLifetime(ServiceLifetime lifetime, SharingMode sharingMode);

    /// <summary>
    ///     Registers all descriptors as <see cref="ServiceLifetime.Singleton"/> using
    ///     <see cref="SharingMode.SharedComponent"/>. One singleton instance is shared across every selected
    ///     service type.
    /// </summary>
    IServiceCollection AsSingleton()
    {
        return AsLifetime(ServiceLifetime.Singleton, SharingMode.SharedComponent);
    }

    /// <summary>
    ///     Registers all descriptors as <see cref="ServiceLifetime.Singleton"/> using
    ///     <see cref="SharingMode.Dependent"/>. Each service type is registered as a factory that resolves the
    ///     implementation, which must be registered elsewhere — either as one of the selected service types
    ///     (e.g. via a separate <c>AsSelf()</c> selection) or separately by the caller.
    /// </summary>
    IServiceCollection AsSingletonDependent()
    {
        return AsLifetime(ServiceLifetime.Singleton, SharingMode.Dependent);
    }

    /// <summary>
    ///     Registers all descriptors as <see cref="ServiceLifetime.Singleton"/> using
    ///     <see cref="SharingMode.Independent"/>. Each service type gets its own independent singleton instance.
    /// </summary>
    IServiceCollection AsSingletonIndependent()
    {
        return AsLifetime(ServiceLifetime.Singleton, SharingMode.Independent);
    }

    /// <summary>
    ///     Registers all descriptors as <see cref="ServiceLifetime.Scoped"/> using
    ///     <see cref="SharingMode.SharedComponent"/>. One per-scope instance is shared across every selected
    ///     service type.
    /// </summary>
    IServiceCollection AsScoped()
    {
        return AsLifetime(ServiceLifetime.Scoped, SharingMode.SharedComponent);
    }

    /// <summary>
    ///     Registers all descriptors as <see cref="ServiceLifetime.Scoped"/> using
    ///     <see cref="SharingMode.Dependent"/>. Each service type is registered as a factory that resolves the
    ///     implementation, which must be registered elsewhere — either as one of the selected service types
    ///     (e.g. via a separate <c>AsSelf()</c> selection) or separately by the caller.
    /// </summary>
    IServiceCollection AsScopedDependent()
    {
        return AsLifetime(ServiceLifetime.Scoped, SharingMode.Dependent);
    }

    /// <summary>
    ///     Registers all descriptors as <see cref="ServiceLifetime.Scoped"/> using
    ///     <see cref="SharingMode.Independent"/>. Each service type gets its own independent per-scope instance.
    /// </summary>
    IServiceCollection AsScopedIndependent()
    {
        return AsLifetime(ServiceLifetime.Scoped, SharingMode.Independent);
    }

    /// <summary>
    ///     Registers all descriptors as <see cref="ServiceLifetime.Transient"/>. A new instance is constructed on
    ///     every resolution, so sharing is not meaningful for transient services.
    /// </summary>
    IServiceCollection AsTransient()
    {
        return AsLifetime(ServiceLifetime.Transient, SharingMode.Independent);
    }

    /// <summary>
    ///     Collects all the services into a <see cref="IServiceCollection"/>.
    /// </summary>
    /// <returns>The resulting service collection.</returns>
    IServiceCollection Collect();
}
