using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public class ServiceSelector : ServiceSource, IServiceSelector
{
    private readonly IEnumerable<Type> types;
    private readonly IEnumerable<Type> baseTypes;

    internal ServiceSelector(IEnumerable<Type> types, IEnumerable<Type> baseTypes)
    {
        this.types = types;
        this.baseTypes = baseTypes;
    }

    public IServiceSource AsAllInterfaces()
    {
        return SelectFromType(type => type.GetInterfaces());
    }

    public IServiceSource AsAllNonSystemInterfaces()
    {
        return SelectFromType(type =>
            type.GetInterfaces().Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
        );
    }

    public IServiceSource AsDefaultInterfaces()
    {
        return SelectFromType(type =>
            type.GetInterfaces().Where(service => type.Name.Contains(service.GetInterfaceName()))
        );
    }

    public IServiceSource AsDefaultNonSystemInterfaces()
    {
        return SelectFromType(type =>
            type.GetInterfaces()
                .Where(service => type.Name.Contains(service.GetInterfaceName()))
                .Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
        );
    }

    public IServiceSource AsFirstInterface()
    {
        return SelectFromType(type =>
        {
            var firstInterface = type.GetInterfaces().FirstOrDefault();
            return firstInterface != null ? [firstInterface] : [];
        });
    }

    public IServiceSource As(Func<Type, Type[]> typeSelector)
    {
        return SelectFromType(typeSelector);
    }

    public IServiceSource AsSelf()
    {
        return SelectFromType(type => [type]);
    }

    public IServiceSource AsInterface()
    {
        return SelectFromBase(GetDerivedTypes);
    }

    public IServiceSource AsInterface<T>()
    {
        return SelectFromType(type => GetDerivedTypes(type, [typeof(T)]));
    }

    public IServiceSource AsInterface(Type interfaceType)
    {
        return SelectFromType(type => GetDerivedTypes(type, [interfaceType]));
    }

    public IServiceSource AsInterfaces(params Type[] interfaceTypes)
    {
        return SelectFromType(type => GetDerivedTypes(type, interfaceTypes));
    }

    public IServiceSource As(Func<Type, Type[], Type[]> typeWithBaseTypesSelector)
    {
        return SelectFromBase(typeWithBaseTypesSelector);
    }

    public IServiceSource AsBase()
    {
        return SelectFromBase((_, basesTypes) => basesTypes);
    }

    protected override IEnumerable<ServiceDescriptor> SelectServices()
    {
        return AsSelf();
    }

    private ServiceCollectionSource SelectFromType(Func<Type, IEnumerable<Type>> serviceSelector)
    {
        var descriptors = new LinkedList<ServiceDescriptor>();
        foreach (var type in this.types)
        {
            var services = serviceSelector(type);
            foreach (var service in services)
            {
                var descriptor = new ServiceDescriptor(service, type, ServiceLifetime.Singleton);
                descriptors.AddLast(descriptor);
            }
        }
        return new ServiceCollectionSource(descriptors);
    }

    private ServiceCollectionSource SelectFromBase(Func<Type, Type[], IEnumerable<Type>> serviceSelector)
    {
        var descriptors = new LinkedList<ServiceDescriptor>();
        foreach (var type in this.types)
        {
            var assignableBaseTypes = GetBaseTypes(type);
            var services = serviceSelector(type, assignableBaseTypes);
            foreach (var service in services)
            {
                var descriptor = new ServiceDescriptor(service, type, ServiceLifetime.Singleton);
                descriptors.AddLast(descriptor);
            }
        }
        return new ServiceCollectionSource(descriptors);
    }

    private Type[] GetDerivedTypes(Type type, IEnumerable<Type> potentialBases)
    {
        var potentialBasesArray = potentialBases.ToArray();
        var matches = new HashSet<Type>();

        var topLevelInterfaces = type.GetTopLevelInterfaces();
        foreach (var topLevelInterface in topLevelInterfaces)
        {
            foreach (var interfaceType in potentialBasesArray)
            {
                if (interfaceType.IsAssignableFrom(topLevelInterface))
                {
                    matches.Add(topLevelInterface);
                    break;
                }
            }
        }

        if (matches.Count != 0)
        {
            return matches.ToArray();
        }

        // Slight deviation from Castle Windsor: this will register generic base types - this feels like an
        // oversight from the original impl
        return potentialBasesArray;
    }

    private Type[] GetBaseTypes(Type type)
    {
        var actuallyBasedOn = new HashSet<Type>();
        foreach (var potentialBase in this.baseTypes)
        {
            // Only consider specified base types or object if no base types were specified
            if (potentialBase.IsAssignableFrom(type))
            {
                actuallyBasedOn.Add(potentialBase);
                continue;
            }

            // If it isn't generic, and it isn't assignable then it isn't a possible base type
            if (!potentialBase.IsGenericTypeDefinition)
            {
                continue;
            }

            var genericBaseTypes = GetGenericBaseTypes(type, potentialBase);
            actuallyBasedOn.UnionWith(genericBaseTypes);
        }

        return actuallyBasedOn.ToArray();
    }

    private static Type[] GetGenericBaseTypes(Type type, Type genericInterface)
    {
        var types = new List<Type>(4);
        foreach (var @interface in type.GetInterfaces())
        {
            // Only consider generic types
            if (!@interface.IsGenericType)
            {
                continue;
            }

            // Only consider generic types that match the generic interface we're looking for
            if (@interface.GetGenericTypeDefinition() != genericInterface)
            {
                continue;
            }

            // If it is a generic unbound type (like List<T>) then register the generic type
            if (@interface.DeclaringType == null && @interface.ContainsGenericParameters)
            {
                types.Add(genericInterface);
                continue;
            }

            // It is a generic bound type (like List<string>)
            types.Add(@interface);
        }

        return types.ToArray();
    }
}
