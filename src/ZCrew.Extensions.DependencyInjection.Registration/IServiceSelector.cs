namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Defines methods for selecting which service types each implementation type should be registered as. This is
///     the service selection stage of the registration fluent API, analogous to Castle Windsor's <c>WithService</c>
///     methods. Maintains an immutable chain: each select method returns a new instance.
/// </summary>
public partial interface IServiceSelector : IKeyedServiceSelector
{
    /// <summary>
    ///     Registers each type against all interfaces it implements.
    /// </summary>
    /// <example>
    ///     Given <c>CustomerRepository : ICustomerRepository, IDisposable</c>:
    ///     <code>
    ///     Classes.From(types).AsAllInterfaces()
    ///     // Registers CustomerRepository as both
    ///     // ICustomerRepository and IDisposable
    ///     </code>
    /// </example>
    IKeyedServiceSelector AsAllInterfaces();

    /// <summary>
    ///     Registers each type against all interfaces it implements, excluding interfaces in the <c>System</c>
    ///     namespace and its sub-namespaces.
    /// </summary>
    /// <example>
    ///     Given <c>CustomerRepository : ICustomerRepository, IDisposable</c>:
    ///     <code>
    ///     Classes.From(types).AsAllNonSystemInterfaces()
    ///     // Registers CustomerRepository as ICustomerRepository only
    ///     // (IDisposable is in System and is excluded)
    ///     </code>
    /// </example>
    IKeyedServiceSelector AsAllNonSystemInterfaces();

    /// <summary>
    ///     Registers each type against interfaces whose name matches the type name by convention.
    /// </summary>
    /// <example>
    ///     Given <c>CustomerRepository : ICustomerRepository, IDisposable</c>:
    ///     <code>
    ///     Classes.From(types).AsDefaultInterfaces()
    ///     // Registers CustomerRepository as ICustomerRepository
    ///     // ("CustomerRepository" contains "CustomerRepository" from
    ///     // "ICustomerRepository", but not "Disposable")
    ///     </code>
    /// </example>
    IKeyedServiceSelector AsDefaultInterfaces();

    /// <summary>
    ///     Registers each type against convention-matching interfaces (see <see cref="AsDefaultInterfaces"/>),
    ///     additionally excluding interfaces in the <c>System</c> namespace and its sub-namespaces.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Classes.From(types).AsDefaultNonSystemInterfaces()
    ///     </code>
    /// </example>
    IKeyedServiceSelector AsDefaultNonSystemInterfaces();

    /// <summary>
    ///     Registers each type against the first interface it implements.
    ///     Types with no interfaces are skipped.
    /// </summary>
    /// <example>
    ///     Given <c>CustomerRepository : ICustomerRepository, IRepository</c>:
    ///     <code>
    ///     Classes.From(types).AsFirstInterface()
    ///     // Registers CustomerRepository as ICustomerRepository
    ///     </code>
    /// </example>
    IKeyedServiceSelector AsFirstInterface();

    /// <summary>
    ///     Registers each type against its top-level interfaces that derive from the base types specified via
    ///     <see cref="ITypeFilter.BasedOn{T}"/> or <see cref="ITypeFilter.BasedOn(Type)"/>.
    /// </summary>
    /// <example>
    ///     Given <c>CustomerRepository : ICustomerRepository : IRepository</c>:
    ///     <code>
    ///     Classes.From(types)
    ///         .BasedOn&lt;IRepository&gt;()
    ///         .AsInterface()
    ///     // Registers CustomerRepository as ICustomerRepository
    ///     </code>
    /// </example>
    IKeyedServiceSelector AsInterface();

    /// <summary>
    ///     Registers each type against its top-level interfaces that derive from <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The base interface type to match against.</typeparam>
    /// <example>
    ///     Given <c>CustomerRepository : ICustomerRepository : IRepository</c>:
    ///     <code>
    ///     Classes.From(types).AsInterface&lt;IRepository&gt;()
    ///     // Registers CustomerRepository as ICustomerRepository
    ///     </code>
    /// </example>
    IKeyedServiceSelector AsInterface<T>();

    /// <summary>
    ///     Registers each type against its top-level interfaces that derive from <paramref name="interfaceType"/>.
    /// </summary>
    /// <param name="interfaceType">The base interface type to match against.</param>
    /// <example>
    ///     Given <c>CustomerRepository : ICustomerRepository : IRepository</c>:
    ///     <code>
    ///     Classes.From(types).AsInterface(typeof(IRepository))
    ///     // Registers CustomerRepository as ICustomerRepository
    ///     </code>
    /// </example>
    IKeyedServiceSelector AsInterface(Type interfaceType);

    /// <summary>
    ///     Registers each type against its top-level interfaces that derive from any of the specified
    ///     <paramref name="interfaceTypes"/>.
    /// </summary>
    /// <param name="interfaceTypes">The base interface types to match against.</param>
    /// <example>
    ///     Given <c>OrderService : IOrderService : IService</c> and
    ///     <c>OrderRepository : IOrderRepository : IRepository</c>:
    ///     <code>
    ///     Classes.From(types)
    ///         .AsInterfaces(typeof(IService), typeof(IRepository))
    ///     // Registers OrderService as IOrderService,
    ///     //          OrderRepository as IOrderRepository
    ///     </code>
    /// </example>
    IKeyedServiceSelector AsInterfaces(params Type[] interfaceTypes);

    /// <summary>
    ///     Registers each type against service types returned by the specified <paramref name="typeSelector"/> delegate.
    /// </summary>
    /// <param name="typeSelector">
    ///     A function that receives the implementation type and returns the service types to register.
    /// </param>
    /// <example>
    ///     <code>
    ///     Classes.From(types).As(type => type.GetInterfaces()
    ///         .Where(i => i.Name.EndsWith("Service"))
    ///         .ToArray())
    ///     </code>
    /// </example>
    IKeyedServiceSelector As(Func<Type, Type[]> typeSelector);

    /// <summary>
    ///     Registers each type against service types returned by the specified
    ///     <paramref name="typeWithBaseTypesSelector"/> delegate, which also receives the resolved base types.
    /// </summary>
    /// <param name="typeWithBaseTypesSelector">
    ///     A function that receives the implementation type and its resolved base types, and returns the service
    ///     types to register.
    /// </param>
    /// <example>
    ///     <code>
    ///     Classes.From(types)
    ///         .BasedOn&lt;IRepository&gt;()
    ///         .As((type, baseTypes) => baseTypes)
    ///     </code>
    /// </example>
    IKeyedServiceSelector As(Func<Type, Type[], Type[]> typeWithBaseTypesSelector);

    /// <summary>
    ///     Registers each type as itself (the implementation type is also the service type).
    /// </summary>
    /// <example>
    ///     Given <c>CustomerRepository : ICustomerRepository</c>:
    ///     <code>
    ///     Classes.From(types).AsSelf()
    ///     // Registers CustomerRepository as CustomerRepository
    ///     </code>
    /// </example>
    IKeyedServiceSelector AsSelf();

    /// <summary>
    ///     Registers each type against the base types specified via <see cref="ITypeFilter.BasedOn{T}"/> or
    ///     <see cref="ITypeFilter.BasedOn(Type)"/>.
    /// </summary>
    /// <example>
    ///     Given <c>CustomerRepository : ICustomerRepository : IRepository</c>:
    ///     <code>
    ///     Classes.From(types)
    ///         .BasedOn&lt;IRepository&gt;()
    ///         .AsBase()
    ///     // Registers CustomerRepository as IRepository
    ///     </code>
    /// </example>
    IKeyedServiceSelector AsBase();
}
