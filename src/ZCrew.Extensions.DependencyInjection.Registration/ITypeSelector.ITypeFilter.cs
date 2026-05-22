namespace ZCrew.Extensions.DependencyInjection.Registration;

// Default interface implementations that bridge ITypeSelector to ITypeFilter.
// When a filter method is called on an ITypeSelector, the implementation
// materializes the selected types via SelectTypes() into a TypeFilter, then
// delegates to the corresponding filter method.
public partial interface ITypeSelector
{
    IServiceSelector ITypeFilter.AllTypes()
    {
        return new TypeFilter(SelectTypes());
    }

    ITypeFilter ITypeFilter.Where(Func<Type, bool> filter)
    {
        return new TypeFilter(SelectTypes()).Where(filter);
    }

    ITypeFilter ITypeFilter.BasedOn(params Type[] baseTypes)
    {
        return new TypeFilter(SelectTypes()).BasedOn(baseTypes);
    }
}
