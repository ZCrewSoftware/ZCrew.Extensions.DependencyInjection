using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Base class for type selectors that lazily produces service descriptors by registering each selected type as
///     itself with <see cref="ServiceLifetime.Singleton"/>.
/// </summary>
public abstract class TypeSelectorBase : ServiceSource, ITypeSelector
{
    /// <inheritdoc />
    public abstract IEnumerable<Type> SelectTypes();

    /// <inheritdoc />
    protected override IEnumerable<ServiceDescriptor> SelectServices()
    {
        return SelectTypes().Select(type => new ServiceDescriptor(type, type, ServiceLifetime.Singleton));
    }
}
