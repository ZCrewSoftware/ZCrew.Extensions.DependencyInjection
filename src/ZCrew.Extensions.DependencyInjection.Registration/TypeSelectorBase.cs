namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Base class for type selectors. Provides bridge implementations of <see cref="ITypeFilter"/> methods that
///     materialize the selected types via <see cref="SelectTypes"/> into a new <see cref="TypeFilter"/> and delegate
///     to it, so subclasses only need to implement <see cref="SelectTypes"/>.
/// </summary>
internal abstract class TypeSelectorBase : TypeFilterBase, ITypeSelector
{
    /// <inheritdoc />
    public abstract IEnumerable<Type> SelectTypes();

    /// <inheritdoc />
    public override IServiceSelector AllTypes()
    {
        return new TypeFilter(SelectTypes()).AllTypes();
    }

    /// <inheritdoc />
    public override ITypeFilter Where(Func<Type, bool> filter)
    {
        return new TypeFilter(SelectTypes()).Where(filter);
    }

    /// <inheritdoc />
    public override ITypeFilter BasedOn(params Type[] baseTypes)
    {
        return new TypeFilter(SelectTypes()).BasedOn(baseTypes);
    }
}
