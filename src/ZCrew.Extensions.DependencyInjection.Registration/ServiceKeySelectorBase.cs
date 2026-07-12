using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Base class selecting service keys. Provides bridge implementations of <see cref="IServiceSource"/> methods that
///     route through <see cref="Unkeyed"/>, so subclasses only need to implement the <see cref="IServiceKeySelector"/>
///     methods themselves.
/// </summary>
internal abstract class ServiceKeySelectorBase : IServiceKeySelector
{
    /// <inheritdoc />
    public abstract IServiceSource Unkeyed();

    /// <inheritdoc />
    public abstract IServiceSource Keyed();

    /// <inheritdoc />
    public abstract IServiceSource Keyed(object? serviceKey);

    /// <inheritdoc />
    public abstract IServiceSource Keyed(Func<Type, object?> serviceKeySelector);

    /// <inheritdoc />
    public abstract IServiceSource Keyed(Func<Type, Type, object?> serviceKeySelector);

    /// <inheritdoc />
    public virtual IServiceCollection AsLifetime(ServiceLifetime lifetime, SharingMode sharingMode)
    {
        return Unkeyed().AsLifetime(lifetime, sharingMode);
    }

    /// <inheritdoc />
    public virtual IServiceCollection ToServiceCollection(IServiceCollection serviceCollection)
    {
        return Unkeyed().ToServiceCollection(serviceCollection);
    }
}
