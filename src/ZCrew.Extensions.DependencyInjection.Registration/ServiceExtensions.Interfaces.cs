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
            return service.As(service.ImplementationType.GetInterfaces());
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
            return service.As(
                service
                    .ImplementationType.GetInterfaces()
                    .Where(candidate => !candidate.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
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
            var implementation = service.ImplementationType;
            return service.As(
                implementation.GetInterfaces().Where(candidate => implementation.Name.Contains(candidate.GetInterfaceName()))
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
            var implementation = service.ImplementationType;
            return service.As(
                implementation
                    .GetInterfaces()
                    .Where(candidate => implementation.Name.Contains(candidate.GetInterfaceName()))
                    .Where(candidate => !candidate.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
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
            var firstInterface = service.ImplementationType.GetInterfaces().FirstOrDefault();
            Type[] services = firstInterface != null ? [firstInterface] : [];
            return service.As(services);
        }
    }
}
