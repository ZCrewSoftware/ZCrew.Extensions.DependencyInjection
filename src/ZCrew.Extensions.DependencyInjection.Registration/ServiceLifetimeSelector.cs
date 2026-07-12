using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Assigns the <see cref="ServiceLifetime"/> and <see cref="SharingMode"/> to registrations produced by the service
///     key selection stage. This is an optional stage between <see cref="ServiceKeySelector"/> and
///     <see cref="ServiceSource"/> in the registration fluent API. When the stage is skipped, the registrations use
///     <see cref="ServiceLifetime.Singleton"/> and with the <see cref="SharingMode.SharedComponent"/>.
/// </summary>
public class ServiceLifetimeSelector : ServiceSource
{
    private readonly IEnumerable<ServiceComponent> components;

    // Single walk per terminal is verified by MultiEnumerationTests.
    // ReSharper disable PossibleMultipleEnumeration
    internal ServiceLifetimeSelector(IEnumerable<ServiceComponent> components)
        : base(components, SharingMode.SharedComponent)
    {
        this.components = components;
    }
    // ReSharper restore PossibleMultipleEnumeration

    /// <summary>
    ///     Returns a new <see cref="ServiceSource"/> with all descriptors set to the specified
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
    public ServiceSource AsLifetime(ServiceLifetime lifetime, SharingMode sharingMode)
    {
        if (lifetime == ServiceLifetime.Transient && sharingMode != SharingMode.Independent)
        {
            throw new ArgumentException(
                "Transient services can only be registered with SharingMode.Independent. "
                + "Sharing only adds value for Singleton or Scoped services. "
                + "This exception was thrown to immediately surface this mismatch instead of silently ignoring it"
            );
        }

        return new ServiceSource(this.components.Select(component => component.WithLifetime(lifetime)), sharingMode);
    }
}
