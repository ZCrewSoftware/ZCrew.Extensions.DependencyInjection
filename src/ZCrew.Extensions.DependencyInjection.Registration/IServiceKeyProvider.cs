using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Represents a provider for a <see cref="ServiceDescriptor.ServiceKey"/>.
/// </summary>
/// <seealso cref="KeyedAttribute"/>
public interface IServiceKeyProvider
{
    /// <summary>
    ///     The service key. <see langword="null"/> is valid and represents a non-keyed service.
    /// </summary>
    object? ServiceKey { get; }
}
