namespace ZCrew.Extensions.DependencyInjection.Registration;

public partial interface ITypeFilter : IServiceSelector
{
    IServiceSelector AllTypes();

    IServiceSelector InNamespace(string @namespace);

    IServiceSelector InNamespace(string @namespace, bool includeSubnamespaces);

    IServiceSelector InSameNamespaceAs(Type otherType);

    IServiceSelector InSameNamespaceAs(Type otherType, bool includeSubnamespaces);

    IServiceSelector InSameNamespaceAs<T>();

    IServiceSelector InSameNamespaceAs<T>(bool includeSubnamespaces);

    ITypeFilter Where(Func<Type, bool> filter);

    ITypeFilter BasedOn<T>();

    ITypeFilter BasedOn(Type baseType);

    ITypeFilter BasedOn(params Type[] baseTypes);
}
