using System.Reflection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceComponentExtensions
{
    extension(ServiceComponent component)
    {
        /// <summary>
        ///     Adds the service types returned by the <see cref="IServiceTypesProvider"/> attribute applied to the
        ///     implementation to the component, inspecting inherited attributes. An implementation without such an
        ///     attribute, or whose provider yields no service types, is left registered against itself alone.
        /// </summary>
        /// <example>
        ///     <code>
        ///     Component.From&lt;StripePaymentGateway&gt;().AsServicesFromAttribute()
        ///     </code>
        /// </example>
        /// <returns>The modified component, or the same component if no service types were found.</returns>
        /// <exception cref="ArgumentException">
        ///     If the attribute names a service type the implementation isn't based on.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when the implementation has more than one <see cref="IServiceTypesProvider"/> attribute.
        /// </exception>
        public ServiceComponent AsServicesFromAttribute()
        {
            return component.AsServicesFromAttribute(true);
        }

        /// <summary>
        ///     Adds the service types returned by the <see cref="IServiceTypesProvider"/> attribute applied to the
        ///     implementation to the component. An implementation without such an attribute, or whose provider yields
        ///     no service types, is left registered against itself alone.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <returns>The modified component, or the same component if no service types were found.</returns>
        /// <exception cref="ArgumentException">
        ///     If the attribute names a service type the implementation isn't based on.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when the implementation has more than one <see cref="IServiceTypesProvider"/> attribute.
        /// </exception>
        public ServiceComponent AsServicesFromAttribute(bool inherited)
        {
            return component.As(type => type.GetAttribute<IServiceTypesProvider>(inherited)?.ServiceTypes ?? []);
        }

        /// <summary>
        ///     Adds the service types projected from a <typeparamref name="TAttribute"/> applied to the implementation
        ///     through <paramref name="serviceSelector"/> to the component, inspecting inherited attributes. An
        ///     implementation without the attribute, or for which the selector yields no service types, is left
        ///     registered against itself alone.
        /// </summary>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <example>
        ///     <code>
        ///     Component.From&lt;ContractStore&gt;()
        ///         .AsServicesFromAttribute&lt;ContractAttribute&gt;(attribute => attribute.Contracts)
        ///     </code>
        /// </example>
        /// <returns>The modified component, or the same component if no service types were found.</returns>
        /// <exception cref="ArgumentException">
        ///     If the attribute names a service type the implementation isn't based on.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when the implementation has more than one matching <typeparamref name="TAttribute"/>.
        /// </exception>
        public ServiceComponent AsServicesFromAttribute<TAttribute>(
            Func<TAttribute, IEnumerable<Type>> serviceSelector
        )
            where TAttribute : class
        {
            return component.AsServicesFromAttribute(true, serviceSelector);
        }

        /// <summary>
        ///     Adds the service types projected from a <typeparamref name="TAttribute"/> applied to the implementation
        ///     through <paramref name="serviceSelector"/> to the component. An implementation without the attribute, or
        ///     for which the selector yields no service types, is left registered against itself alone.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <returns>The modified component, or the same component if no service types were found.</returns>
        /// <exception cref="ArgumentException">
        ///     If the attribute names a service type the implementation isn't based on.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when the implementation has more than one matching <typeparamref name="TAttribute"/>.
        /// </exception>
        public ServiceComponent AsServicesFromAttribute<TAttribute>(
            bool inherited,
            Func<TAttribute, IEnumerable<Type>> serviceSelector
        )
            where TAttribute : class
        {
            ArgumentNullException.ThrowIfNull(serviceSelector);
            return component.As(type =>
            {
                var attribute = type.GetAttribute<TAttribute>(inherited);
                return attribute != null ? serviceSelector(attribute) ?? [] : [];
            });
        }

        /// <summary>
        ///     Adds the service types projected from an attribute assignable to <paramref name="attributeType"/>
        ///     applied to the implementation through <paramref name="serviceSelector"/> to the component, inspecting
        ///     inherited attributes. An implementation without a matching attribute, or for which the selector yields
        ///     no service types, is left registered against itself alone.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <example>
        ///     <code>
        ///     Component.From&lt;ContractStore&gt;()
        ///         .AsServicesFromAttribute(typeof(ContractAttribute), attribute => ((ContractAttribute)attribute).Contracts)
        ///     </code>
        /// </example>
        /// <returns>The modified component, or the same component if no service types were found.</returns>
        /// <exception cref="ArgumentException">
        ///     If the attribute names a service type the implementation isn't based on.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when the implementation has more than one attribute assignable to
        ///     <paramref name="attributeType"/>.
        /// </exception>
        public ServiceComponent AsServicesFromAttribute(
            Type attributeType,
            Func<Attribute, IEnumerable<Type>> serviceSelector
        )
        {
            return component.AsServicesFromAttribute(attributeType, true, serviceSelector);
        }

        /// <summary>
        ///     Adds the service types projected from an attribute assignable to <paramref name="attributeType"/>
        ///     applied to the implementation through <paramref name="serviceSelector"/> to the component. An
        ///     implementation without a matching attribute, or for which the selector yields no service types, is left
        ///     registered against itself alone.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <returns>The modified component, or the same component if no service types were found.</returns>
        /// <exception cref="ArgumentException">
        ///     If the attribute names a service type the implementation isn't based on.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when the implementation has more than one attribute assignable to
        ///     <paramref name="attributeType"/>.
        /// </exception>
        public ServiceComponent AsServicesFromAttribute(
            Type attributeType,
            bool inherited,
            Func<Attribute, IEnumerable<Type>> serviceSelector
        )
        {
            ArgumentNullException.ThrowIfNull(serviceSelector);
            return component.As(type =>
            {
                var attribute = type.GetAttribute(attributeType, inherited);
                return attribute != null ? serviceSelector(attribute) ?? [] : [];
            });
        }
    }
}
