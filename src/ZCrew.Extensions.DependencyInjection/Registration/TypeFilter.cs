using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public class TypeFilter : ServiceSource, ITypeFilter
{
    private static readonly IEnumerable<Type> defaultBases = [typeof(object)];
    private readonly IEnumerable<Type> types;
    private readonly IEnumerable<Type> baseTypes;

    internal TypeFilter(IEnumerable<Type> types)
    {
        this.types = types;
        this.baseTypes = defaultBases;
    }

    internal TypeFilter(IEnumerable<Type> types, IEnumerable<Type> baseTypes)
    {
        this.types = types;
        this.baseTypes = baseTypes;
    }

    public IServiceSelector AllTypes()
    {
        return Where(_ => true);
    }

    public IServiceSelector InNamespace(string @namespace)
    {
        return Where(type => type.IsInNamespace(@namespace));
    }

    public IServiceSelector InNamespace(string @namespace, bool includeSubnamespaces)
    {
        return Where(type => type.IsInNamespace(@namespace, includeSubnamespaces));
    }

    public IServiceSelector InSameNamespaceAs(Type otherType)
    {
        return Where(type => type.IsInSameNamespaceAs(otherType));
    }

    public IServiceSelector InSameNamespaceAs(Type otherType, bool includeSubnamespaces)
    {
        return Where(type => type.IsInSameNamespaceAs(otherType, includeSubnamespaces));
    }

    public IServiceSelector InSameNamespaceAs<T>()
    {
        return Where(type => type.IsInSameNamespaceAs<T>());
    }

    public IServiceSelector InSameNamespaceAs<T>(bool includeSubnamespaces)
    {
        return Where(type => type.IsInSameNamespaceAs<T>(includeSubnamespaces));
    }

    public ITypeFilter Where(Func<Type, bool> filter)
    {
        return new TypeFilter(this.types.Where(filter));
    }

    public ITypeFilter BasedOn<T>()
    {
        return BasedOn([typeof(T)]);
    }

    public ITypeFilter BasedOn(Type baseType)
    {
        return BasedOn([baseType]);
    }

    public ITypeFilter BasedOn(params Type[] baseTypes)
    {
        if (ReferenceEquals(this.baseTypes, defaultBases))
        {
            return new TypeFilter(this.types, baseTypes);
        }

        return new TypeFilter(this.types, this.baseTypes.Concat(baseTypes));
    }

    protected override IEnumerable<ServiceDescriptor> SelectServices()
    {
        return this.types.Select(type => new ServiceDescriptor(type, type, ServiceLifetime.Singleton));
    }
}
