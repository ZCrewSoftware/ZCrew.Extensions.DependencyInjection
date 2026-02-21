namespace ZCrew.Extensions.DependencyInjection.Registration;

public class EnumerableTypeSelector : TypeSelectorBase, ITypeSelector
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

    public override IEnumerable<Type> SelectTypes()
    {
        if (this.filter != null)
        {
            return this.types.Where(this.filter);
        }

        return this.types;
    }
}
