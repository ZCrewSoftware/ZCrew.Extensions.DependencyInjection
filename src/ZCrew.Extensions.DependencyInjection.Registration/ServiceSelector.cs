using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Produces <see cref="ServiceDescriptor"/> registrations by mapping each implementation type to one or more
///     service types based on the chosen selection strategy (e.g. all interfaces, default interfaces, self, base
///     types). All generated descriptors use <see cref="ServiceLifetime.Singleton"/> by default.
/// </summary>
internal sealed class ServiceSelector : IServiceSelector
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
        return SelectFromBase(GetTopLevelInterfacesBasedOnAny);
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsInterface<T>()
    {
        return SelectFromType(type => GetTopLevelInterfacesBasedOnAny(type, [typeof(T)]));
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsInterface(Type interfaceType)
    {
        ArgumentNullException.ThrowIfNull(interfaceType);
        return SelectFromType(type => GetTopLevelInterfacesBasedOnAny(type, [interfaceType]));
    }

    /// <inheritdoc />
    public IKeyedServiceSelector AsInterfaces(params Type[] interfaceTypes)
    {
        ArgumentNullException.ThrowIfNull(interfaceTypes);
        return SelectFromType(type => GetTopLevelInterfacesBasedOnAny(type, interfaceTypes));
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

    private KeyedServiceSelector SelectFromType(Func<Type, IEnumerable<Type>> serviceSelector)
    {
        return new KeyedServiceSelector(
            this.types.Select(type => new ServiceComponent(type, serviceSelector(type).ToArray()))
        );
    }

    private KeyedServiceSelector SelectFromBase(Func<Type, Type[], IEnumerable<Type>> serviceSelector)
    {
        return new KeyedServiceSelector(
            this.types.Select(type =>
            {
                var assignableBaseTypes = GetBaseTypes(type).ToArray();
                var services = serviceSelector(type, assignableBaseTypes).ToArray();
                return new ServiceComponent(type, services);
            })
        );
    }

    /// <summary>
    ///     Returns the most-derived interfaces of <paramref name="type"/> that are based on at least one of
    ///     <paramref name="potentialBases"/>. The returned values are <paramref name="type"/>'s own top-level
    ///     interfaces, the closest derived form to the implementation, so the DI container can resolve them.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If no top-level interface of <paramref name="type"/> is based on any of
    ///         <paramref name="potentialBases"/>, an empty array is returned and the type produces no
    ///         registrations. This matches the documented contract on <see cref="IServiceSelector.AsInterface()"/> and
    ///         its overloads.
    ///     </para>
    ///     <para>
    ///         When a matched top-level interface carries unbound generic parameters from <paramref name="type"/>
    ///         (e.g. an open-generic class implementing a generic interface), it is collapsed to its generic type
    ///         definition so the produced service type is resolvable by the DI container.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     Consider the following 4 types:
    ///     <code>
    ///         public interface IRepository&lt;T&gt;;
    ///         public interface ICachedRepository&lt;T&gt; : IRepository&lt;T&gt;;
    ///         public interface ICustomerRepository : ICachedRepository&lt;Customer&gt;;
    ///         public class CustomerRepository : ICustomerRepository;
    ///     </code>
    ///
    ///     We can see that <c>ICachedRepository&lt;T&gt;</c> and <c>ICustomerRepository</c> both <i>derive</i> from
    ///     <c>IRepository&lt;&gt;</c>. <c>ICustomerRepository</c> is <i>more-derived</i> or <i>closer</i> to
    ///     <c>CustomerRepository</c>. Calling this method will return the closest interface that derives
    ///     <c>IRepository&lt;&gt;</c>:
    ///
    ///     <code>
    ///         // types is [typeof(ICustomerRepository)]
    ///         var types = GetTopLevelInterfacesBasedOnAny(typeof(CustomerRepository), [typeof(IRepository&lt;&gt;)]);
    ///     </code>
    /// </example>
    private static IEnumerable<Type> GetTopLevelInterfacesBasedOnAny(Type type, Type[] potentialBases)
    {
        var matches = new HashSet<Type>();
        foreach (var topLevelInterface in type.GetTopLevelInterfaces())
        {
            foreach (var potentialBase in potentialBases)
            {
                if (!topLevelInterface.IsBasedOn(potentialBase))
                {
                    continue;
                }

                // If the interface has generic type parameters then use the open form
                // This would register 'IRepository<>' and 'IRepository<T>' as 'IRepository<>'
                if (topLevelInterface.ContainsGenericParameters)
                {
                    matches.Add(topLevelInterface.GetGenericTypeDefinition());
                    continue;
                }

                // The interface is closed, this would register 'IRepository<Customer>'
                matches.Add(topLevelInterface);
                break;
            }
        }

        return matches;
    }

    /// <summary>
    ///     Returns the constructed-or-open forms of <see cref="baseTypes"/> that <paramref name="type"/> derives from,
    ///     used by <see cref="AsBase"/> to map an implementation to its configured base service types.
    /// </summary>
    /// <remarks>
    ///     Walks <paramref name="type"/>'s interfaces and base class chain. Open generic bases are matched against
    ///     constructed forms; bound forms (e.g. <c>IRepository&lt;Customer&gt;</c>) are returned as-is, unbound forms
    ///     fall back to the open generic. <paramref name="type"/> itself is never included &#8212; registering a type
    ///     against itself is <see cref="AsSelf"/>'s responsibility.
    /// </remarks>
    private IEnumerable<Type> GetBaseTypes(Type type)
    {
        var results = new HashSet<Type>();
        foreach (var potentialBase in this.baseTypes)
        {
            AddMatchedBaseForms(type, potentialBase, results);
        }
        return results;
    }

    private static void AddMatchedBaseForms(Type type, Type baseType, HashSet<Type> results)
    {
        if (baseType.IsAssignableFrom(type))
        {
            results.Add(baseType);
            return;
        }

        // All non-generic types would have matched IsAssignableFrom - if it isn't generic then it isn't a match
        if (!baseType.IsGenericTypeDefinition)
        {
            return;
        }

        foreach (var @interface in type.GetInterfaces())
        {
            AddIfGenericBaseType(@interface, baseType, results);
        }

        for (var current = type.BaseType; current != null; current = current.BaseType)
        {
            AddIfGenericBaseType(current, baseType, results);
        }
    }

    private static void AddIfGenericBaseType(Type type, Type baseType, HashSet<Type> results)
    {
        // We only care about generic types here, where 'IsGenericTypeDefinition' was true for baseType
        if (!type.IsGenericType)
        {
            return;
        }

        // We only care about the same generic definition (without parameters)
        // 'IRepository<>', 'IRepository<T>', and 'IRepository<Customer>' all share the same generic type definition
        if (type.GetGenericTypeDefinition() != baseType)
        {
            return;
        }

        // If the interface or base type has generic type parameters then use the open form
        // This would register 'IRepository<>' and 'IRepository<T>' as 'IRepository<>'
        // Note: per the check above, 'baseType' is the generic definition we're after
        if (type.ContainsGenericParameters)
        {
            results.Add(baseType);
            return;
        }

        // The interface or base type is closed, this would register 'IRepository<Customer>'
        results.Add(type);
    }
}
