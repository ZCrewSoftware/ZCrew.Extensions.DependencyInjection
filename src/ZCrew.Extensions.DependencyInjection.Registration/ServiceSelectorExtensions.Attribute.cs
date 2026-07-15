using System.Reflection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceSelectorExtensions
{
    extension(ServiceSelector selector)
    {
        /// <summary>
        ///     Registers each type against the service types returned by the <see cref="IServiceTypesProvider"/>
        ///     attribute applied to the implementation type, inspecting inherited attributes. Implementation types
        ///     without such an attribute, or whose provider yields no service types, are not registered.
        /// </summary>
        /// <example>
        ///     <code>
        ///     Classes.FromThisAssembly().BasedOn&lt;IPaymentGateway&gt;().AsServicesFromAttribute()
        ///     </code>
        /// </example>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one <see cref="IServiceTypesProvider"/> attribute.
        /// </exception>
        public ServiceSelector AsServicesFromAttribute()
        {
            return selector.AsServicesFromAttribute(true);
        }

        /// <summary>
        ///     Registers each type against the service types returned by the <see cref="IServiceTypesProvider"/>
        ///     attribute applied to the implementation type. Implementation types without such an attribute, or whose
        ///     provider yields no service types, are not registered.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one <see cref="IServiceTypesProvider"/> attribute.
        /// </exception>
        public ServiceSelector AsServicesFromAttribute(bool inherited)
        {
            return selector.As(type => type.GetAttribute<IServiceTypesProvider>(inherited)?.ServiceTypes ?? []);
        }

        /// <summary>
        ///     Registers each type against the service types returned by the <see cref="IServiceTypesProvider"/>
        ///     attribute applied to the implementation type, inspecting inherited attributes; or
        ///     <see cref="AsSelf"/> if there were no service types found.
        /// </summary>
        /// <example>
        ///     <code>
        ///     Classes.FromThisAssembly().BasedOn&lt;IPaymentGateway&gt;().AsServicesFromAttributeOrSelf()
        ///     </code>
        /// </example>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one <see cref="IServiceTypesProvider"/> attribute.
        /// </exception>
        public ServiceSelector AsServicesFromAttributeOrSelf()
        {
            return selector.AsServicesFromAttributeOrSelf(true);
        }

        /// <summary>
        ///     Registers each type against the service types returned by the <see cref="IServiceTypesProvider"/>
        ///     attribute applied to the implementation type; or <see cref="AsSelf"/> if there were no service types
        ///     found.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one <see cref="IServiceTypesProvider"/> attribute.
        /// </exception>
        public ServiceSelector AsServicesFromAttributeOrSelf(bool inherited)
        {
            return selector.As(type =>
            {
                var services = (type.GetAttribute<IServiceTypesProvider>(inherited)?.ServiceTypes ?? []).ToArray();
                return services.Length == 0 ? [type] : services;
            });
        }

        /// <summary>
        ///     Registers each type against the service types projected from a <typeparamref name="TAttribute"/>
        ///     applied to the implementation type through <paramref name="serviceSelector"/>, inspecting inherited
        ///     attributes. Implementation types without the attribute, or for which the selector yields no service
        ///     types, are not registered.
        /// </summary>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <example>
        ///     <code>
        ///     Classes.FromThisAssembly().BasedOn&lt;IStore&gt;()
        ///         .AsServicesFromAttribute&lt;ContractAttribute&gt;(attribute => attribute.Contracts)
        ///     </code>
        /// </example>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one matching <typeparamref name="TAttribute"/>.
        /// </exception>
        public ServiceSelector AsServicesFromAttribute<TAttribute>(
            Func<TAttribute, IEnumerable<Type>> serviceSelector
        )
            where TAttribute : class
        {
            return selector.AsServicesFromAttribute(true, serviceSelector);
        }

        /// <summary>
        ///     Registers each type against the service types projected from a <typeparamref name="TAttribute"/>
        ///     applied to the implementation type through <paramref name="serviceSelector"/>. Implementation types
        ///     without the attribute, or for which the selector yields no service types, are not registered.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one matching <typeparamref name="TAttribute"/>.
        /// </exception>
        public ServiceSelector AsServicesFromAttribute<TAttribute>(
            bool inherited,
            Func<TAttribute, IEnumerable<Type>> serviceSelector
        )
            where TAttribute : class
        {
            ArgumentNullException.ThrowIfNull(serviceSelector);
            return selector.As(type =>
            {
                var attribute = type.GetAttribute<TAttribute>(inherited);
                return attribute != null ? serviceSelector(attribute) ?? [] : [];
            });
        }

        /// <summary>
        ///     Registers each type against the service types projected from a <typeparamref name="TAttribute"/>
        ///     applied to the implementation type through <paramref name="serviceSelector"/>, inspecting inherited
        ///     attributes; or <see cref="AsSelf"/> if there were no service types found.
        /// </summary>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one matching <typeparamref name="TAttribute"/>.
        /// </exception>
        public ServiceSelector AsServicesFromAttributeOrSelf<TAttribute>(
            Func<TAttribute, IEnumerable<Type>> serviceSelector
        )
            where TAttribute : class
        {
            return selector.AsServicesFromAttributeOrSelf(true, serviceSelector);
        }

        /// <summary>
        ///     Registers each type against the service types projected from a <typeparamref name="TAttribute"/>
        ///     applied to the implementation type through <paramref name="serviceSelector"/>; or <see cref="AsSelf"/>
        ///     if there were no service types found.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one matching <typeparamref name="TAttribute"/>.
        /// </exception>
        public ServiceSelector AsServicesFromAttributeOrSelf<TAttribute>(
            bool inherited,
            Func<TAttribute, IEnumerable<Type>> serviceSelector
        )
            where TAttribute : class
        {
            ArgumentNullException.ThrowIfNull(serviceSelector);
            return selector.As(type =>
            {
                var attribute = type.GetAttribute<TAttribute>(inherited);
                var services = (attribute != null ? serviceSelector(attribute) ?? [] : []).ToArray();
                return services.Length == 0 ? [type] : services;
            });
        }

        /// <summary>
        ///     Registers each type against the service types projected from an attribute assignable to
        ///     <paramref name="attributeType"/> applied to the implementation type through
        ///     <paramref name="serviceSelector"/>, inspecting inherited attributes. Implementation types without a
        ///     matching attribute, or for which the selector yields no service types, are not registered.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <example>
        ///     <code>
        ///     Classes.FromThisAssembly().BasedOn&lt;IStore&gt;()
        ///         .AsServicesFromAttribute(typeof(ContractAttribute), attribute => ((ContractAttribute)attribute).Contracts)
        ///     </code>
        /// </example>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one attribute assignable to
        ///     <paramref name="attributeType"/>.
        /// </exception>
        public ServiceSelector AsServicesFromAttribute(
            Type attributeType,
            Func<Attribute, IEnumerable<Type>> serviceSelector
        )
        {
            return selector.AsServicesFromAttribute(attributeType, true, serviceSelector);
        }

        /// <summary>
        ///     Registers each type against the service types projected from an attribute assignable to
        ///     <paramref name="attributeType"/> applied to the implementation type through
        ///     <paramref name="serviceSelector"/>. Implementation types without a matching attribute, or for which
        ///     the selector yields no service types, are not registered.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one attribute assignable to
        ///     <paramref name="attributeType"/>.
        /// </exception>
        public ServiceSelector AsServicesFromAttribute(
            Type attributeType,
            bool inherited,
            Func<Attribute, IEnumerable<Type>> serviceSelector
        )
        {
            ArgumentNullException.ThrowIfNull(serviceSelector);
            return selector.As(type =>
            {
                var attribute = type.GetAttribute(attributeType, inherited);
                return attribute != null ? serviceSelector(attribute) ?? [] : [];
            });
        }

        /// <summary>
        ///     Registers each type against the service types projected from an attribute assignable to
        ///     <paramref name="attributeType"/> applied to the implementation type through
        ///     <paramref name="serviceSelector"/>, inspecting inherited attributes; or <see cref="AsSelf"/> if there
        ///     were no service types found.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one attribute assignable to
        ///     <paramref name="attributeType"/>.
        /// </exception>
        public ServiceSelector AsServicesFromAttributeOrSelf(
            Type attributeType,
            Func<Attribute, IEnumerable<Type>> serviceSelector
        )
        {
            return selector.AsServicesFromAttributeOrSelf(attributeType, true, serviceSelector);
        }

        /// <summary>
        ///     Registers each type against the service types projected from an attribute assignable to
        ///     <paramref name="attributeType"/> applied to the implementation type through
        ///     <paramref name="serviceSelector"/>; or <see cref="AsSelf"/> if there were no service types found.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="serviceSelector">A function that maps the matching attribute to the service types.</param>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one attribute assignable to
        ///     <paramref name="attributeType"/>.
        /// </exception>
        public ServiceSelector AsServicesFromAttributeOrSelf(
            Type attributeType,
            bool inherited,
            Func<Attribute, IEnumerable<Type>> serviceSelector
        )
        {
            ArgumentNullException.ThrowIfNull(serviceSelector);
            return selector.As(type =>
            {
                var attribute = type.GetAttribute(attributeType, inherited);
                var services = (attribute != null ? serviceSelector(attribute) ?? [] : []).ToArray();
                return services.Length == 0 ? [type] : services;
            });
        }
    }
}
