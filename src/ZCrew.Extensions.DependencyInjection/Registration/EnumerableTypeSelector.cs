namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Selects types from an in-memory enumerable with an optional filter predicate.
/// </summary>
public sealed class EnumerableTypeSelector : TypeSelectorBase, ITypeSelector
{
    private readonly IEnumerable<Type> types;
    private readonly Func<Type, bool>? filter;

    internal EnumerableTypeSelector(IEnumerable<Type> types)
    {
        this.types = types;
        this.filter = null;
    }

    internal EnumerableTypeSelector(IEnumerable<Type> types, Func<Type, bool>? filter)
    {
        this.types = types;
        this.filter = filter;
    }

    /// <inheritdoc />
    public override IEnumerable<Type> SelectTypes()
    {
        if (this.filter != null)
        {
            return this.types.Where(this.filter);
        }

        return this.types;
    }
}
