namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceSelectorExtensions
{
    extension(ServiceSelector selector)
    {
        /// <summary>
        ///     Registers each type as itself, against all non-abstract classes it extends, and against all interfaces
        ///     it implements.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : AbstractRepository, ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Classes.From(types).AsAllInterfaces()
        ///     // Registers CustomerRepository as
        ///     // CustomerRepository, ICustomerRepository and IDisposable
        ///     // (AbstractRepository is abstract and is excluded)
        ///     </code>
        /// </example>
        public ServiceKeySelector AsAllTypes()
        {
            return selector.As(type => type.GetTypes().Where(service => !service.IsAbstractClass));
        }

        /// <summary>
        ///     Registers each type against all non-abstract classes it extends and interfaces it implements, excluding
        ///     types in the <c>System</c> namespace and its sub-namespaces.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : AbstractRepository, ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Classes.From(types).AsAllNonSystemTypes()
        ///     // Registers CustomerRepository as CustomerRepository and ICustomerRepository
        ///     // (IDisposable is in System and is excluded)
        ///     // (AbstractRepository is abstract and is excluded)
        ///     </code>
        /// </example>
        public ServiceKeySelector AsAllNonSystemTypes()
        {
            return selector.As(type =>
                type.GetTypes()
                    .Where(service => !service.IsAbstractClass)
                    .Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
            );
        }

        /// <summary>
        ///     Registers each type against all non-abstract classes it extends and interfaces it implements whose name
        ///     matches the type name by convention.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : AbstractRepository, ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Classes.From(types).AsDefaultTypes()
        ///     // Registers CustomerRepository as ICustomerRepository
        ///     // ("CustomerRepository" contains "CustomerRepository" from
        ///     // "ICustomerRepository", but not "Disposable" nor "AbstractRepository")
        ///     // (AbstractRepository is abstract and is excluded for a second reason)
        ///     </code>
        /// </example>
        public ServiceKeySelector AsDefaultTypes()
        {
            return selector.As(type =>
                type.GetTypes()
                    .Where(service => !service.IsAbstractClass)
                    .Where(service => type.Name.Contains(service.GetInterfaceName()))
            );
        }

        /// <summary>
        ///     Registers each type against all non-abstract classes it extends and interfaces it implements whose name
        ///     matches the type name by convention, excluding types in the <c>System</c> namespace and its
        ///     sub-namespaces.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : AbstractRepository, ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Classes.From(types).AsDefaultTypes()
        ///     // Registers CustomerRepository as ICustomerRepository
        ///     // ("CustomerRepository" contains "CustomerRepository" from
        ///     // "ICustomerRepository", but not "Disposable" nor "AbstractRepository")
        ///     // (IDisposable is in System and is excluded for a second reason)
        ///     // (AbstractRepository is abstract and is excluded for a second reason)
        ///     </code>
        /// </example>
        public ServiceKeySelector AsDefaultNonSystemTypes()
        {
            return selector.As(type =>
                type.GetTypes()
                    .Where(service => !service.IsAbstractClass)
                    .Where(service => type.Name.Contains(service.GetInterfaceName()))
                    .Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
            );
        }
    }
}
