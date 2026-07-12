namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Base class for type filters. Provides bridge implementations of <see cref="IServiceSelector"/> methods that
///     route through <see cref="AllTypes"/>, so subclasses only need to implement the <see cref="ITypeFilter"/>
///     methods themselves.
/// </summary>
internal abstract class TypeFilterBase : ServiceSelectorBase, ITypeFilter
{
    /// <inheritdoc />
    public abstract IServiceSelector AllTypes();

    /// <inheritdoc />
    public abstract ITypeFilter Where(Func<Type, bool> filter);

    /// <inheritdoc />
    public abstract ITypeFilter BasedOn(params Type[] baseTypes);

    /// <inheritdoc />
    public override IServiceKeySelector As(Func<Type, IEnumerable<Type>> serviceSelector)
    {
        return AllTypes().As(serviceSelector);
    }

    /// <inheritdoc />
    public override IServiceKeySelector As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>> serviceSelector)
    {
        return AllTypes().As(serviceSelector);
    }
}
