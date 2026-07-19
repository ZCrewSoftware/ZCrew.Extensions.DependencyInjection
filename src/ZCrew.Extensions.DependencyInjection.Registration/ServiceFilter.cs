using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Filters the compile-time <see cref="Service"/> registrations emitted by the <c>[Service]</c> source generator
///     (<c>Services.FromThisAssembly()</c>) before adding them to an <see cref="IServiceCollection"/>. Analogous to
///     <see cref="TypeFilter"/>, but terminal: the attribute already fixed each service's types, lifetime, and key, so
///     this stage only narrows the set. Immutable and lazy: each filter returns a new <see cref="ServiceFilter"/>
///     wrapping a deferred, chained sequence, and nothing is enumerated until adding the services to a collection.
/// </summary>
public sealed class ServiceFilter
{
    private readonly IEnumerable<Service> services;

    /// <summary>
    ///     Wraps the generated <see cref="Service"/> array.
    /// </summary>
    /// <param name="services">The generated services to wrap.</param>
    /// <remarks>
    ///     This constructor exists for the code the source generator emits for <c>Services.FromThisAssembly()</c>; it
    ///     is not intended to be called directly. It is public only because that generated code compiles into the
    ///     consuming assembly, where an internal member would be unreachable.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ServiceFilter(Service[] services)
    {
        ArgumentNullException.ThrowIfNull(services);
        this.services = services;
    }

    /// <summary>
    ///     Wraps a deferred sequence of services, for chaining filters. The distinct parameter type keeps this a valid
    ///     overload alongside the public array constructor.
    /// </summary>
    /// <param name="services">The services to wrap.</param>
    internal ServiceFilter(IEnumerable<Service> services)
    {
        this.services = services;
    }

    /// <summary>
    ///     Filters the services using a custom predicate over each <see cref="Service"/>. Can be chained to combine
    ///     multiple filters.
    /// </summary>
    /// <param name="filter">A predicate returning <see langword="true"/> for services to keep.</param>
    /// <returns>A new <see cref="ServiceFilter"/> over the retained services.</returns>
    public ServiceFilter Where(Func<Service, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return new ServiceFilter(this.services.Where(filter));
    }

    /// <summary>
    ///     Adds the descriptors represented by each retained <see cref="Service"/> into the
    ///     <paramref name="serviceCollection"/>. Each service keeps its attribute-decided lifetime and key.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add the descriptors to.</param>
    /// <returns>The <paramref name="serviceCollection"/>, for chaining.</returns>
    /// <remarks>
    ///     A service with no lifetime set is registered as <see cref="ServiceLifetime.Singleton"/>.
    /// </remarks>
    public IServiceCollection ToServiceCollection(IServiceCollection serviceCollection)
    {
        ArgumentNullException.ThrowIfNull(serviceCollection);
        foreach (var service in this.services)
        {
            service.AddServiceDescriptors(serviceCollection);
        }
        return serviceCollection;
    }

    /// <summary>
    ///     Collects the retained services into a new <see cref="IServiceCollection"/>.
    /// </summary>
    /// <returns>The resulting service collection.</returns>
    public IServiceCollection ToServiceCollection()
    {
        return ToServiceCollection(new ServiceCollection());
    }
}
