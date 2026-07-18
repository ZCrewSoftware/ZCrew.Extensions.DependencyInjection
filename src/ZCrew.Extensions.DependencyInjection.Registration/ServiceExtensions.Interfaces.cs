namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceExtensions
{
    extension(Service service)
    {
        /// <summary>
        ///     Adds every interface the implementation implements to the service.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Service.From&lt;CustomerRepository&gt;().AsAllInterfaces()
        ///     // Registers CustomerRepository as CustomerRepository, ICustomerRepository
        ///     // and IDisposable, all resolving to one shared instance
        ///     </code>
        /// </example>
        /// <returns>The modified service, or the same service if the implementation has no interfaces.</returns>
        public Service AsAllInterfaces()
        {
            return service.As(type => type.GetInterfaces());
        }

        /// <summary>
        ///     Adds every interface the implementation implements to the service, excluding interfaces in the
        ///     <c>System</c> namespace and its sub-namespaces.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Service.From&lt;CustomerRepository&gt;().AsAllNonSystemInterfaces()
        ///     // Registers CustomerRepository as CustomerRepository and ICustomerRepository
        ///     // (IDisposable is in System and is excluded)
        ///     </code>
        /// </example>
        /// <returns>The modified service, or the same service if no interfaces matched.</returns>
        public Service AsAllNonSystemInterfaces()
        {
            return service.As(type =>
                type.GetInterfaces().Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
            );
        }

        /// <summary>
        ///     Adds the interfaces whose name matches the implementation name by convention to the service.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Service.From&lt;CustomerRepository&gt;().AsDefaultInterfaces()
        ///     // Registers CustomerRepository as CustomerRepository and ICustomerRepository
        ///     // ("CustomerRepository" contains "CustomerRepository" from
        ///     // "ICustomerRepository", but not "Disposable")
        ///     </code>
        /// </example>
        /// <returns>The modified service, or the same service if no interfaces matched.</returns>
        public Service AsDefaultInterfaces()
        {
            return service.As(type =>
                type.GetInterfaces().Where(service => type.Name.Contains(service.GetInterfaceName()))
            );
        }

        /// <summary>
        ///     Adds the convention-matching interfaces (see <see cref="AsDefaultInterfaces"/>) to the service,
        ///     additionally excluding interfaces in the <c>System</c> namespace and its sub-namespaces.
        /// </summary>
        /// <example>
        ///     <code>
        ///     Service.From&lt;CustomerRepository&gt;().AsDefaultNonSystemInterfaces()
        ///     </code>
        /// </example>
        /// <returns>The modified service, or the same service if no interfaces matched.</returns>
        public Service AsDefaultNonSystemInterfaces()
        {
            return service.As(type =>
                type.GetInterfaces()
                    .Where(service => type.Name.Contains(service.GetInterfaceName()))
                    .Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
            );
        }

        /// <summary>
        ///     Adds the first interface the implementation implements to the service.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : ICustomerRepository, IRepository</c>:
        ///     <code>
        ///     Service.From&lt;CustomerRepository&gt;().AsFirstInterface()
        ///     // Registers CustomerRepository as CustomerRepository and ICustomerRepository
        ///     </code>
        /// </example>
        /// <returns>The modified service, or the same service if the implementation has no interfaces.</returns>
        public Service AsFirstInterface()
        {
            return service.As(type =>
            {
                var firstInterface = type.GetInterfaces().FirstOrDefault();
                return firstInterface != null ? [firstInterface] : [];
            });
        }
    }
}
