using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Assigns the <see cref="ServiceLifetime"/> to registrations produced by the service key selection stage. This is
///     an optional stage between <see cref="ServiceKeySelector"/> and <see cref="ServiceSource"/> in the registration
///     fluent API. When the stage is skipped, the registrations use <see cref="ServiceLifetime.Singleton"/>.
/// </summary>
public class ServiceLifetimeSelector : ServiceSource
{
    private readonly IEnumerable<Service> components;

    // Single walk per terminal is verified by MultiEnumerationTests.
    // ReSharper disable PossibleMultipleEnumeration
    internal ServiceLifetimeSelector(IEnumerable<Service> components)
        : base(components.Select(component => component.AsLifetime(ServiceLifetime.Singleton)))
    {
        this.components = components;
    }
    // ReSharper restore PossibleMultipleEnumeration

    /// <summary>
    ///     Returns a new <see cref="ServiceSource"/> with all descriptors set to the specified
    ///     <paramref name="lifetime"/>. Each service registered for an implementation uses the same
    ///     <see cref="ServiceLifetime"/>.
    /// </summary>
    /// <param name="lifetime">The target service lifetime.</param>
    public ServiceSource AsLifetime(ServiceLifetime lifetime)
    {
        return new ServiceSource(this.components.Select(component => component.AsLifetime(lifetime)));
    }

    /// <summary>
    ///     Returns a new <see cref="ServiceSource"/> whose descriptors take the <see cref="ServiceLifetime"/> produced
    ///     by <paramref name="lifetimeSelector"/> for each implementation type. Each service registered for an
    ///     implementation uses the same <see cref="ServiceLifetime"/>.
    /// </summary>
    /// <param name="lifetimeSelector">
    ///     A function that receives the implementation type and returns the service lifetime.
    /// </param>
    /// <example>
    ///     <code>
    ///     Classes.From(typeof(CustomerService), typeof(OrderService))
    ///         .AsInterface()
    ///         .AsLifetime(type => type == typeof(OrderService) ? ServiceLifetime.Scoped : ServiceLifetime.Singleton)
    ///     </code>
    /// </example>
    public ServiceSource AsLifetime(Func<Type, ServiceLifetime> lifetimeSelector)
    {
        ArgumentNullException.ThrowIfNull(lifetimeSelector);
        return new ServiceSource(this.components.Select(component => component.AsLifetime(lifetimeSelector)));
    }
}
