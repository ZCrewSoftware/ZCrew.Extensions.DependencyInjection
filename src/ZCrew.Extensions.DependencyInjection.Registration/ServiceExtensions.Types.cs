namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceExtensions
{
    extension(Service component)
    {
        /// <summary>
        ///     Adds every non-abstract class the implementation extends and every interface it implements to the
        ///     component.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : AbstractRepository, ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Service.From&lt;CustomerRepository&gt;().AsAllTypes()
        ///     // Registers CustomerRepository as CustomerRepository, ICustomerRepository
        ///     // and IDisposable
        ///     // (AbstractRepository is abstract and is excluded)
        ///     </code>
        /// </example>
        /// <returns>The modified component.</returns>
        public Service AsAllTypes()
        {
            return component.As(type => type.GetTypes().Where(service => !service.IsAbstractClass));
        }

        /// <summary>
        ///     Adds every non-abstract class the implementation extends and every interface it implements to the
        ///     component, excluding types in the <c>System</c> namespace and its sub-namespaces.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : AbstractRepository, ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Service.From&lt;CustomerRepository&gt;().AsAllNonSystemTypes()
        ///     // Registers CustomerRepository as CustomerRepository and ICustomerRepository
        ///     // (IDisposable is in System and is excluded)
        ///     // (AbstractRepository is abstract and is excluded)
        ///     </code>
        /// </example>
        /// <returns>The modified component.</returns>
        public Service AsAllNonSystemTypes()
        {
            return component.As(type =>
                type.GetTypes()
                    .Where(service => !service.IsAbstractClass)
                    .Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
            );
        }

        /// <summary>
        ///     Adds the non-abstract classes the implementation extends and the interfaces it implements whose name
        ///     matches the implementation name by convention to the component.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : AbstractRepository, ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Service.From&lt;CustomerRepository&gt;().AsDefaultTypes()
        ///     // Registers CustomerRepository as CustomerRepository and ICustomerRepository
        ///     // ("CustomerRepository" contains "CustomerRepository" from
        ///     // "ICustomerRepository", but not "Disposable" nor "AbstractRepository")
        ///     // (AbstractRepository is abstract and is excluded for a second reason)
        ///     </code>
        /// </example>
        /// <returns>The modified component.</returns>
        public Service AsDefaultTypes()
        {
            return component.As(type =>
                type.GetTypes()
                    .Where(service => !service.IsAbstractClass)
                    .Where(service => type.Name.Contains(service.GetInterfaceName()))
            );
        }

        /// <summary>
        ///     Adds the non-abstract classes the implementation extends and the interfaces it implements whose name
        ///     matches the implementation name by convention to the component, excluding types in the <c>System</c>
        ///     namespace and its sub-namespaces.
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : AbstractRepository, ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Service.From&lt;CustomerRepository&gt;().AsDefaultNonSystemTypes()
        ///     // Registers CustomerRepository as CustomerRepository and ICustomerRepository
        ///     // ("CustomerRepository" contains "CustomerRepository" from
        ///     // "ICustomerRepository", but not "Disposable" nor "AbstractRepository")
        ///     // (IDisposable is in System and is excluded for a second reason)
        ///     // (AbstractRepository is abstract and is excluded for a second reason)
        ///     </code>
        /// </example>
        /// <returns>The modified component.</returns>
        public Service AsDefaultNonSystemTypes()
        {
            return component.As(type =>
                type.GetTypes()
                    .Where(service => !service.IsAbstractClass)
                    .Where(service => type.Name.Contains(service.GetInterfaceName()))
                    .Where(service => !service.IsInSameNamespaceAs<object>(includeSubnamespaces: true))
            );
        }
    }
}
