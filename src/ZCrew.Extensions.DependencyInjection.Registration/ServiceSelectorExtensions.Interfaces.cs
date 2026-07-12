namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceSelectorExtensions
{
    extension(IServiceSelector selector)
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
        public IServiceKeySelector AsAllInterfaces()
        {
            return selector.As(type => type.GetInterfaces());
        }

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
        public IServiceKeySelector AsAllNonSystemInterfaces()
        {
            return selector.As(type =>
                type.GetInterfaces().Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
            );
        }

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
        public IServiceKeySelector AsDefaultInterfaces()
        {
            return selector.As(type =>
                type.GetInterfaces().Where(service => type.Name.Contains(service.GetInterfaceName()))
            );
        }

        /// <summary>
        ///     Registers each type against convention-matching interfaces (see <see cref="AsDefaultInterfaces"/>),
        ///     additionally excluding interfaces in the <c>System</c> namespace and its sub-namespaces.
        /// </summary>
        /// <example>
        ///     <code>
        ///     Classes.From(types).AsDefaultNonSystemInterfaces()
        ///     </code>
        /// </example>
        public IServiceKeySelector AsDefaultNonSystemInterfaces()
        {
            return selector.As(type =>
                type.GetInterfaces()
                    .Where(service => type.Name.Contains(service.GetInterfaceName()))
                    .Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
            );
        }

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
        public IServiceKeySelector AsFirstInterface()
        {
            return selector.As(type =>
            {
                var firstInterface = type.GetInterfaces().FirstOrDefault();
                return firstInterface != null ? [firstInterface] : [];
            });
        }

        /// <summary>
        ///     Registers each type against its top-level interfaces that derive from the base types specified via
        ///     <see cref="ITypeFilter.BasedOn"/>.
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
        public IServiceKeySelector AsInterface()
        {
            return selector.As((type, baseTypes) => type.GetTopLevelInterfacesMatchingBaseTypes(baseTypes));
        }

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
        public IServiceKeySelector AsInterface<T>()
        {
            return selector.As(type => type.GetTopLevelInterfacesMatchingBaseTypes([typeof(T)]));
        }

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
        public IServiceKeySelector AsInterface(Type interfaceType)
        {
            ArgumentNullException.ThrowIfNull(interfaceType);
            return selector.As(type => type.GetTopLevelInterfacesMatchingBaseTypes([interfaceType]));
        }

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
        public IServiceKeySelector AsInterfaces(params Type[] interfaceTypes)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentNullException.ThrowIfNull(interfaceTypes);
            return selector.As(type => type.GetTopLevelInterfacesMatchingBaseTypes(interfaceTypes));
        }
    }
}
