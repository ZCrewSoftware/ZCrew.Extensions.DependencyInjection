namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Extensions for the <see cref="ServiceSelector"/> type to extend existing functionality with convenient helpers.
/// </summary>
public static partial class ServiceSelectorExtensions
{
    extension(ServiceSelector selector)
    {
        /// <summary>
        ///     Registers each type as itself (the implementation type is also the service type).
        /// </summary>
        /// <example>
        ///     Given <c>CustomerRepository : Repository, ICustomerRepository, IDisposable</c>:
        ///     <code>
        ///     Classes.From(types).AsSelf()
        ///     // Registers CustomerRepository as CustomerRepository
        ///     </code>
        /// </example>
        public ServiceSelector AsSelf()
        {
            return selector.As(type => [type]);
        }

        /// <summary>
        ///     Registers each type against the base types specified via <see cref="TypeFilter.BasedOn"/>.
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
        public ServiceSelector AsBase()
        {
            return selector.As((_, baseTypes) => baseTypes);
        }
    }
}
