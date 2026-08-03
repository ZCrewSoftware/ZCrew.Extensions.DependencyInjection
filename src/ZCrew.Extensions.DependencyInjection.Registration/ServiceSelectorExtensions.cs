using System.Diagnostics.CodeAnalysis;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Extensions for the <see cref="ServiceSelector"/> type to extend existing functionality with convenient helpers.
/// </summary>
/// <remarks>
///     This API is not compatible with trimming or native AOT; see <see cref="Classes"/>. The suppressions below cover
///     every part of this type, including the other <c>ServiceSelectorExtensions.*.cs</c> files.
/// </remarks>
[UnconditionalSuppressMessage(
    "Trimming",
    "IL2067:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The source value must declare at least the same requirements as those declared on the target location it is assigned to",
    Justification = Aot.Justification
)]
[UnconditionalSuppressMessage(
    "Trimming",
    "IL2070:'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The generic parameter of the source method does not have matching annotations",
    Justification = Aot.Justification
)]
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
