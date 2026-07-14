using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Declares the <see cref="ServiceLifetime"/> for a service. This can be detected automatically with
///     <see cref="ServiceLifetimeSelectorExtensions.AsLifetimeByAttribute(ServiceLifetimeSelector)"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class LifetimeAttribute : Attribute, IServiceLifetimeProvider
{
    /// <summary>
    ///     Initializes a new <see cref="LifetimeAttribute"/> specifying the lifetime for this service.
    /// </summary>
    /// <param name="lifetime">The service lifetime to apply to the registration.</param>
    public LifetimeAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    /// <inheritdoc />
    public ServiceLifetime Lifetime { get; }
}
