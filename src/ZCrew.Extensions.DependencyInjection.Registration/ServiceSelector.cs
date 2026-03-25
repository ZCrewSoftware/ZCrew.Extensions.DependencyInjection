using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Produces <see cref="ServiceDescriptor"/> registrations by mapping each implementation type to one or more
///     service types based on the chosen selection strategy (e.g. all interfaces, default interfaces, self, base
///     types). All generated descriptors use <see cref="ServiceLifetime.Singleton"/> by default.
/// </summary>
public sealed class ServiceSelector : ServiceSource, IServiceSelector
{
    private readonly IEnumerable<Type> types;
    private readonly IEnumerable<Type> baseTypes;

    internal ServiceSelector(IEnumerable<Type> types, IEnumerable<Type> baseTypes)
    {
        this.types = types;
        this.baseTypes = baseTypes;
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsAllInterfaces()
    {
        return SelectFromType(type => type.GetInterfaces());
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsAllNonSystemInterfaces()
    {
        return SelectFromType(type =>
            type.GetInterfaces().Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
        );
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsDefaultInterfaces()
    {
        return SelectFromType(type =>
            type.GetInterfaces().Where(service => type.Name.Contains(service.GetInterfaceName()))
        );
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsDefaultNonSystemInterfaces()
    {
        return SelectFromType(type =>
            type.GetInterfaces()
                .Where(service => type.Name.Contains(service.GetInterfaceName()))
                .Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
        );
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsFirstInterface()
    {
        return SelectFromType(type =>
        {
            var firstInterface = type.GetInterfaces().FirstOrDefault();
            return firstInterface != null ? [firstInterface] : [];
        });
    }

    /// <inheritdoc />
    public IKeyedServiceSelector As(Func<Type, Type[]> typeSelector)
    {
        ArgumentNullException.ThrowIfNull(typeSelector);
        return SelectFromType(typeSelector);
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsSelf()
    {
        return SelectFromType(type => [type]);
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsInterface()
    {
        return SelectFromBase(GetDerivedTypes);
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsInterface<T>()
    {
        return SelectFromType(type => GetDerivedTypes(type, [typeof(T)]));
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsInterface(Type interfaceType)
    {
        ArgumentNullException.ThrowIfNull(interfaceType);
        return SelectFromType(type => GetDerivedTypes(type, [interfaceType]));
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsInterfaces(params Type[] interfaceTypes)
    {
        ArgumentNullException.ThrowIfNull(interfaceTypes);
        return SelectFromType(type => GetDerivedTypes(type, interfaceTypes));
    }

    /// <inheritdoc />
    public IKeyedServiceSelector As(Func<Type, Type[], Type[]> typeWithBaseTypesSelector)
    {
        ArgumentNullException.ThrowIfNull(typeWithBaseTypesSelector);
        return SelectFromBase(typeWithBaseTypesSelector);
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsBase()
    {
        return SelectFromBase((_, basesTypes) => basesTypes);
    }

    /// <inheritdoc />
    protected override IEnumerable<ServiceDescriptor> SelectServices(ServiceLifetime lifetime)
    {
        return SelectFromType(type => [type], lifetime);
    }

    private KeyedServiceSelector SelectFromType(
        Func<Type, IEnumerable<Type>> serviceSelector,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        var descriptors = new LinkedList<ServiceDescriptor>();
        foreach (var type in this.types)
        {
            var services = serviceSelector(type);
            foreach (var service in services)
            {
                var descriptor = new ServiceDescriptor(service, type, lifetime);
                descriptors.AddLast(descriptor);
            }
        }
        return new KeyedServiceSelector(descriptors);
    }

    private KeyedServiceSelector SelectFromBase(Func<Type, Type[], IEnumerable<Type>> serviceSelector)
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
        return new KeyedServiceSelector(descriptors);
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

                if (!interfaceType.IsGenericTypeDefinition)
                {
                    continue;
                }

                if (topLevelInterface.IsGenericType && topLevelInterface.GetGenericTypeDefinition() == interfaceType)
                {
                    matches.Add(topLevelInterface);
                    break;
                }

                if (
                    topLevelInterface
                        .GetInterfaces()
                        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType)
                )
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

        // Slight deviation from Castle Windsor: this will register generic base
        // types - this feels like an oversight from the original impl
        return potentialBasesArray;
    }

    private Type[] GetBaseTypes(Type type)
    {
        var actuallyBasedOn = new HashSet<Type>();
        foreach (var potentialBase in this.baseTypes)
        {
            if (potentialBase.IsAssignableFrom(type))
            {
                actuallyBasedOn.Add(potentialBase);
                continue;
            }

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
            if (!@interface.IsGenericType)
            {
                continue;
            }

            if (@interface.GetGenericTypeDefinition() != genericInterface)
            {
                continue;
            }

            if (@interface.DeclaringType == null && @interface.ContainsGenericParameters)
            {
                types.Add(genericInterface);
                continue;
            }

            types.Add(@interface);
        }

        return types.ToArray();
    }
}
