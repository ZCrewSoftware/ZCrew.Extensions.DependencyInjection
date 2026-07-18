using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Represents a provider for the <see cref="ServiceDescriptor.ServiceType"/>(s) an implementation is registered
///     against.
/// </summary>
/// <seealso cref="AsServicesAttribute"/>
public interface IServiceTypesProvider
{
    /// <summary>
    ///     The service types to register the implementation against. An empty sequence represents no services.
    /// </summary>
    IEnumerable<Type> ServiceTypes { get; }
}
