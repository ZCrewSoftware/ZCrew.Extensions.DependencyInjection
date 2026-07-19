using System.Reflection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceExtensions
{
    extension(Service service)
    {
        /// <summary>
        ///     Adds the service types projected from a <typeparamref name="TAttribute"/> applied to the implementation
        ///     through <paramref name="serviceSelector"/> to the service, inspecting inherited attributes. An
        ///     implementation without the attribute, or for which the selector yields no service types, is left
        ///     registered against itself alone.
        /// </summary>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <example>
        ///     <code>
        ///     Service.From&lt;ContractStore&gt;()
        ///         .AsServicesFromAttribute&lt;ContractAttribute&gt;(attribute => attribute.Contracts)
        ///     </code>
        /// </example>
        /// <returns>The modified service, or the same service if no service types were found.</returns>
        /// <exception cref="ArgumentException">
        ///     If the attribute names a service type the implementation isn't based on.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when the implementation has more than one matching <typeparamref name="TAttribute"/>.
        /// </exception>
        public Service AsServicesFromAttribute<TAttribute>(
            Func<TAttribute, IEnumerable<Type>> serviceSelector
        )
            where TAttribute : class
        {
            return service.AsServicesFromAttribute(true, serviceSelector);
        }

        /// <summary>
        ///     Adds the service types projected from a <typeparamref name="TAttribute"/> applied to the implementation
        ///     through <paramref name="serviceSelector"/> to the service. An implementation without the attribute, or
        ///     for which the selector yields no service types, is left registered against itself alone.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <returns>The modified service, or the same service if no service types were found.</returns>
        /// <exception cref="ArgumentException">
        ///     If the attribute names a service type the implementation isn't based on.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when the implementation has more than one matching <typeparamref name="TAttribute"/>.
        /// </exception>
        public Service AsServicesFromAttribute<TAttribute>(
            bool inherited,
            Func<TAttribute, IEnumerable<Type>> serviceSelector
        )
            where TAttribute : class
        {
            ArgumentNullException.ThrowIfNull(serviceSelector);
            return service.As(type =>
            {
                var attribute = type.GetAttribute<TAttribute>(inherited);
                return attribute != null ? serviceSelector(attribute) : [];
            });
        }

        /// <summary>
        ///     Adds the service types projected from an attribute assignable to <paramref name="attributeType"/>
        ///     applied to the implementation through <paramref name="serviceSelector"/> to the service, inspecting
        ///     inherited attributes. An implementation without a matching attribute, or for which the selector yields
        ///     no service types, is left registered against itself alone.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <example>
        ///     <code>
        ///     Service.From&lt;ContractStore&gt;()
        ///         .AsServicesFromAttribute(typeof(ContractAttribute), attribute => ((ContractAttribute)attribute).Contracts)
        ///     </code>
        /// </example>
        /// <returns>The modified service, or the same service if no service types were found.</returns>
        /// <exception cref="ArgumentException">
        ///     If the attribute names a service type the implementation isn't based on.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when the implementation has more than one attribute assignable to
        ///     <paramref name="attributeType"/>.
        /// </exception>
        public Service AsServicesFromAttribute(
            Type attributeType,
            Func<Attribute, IEnumerable<Type>> serviceSelector
        )
        {
            return service.AsServicesFromAttribute(attributeType, true, serviceSelector);
        }

        /// <summary>
        ///     Adds the service types projected from an attribute assignable to <paramref name="attributeType"/>
        ///     applied to the implementation through <paramref name="serviceSelector"/> to the service. An
        ///     implementation without a matching attribute, or for which the selector yields no service types, is left
        ///     registered against itself alone.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <returns>The modified service, or the same service if no service types were found.</returns>
        /// <exception cref="ArgumentException">
        ///     If the attribute names a service type the implementation isn't based on.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when the implementation has more than one attribute assignable to
        ///     <paramref name="attributeType"/>.
        /// </exception>
        public Service AsServicesFromAttribute(
            Type attributeType,
            bool inherited,
            Func<Attribute, IEnumerable<Type>> serviceSelector
        )
        {
            ArgumentNullException.ThrowIfNull(serviceSelector);
            return service.As(type =>
            {
                var attribute = type.GetAttribute(attributeType, inherited);
                return attribute != null ? serviceSelector(attribute) : [];
            });
        }
    }
}
