using Microsoft.Extensions.DependencyInjection;
using ZCrew.Extensions.DependencyInjection.Registration;

namespace RegistrationSample.Services;

/// <summary>
///     Produces unique identifiers.
/// </summary>
public interface IIdGenerator
{
    /// <summary>Returns a new unique identifier.</summary>
    string NewId();
}

/// <summary>
///     Registered against itself and <see cref="IIdGenerator"/> as transient. A transient lifetime registers each
///     service type independently rather than forwarding to one shared instance, so the interface resolves straight
///     to the implementation type (not a factory) in the printed output.
/// </summary>
[Service(typeof(IIdGenerator), Lifetime = ServiceLifetime.Transient)]
public sealed class GuidIdGenerator : IIdGenerator
{
    /// <inheritdoc />
    public string NewId() => Guid.NewGuid().ToString();
}
