namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceComponentExtensions
{
    extension(ServiceComponent component)
    {
        /// <summary>
        ///     Adds every interface the implementation implements to the component.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Component.From&lt;CustomerRepository&gt;().AsAllInterfaces()
        ///     // Registers CustomerRepository as CustomerRepository, ICustomerRepository
        ///     // and IDisposable, all resolving to one shared instance
        ///     </code>
        /// </example>
        /// <returns>The modified component, or the same component if the implementation has no interfaces.</returns>
        public ServiceComponent AsAllInterfaces()
        {
            return component.As(type => type.GetInterfaces());
        }

        /// <summary>
        ///     Adds every interface the implementation implements to the component, excluding interfaces in the
        ///     <c>System</c> namespace and its sub-namespaces.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Component.From&lt;CustomerRepository&gt;().AsAllNonSystemInterfaces()
        ///     // Registers CustomerRepository as CustomerRepository and ICustomerRepository
        ///     // (IDisposable is in System and is excluded)
        ///     </code>
        /// </example>
        /// <returns>The modified component, or the same component if no interfaces matched.</returns>
        public ServiceComponent AsAllNonSystemInterfaces()
        {
            return component.As(type =>
                type.GetInterfaces().Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
            );
        }

        /// <summary>
        ///     Adds the interfaces whose name matches the implementation name by convention to the component.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Component.From&lt;CustomerRepository&gt;().AsDefaultInterfaces()
        ///     // Registers CustomerRepository as CustomerRepository and ICustomerRepository
        ///     // ("CustomerRepository" contains "CustomerRepository" from
        ///     // "ICustomerRepository", but not "Disposable")
        ///     </code>
        /// </example>
        /// <returns>The modified component, or the same component if no interfaces matched.</returns>
        public ServiceComponent AsDefaultInterfaces()
        {
            return component.As(type =>
                type.GetInterfaces().Where(service => type.Name.Contains(service.GetInterfaceName()))
            );
        }

        /// <summary>
        ///     Adds the convention-matching interfaces (see <see cref="AsDefaultInterfaces"/>) to the component,
        ///     additionally excluding interfaces in the <c>System</c> namespace and its sub-namespaces.
        /// </summary>
        /// <example>
        ///     <code>
        ///     Component.From&lt;CustomerRepository&gt;().AsDefaultNonSystemInterfaces()
        ///     </code>
        /// </example>
        /// <returns>The modified component, or the same component if no interfaces matched.</returns>
        public ServiceComponent AsDefaultNonSystemInterfaces()
        {
            return component.As(type =>
                type.GetInterfaces()
                    .Where(service => type.Name.Contains(service.GetInterfaceName()))
                    .Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
            );
        }

        /// <summary>
        ///     Adds the first interface the implementation implements to the component.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : ICustomerRepository, IRepository</c>:
        ///     <code>
        ///     Component.From&lt;CustomerRepository&gt;().AsFirstInterface()
        ///     // Registers CustomerRepository as CustomerRepository and ICustomerRepository
        ///     </code>
        /// </example>
        /// <returns>The modified component, or the same component if the implementation has no interfaces.</returns>
        public ServiceComponent AsFirstInterface()
        {
            return component.As(type =>
            {
                var firstInterface = type.GetInterfaces().FirstOrDefault();
                return firstInterface != null ? [firstInterface] : [];
            });
        }
    }
}
