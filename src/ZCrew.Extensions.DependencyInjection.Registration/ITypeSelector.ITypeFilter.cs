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

    IServiceSelector ITypeFilter.InNamespace(string @namespace)
    {
        return new TypeFilter(SelectTypes()).InNamespace(@namespace);
    }

    IServiceSelector ITypeFilter.InNamespace(string @namespace, bool includeSubnamespaces)
    {
        return new TypeFilter(SelectTypes()).InNamespace(@namespace, includeSubnamespaces);
    }

    IServiceSelector ITypeFilter.InSameNamespaceAs(Type otherType)
    {
        return new TypeFilter(SelectTypes()).InSameNamespaceAs(otherType);
    }

    IServiceSelector ITypeFilter.InSameNamespaceAs(Type otherType, bool includeSubnamespaces)
    {
        return new TypeFilter(SelectTypes()).InSameNamespaceAs(otherType, includeSubnamespaces);
    }

    IServiceSelector ITypeFilter.InSameNamespaceAs<T>()
    {
        return new TypeFilter(SelectTypes()).InSameNamespaceAs<T>();
    }

    IServiceSelector ITypeFilter.InSameNamespaceAs<T>(bool includeSubnamespaces)
    {
        return new TypeFilter(SelectTypes()).InSameNamespaceAs(typeof(T), includeSubnamespaces);
    }

    ITypeFilter ITypeFilter.Where(Func<Type, bool> filter)
    {
        return new TypeFilter(SelectTypes()).Where(filter);
    }

    ITypeFilter ITypeFilter.BasedOn<T>()
    {
        return new TypeFilter(SelectTypes()).BasedOn<T>();
    }

    ITypeFilter ITypeFilter.BasedOn(Type baseType)
    {
        return new TypeFilter(SelectTypes()).BasedOn(baseType);
    }

    ITypeFilter ITypeFilter.BasedOn(params Type[] baseTypes)
    {
        return new TypeFilter(SelectTypes()).BasedOn(baseTypes);
    }
}
