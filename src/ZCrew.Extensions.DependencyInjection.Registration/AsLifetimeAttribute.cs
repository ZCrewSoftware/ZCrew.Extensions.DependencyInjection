using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Declares the <see cref="ServiceLifetime"/> for a service. This can be detected automatically with
///     <see cref="ServiceLifetimeSelectorExtensions.AsLifetimeByAttribute(ServiceLifetimeSelector)"/>.
/// </summary>
/// <remarks>
///     The <c>As</c> prefix mirrors the fluent <c>AsLifetimeByAttribute</c> step this attribute feeds and keeps it
///     distinct from the source generator's <c>ServiceAttribute</c>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class AsLifetimeAttribute : Attribute, IServiceLifetimeProvider
{
    /// <summary>
    ///     Initializes a new <see cref="AsLifetimeAttribute"/> specifying the lifetime for this service.
    /// </summary>
    /// <param name="lifetime">The service lifetime to apply to the registration.</param>
    public AsLifetimeAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    /// <inheritdoc />
    public ServiceLifetime Lifetime { get; }
}
