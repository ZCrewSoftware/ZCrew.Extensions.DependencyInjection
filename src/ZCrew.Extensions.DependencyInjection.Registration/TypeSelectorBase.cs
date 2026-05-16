using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Base class for type selectors that lazily produces service descriptors by registering each selected type as
///     itself with <see cref="ServiceLifetime.Singleton"/>.
/// </summary>
internal abstract class TypeSelectorBase : ITypeSelector
{
    /// <inheritdoc />
    public abstract IEnumerable<Type> SelectTypes();
}
