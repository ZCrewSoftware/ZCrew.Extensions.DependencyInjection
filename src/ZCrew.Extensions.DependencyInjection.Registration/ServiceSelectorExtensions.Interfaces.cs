namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceSelectorExtensions
{
    extension(ServiceSelector selector)
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
        public ServiceKeySelector AsAllInterfaces()
        {
            return selector.As(type => type.GetInterfaces());
        }

        /// <summary>
        ///     Registers each type against all interfaces it implements; or <see cref="AsSelf"/> if there were no
        ///     interfaces found.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Classes.From(types).AsAllInterfacesOrSelf()
        ///     // Registers CustomerRepository as both
        ///     // ICustomerRepository and IDisposable
        ///     </code>
        /// </example>
        public ServiceKeySelector AsAllInterfacesOrSelf()
        {
            return selector.As(type =>
            {
                var interfaces = type.GetInterfaces();
                return interfaces.Length == 0 ? [type] : interfaces;
            });
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
        public ServiceKeySelector AsAllNonSystemInterfaces()
        {
            return selector.As(type =>
                type.GetInterfaces().Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
            );
        }

        /// <summary>
        ///     Registers each type against all interfaces it implements, excluding interfaces in the <c>System</c>
        ///     namespace and its sub-namespaces; or <see cref="AsSelf"/> if there were no interfaces found.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Classes.From(types).AsAllNonSystemInterfacesOrSelf()
        ///     // Registers CustomerRepository as ICustomerRepository only
        ///     // (IDisposable is in System and is excluded)
        ///     </code>
        /// </example>
        public ServiceKeySelector AsAllNonSystemInterfacesOrSelf()
        {
            return selector.As(type =>
            {
                var interfaces = type.GetInterfaces().Where(service =>
                    !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true)).ToArray();
                return interfaces.Length == 0 ? [type] : interfaces;
            });
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
        public ServiceKeySelector AsDefaultInterfaces()
        {
            return selector.As(type =>
                type.GetInterfaces().Where(service => type.Name.Contains(service.GetInterfaceName()))
            );
        }

        /// <summary>
        ///     Registers each type against interfaces whose name matches the type name by convention; or
        ///     <see cref="AsSelf"/> if there were no interfaces found.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Classes.From(types).AsDefaultInterfacesOrSelf()
        ///     // Registers CustomerRepository as ICustomerRepository
        ///     // ("CustomerRepository" contains "CustomerRepository" from
        ///     // "ICustomerRepository", but not "Disposable")
        ///     </code>
        /// </example>
        public ServiceKeySelector AsDefaultInterfacesOrSelf()
        {
            return selector.As(type =>
            {
                var interfaces = type.GetInterfaces().Where(service => type.Name.Contains(service.GetInterfaceName())).ToArray();
                return interfaces.Length == 0 ? [type] : interfaces;
            });
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
        public ServiceKeySelector AsDefaultNonSystemInterfaces()
        {
            return selector.As(type =>
                type.GetInterfaces()
                    .Where(service => type.Name.Contains(service.GetInterfaceName()))
                    .Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
            );
        }

        /// <summary>
        ///     Registers each type against convention-matching interfaces (see <see cref="AsDefaultInterfaces"/>),
        ///     additionally excluding interfaces in the <c>System</c> namespace and its sub-namespaces; or
        ///     <see cref="AsSelf"/> if there were no interfaces found.
        /// </summary>
        /// <example>
        ///     <code>
        ///     Classes.From(types).AsDefaultNonSystemInterfacesOrSelf()
        ///     </code>
        /// </example>
        public ServiceKeySelector AsDefaultNonSystemInterfacesOrSelf()
        {
            return selector.As(type =>
            {
                var interfaces = type.GetInterfaces()
                    .Where(service => type.Name.Contains(service.GetInterfaceName()))
                    .Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
                    .ToArray();
                return interfaces.Length == 0 ? [type] : interfaces;
            });
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
        public ServiceKeySelector AsFirstInterface()
        {
            return selector.As(type =>
            {
                var firstInterface = type.GetInterfaces().FirstOrDefault();
                return firstInterface != null ? [firstInterface] : [];
            });
        }

        /// <summary>
        ///     Registers each type against the first interface it implements; or <see cref="AsSelf"/> if there were no
        ///     interfaces found.
        /// </summary>
        /// <example>
        ///     Given <c>Order : object</c> and <c>CustomerRepository : ICustomerRepository, IRepository</c>:
        ///     <code>
        ///     Classes.From(types).AsFirstInterface()
        ///     // Registers Order as Order
        ///     //           CustomerRepository as ICustomerRepository
        ///     </code>
        /// </example>
        public ServiceKeySelector AsFirstInterfaceOrSelf()
        {
            return selector.As(type => [type.GetInterfaces().FirstOrDefault(type)]);
        }

        /// <summary>
        ///     Registers each type against its top-level interfaces that derive from the base types specified via
        ///     <see cref="TypeFilter.BasedOn"/>.
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
        public ServiceKeySelector AsInterface()
        {
            return selector.As((type, baseTypes) => type.GetTopLevelInterfacesMatchingBaseTypes(baseTypes));
        }

        /// <summary>
        ///     Registers each type against its top-level interfaces that derive from the base types specified via
        ///     <see cref="TypeFilter.BasedOn"/>; or <see cref="AsSelf"/> if there were no interfaces found.
        /// </summary>
        /// <example>
        ///     Given <c>Order : object</c> and <c>CustomerRepository : ICustomerRepository : IRepository</c>:
        ///     <code>
        ///     Classes.From(types)
        ///         .BasedOn&lt;IRepository&gt;()
        ///         .AsInterface()
        ///     // Registers Order as Order
        ///     //           CustomerRepository as ICustomerRepository
        ///     </code>
        ///     Types without an interface are registered as themselves.
        /// </example>
        public ServiceKeySelector AsInterfaceOrSelf()
        {
            return selector.As((type, baseTypes) =>
            {
                var interfaces = type.GetTopLevelInterfacesMatchingBaseTypes(baseTypes).ToArray();
                return interfaces.Length == 0 ? [type] : interfaces;
            });
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
        public ServiceKeySelector AsInterface<T>()
        {
            return selector.As(type => type.GetTopLevelInterfacesMatchingBaseTypes([typeof(T)]));
        }

        /// <summary>
        ///     Registers each type against its top-level interfaces that derive from <typeparamref name="T"/>; or
        ///     <see cref="AsSelf"/> if there were no interfaces found.
        /// </summary>
        /// <typeparam name="T">The base interface type to match against.</typeparam>
        /// <example>
        ///     Given <c>Order : object</c> and <c>CustomerRepository : ICustomerRepository : IRepository</c>:
        ///     <code>
        ///     Classes.From(types).AsInterface&lt;IRepository&gt;()
        ///     // Registers Order as Order
        ///     //           CustomerRepository as ICustomerRepository
        ///     </code>
        ///     Types without an interface are registered as themselves.
        /// </example>
        public ServiceKeySelector AsInterfaceOrSelf<T>()
        {
            return selector.As(type =>
            {
                var interfaces = type.GetTopLevelInterfacesMatchingBaseTypes([typeof(T)]).ToArray();
                return interfaces.Length == 0 ? [type] : interfaces;
            });
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
        public ServiceKeySelector AsInterface(Type interfaceType)
        {
            ArgumentNullException.ThrowIfNull(interfaceType);
            return selector.As(type => type.GetTopLevelInterfacesMatchingBaseTypes([interfaceType]));
        }

        /// <summary>
        ///     Registers each type against its top-level interfaces that derive from <paramref name="interfaceType"/>;
        ///     or <see cref="AsSelf"/> if there were no interfaces found.
        /// </summary>
        /// <param name="interfaceType">The base interface type to match against.</param>
        /// <example>
        ///     Given <c>Order : object</c> and <c>CustomerRepository : ICustomerRepository : IRepository</c>:
        ///     <code>
        ///     Classes.From(types).AsInterface(typeof(IRepository))
        ///     // Registers Order as Order
        ///     //           CustomerRepository as ICustomerRepository
        ///     </code>
        ///     Types without an interface are registered as themselves.
        /// </example>
        public ServiceKeySelector AsInterfaceOrSelf(Type interfaceType)
        {
            ArgumentNullException.ThrowIfNull(interfaceType);
            return selector.As(type =>
            {
                var interfaces = type.GetTopLevelInterfacesMatchingBaseTypes([interfaceType]).ToArray();
                return interfaces.Length == 0 ? [type] : interfaces;
            });
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
        ///     //           OrderRepository as IOrderRepository
        ///     </code>
        /// </example>
        public ServiceKeySelector AsInterfaces(params Type[] interfaceTypes)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentNullException.ThrowIfNull(interfaceTypes);
            return selector.As(type => type.GetTopLevelInterfacesMatchingBaseTypes(interfaceTypes));
        }

        /// <summary>
        ///     Registers each type against its top-level interfaces that derive from any of the specified
        ///     <paramref name="interfaceTypes"/>; or <see cref="AsSelf"/> if there were no interfaces found.
        /// </summary>
        /// <param name="interfaceTypes">The base interface types to match against.</param>
        /// <example>
        ///     Given <c>Order : object</c>, <c>OrderService : IOrderService : IService</c> and
        ///     <c>OrderRepository : IOrderRepository : IRepository</c>:
        ///     <code>
        ///     Classes.From(types)
        ///         .AsInterfaces(typeof(IService), typeof(IRepository))
        ///     // Registers Order as Order
        ///     //           OrderService as IOrderService,
        ///     //           OrderRepository as IOrderRepository
        ///     </code>
        ///     Types without an interface are registered as themselves.
        /// </example>
        public ServiceKeySelector AsInterfacesOrSelf(params Type[] interfaceTypes)
        {
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentNullException.ThrowIfNull(interfaceTypes);
            return selector.As(type =>
            {
                var interfaces = type.GetTopLevelInterfacesMatchingBaseTypes(interfaceTypes).ToArray();
                return interfaces.Length == 0 ? [type] : interfaces;
            });
        }
    }
}
