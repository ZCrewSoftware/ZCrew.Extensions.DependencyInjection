using System.Reflection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Extensions for the <see cref="ServiceKeySelector"/> type to extend existing functionality with convenient
///     helpers.
/// </summary>
public static class ServiceKeySelectorExtensions
{
    extension(ServiceKeySelector selector)
    {
        /// <summary>
        ///     Assigns a service key to each registration from the <see cref="IServiceKeyProvider.ServiceKey"/> of
        ///     an <see cref="IServiceKeyProvider"/> attribute applied to the implementation type, inspecting
        ///     inherited attributes. Implementation types without such an attribute, or whose provider yields
        ///     <see langword="null"/>, are left unkeyed.
        /// </summary>
        /// <example>
        ///     <code>
        ///     Classes.FromThisAssembly().BasedOn&lt;IPaymentGateway&gt;().AsInterface().KeyedByAttribute()
        ///     </code>
        /// </example>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one <see cref="IServiceKeyProvider"/> attribute.
        /// </exception>
        public ServiceLifetimeSelector KeyedByAttribute()
        {
            return selector.KeyedByAttribute(true);
        }

        /// <summary>
        ///     Assigns a service key to each registration from the <see cref="IServiceKeyProvider.ServiceKey"/> of
        ///     an <see cref="IServiceKeyProvider"/> attribute applied to the implementation type. Implementation
        ///     types without such an attribute, or whose provider yields <see langword="null"/>, are left unkeyed.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one <see cref="IServiceKeyProvider"/> attribute.
        /// </exception>
        public ServiceLifetimeSelector KeyedByAttribute(bool inherited)
        {
            return selector.Keyed(type => type.GetAttribute<IServiceKeyProvider>(inherited)?.ServiceKey);
        }

        /// <summary>
        ///     Assigns a service key to each registration by projecting a <typeparamref name="TAttribute"/> applied
        ///     to the implementation type through <paramref name="serviceKeySelector"/>, inspecting inherited
        ///     attributes. Implementation types without the attribute, or for which the selector returns
        ///     <see langword="null"/>, are left unkeyed.
        /// </summary>
        /// <param name="serviceKeySelector">A function that maps the matching attribute to the service key.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <example>
        ///     <code>
        ///     Classes.FromThisAssembly().BasedOn&lt;IPaymentGateway&gt;().AsInterface()
        ///         .KeyedByAttribute&lt;ServiceKeyAttribute&gt;(attribute => attribute.Key)
        ///     </code>
        /// </example>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one matching <typeparamref name="TAttribute"/>.
        /// </exception>
        public ServiceLifetimeSelector KeyedByAttribute<TAttribute>(Func<TAttribute, object?> serviceKeySelector)
            where TAttribute : class
        {
            return selector.KeyedByAttribute(true, serviceKeySelector);
        }

        /// <summary>
        ///     Assigns a service key to each registration by projecting a <typeparamref name="TAttribute"/> applied
        ///     to the implementation type through <paramref name="serviceKeySelector"/>. Implementation types
        ///     without the attribute, or for which the selector returns <see langword="null"/>, are left unkeyed.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="serviceKeySelector">A function that maps the matching attribute to the service key.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one matching <typeparamref name="TAttribute"/>.
        /// </exception>
        public ServiceLifetimeSelector KeyedByAttribute<TAttribute>(
            bool inherited,
            Func<TAttribute, object?> serviceKeySelector
        )
            where TAttribute : class
        {
            ArgumentNullException.ThrowIfNull(serviceKeySelector);
            return selector.Keyed(type =>
            {
                var attribute = type.GetAttribute<TAttribute>(inherited);
                return attribute != null ? serviceKeySelector(attribute) : null;
            });
        }

        /// <summary>
        ///     Assigns a service key to each registration by projecting an attribute assignable to
        ///     <paramref name="attributeType"/> applied to the implementation type through
        ///     <paramref name="serviceKeySelector"/>, inspecting inherited attributes. Implementation types without
        ///     a matching attribute, or for which the selector returns <see langword="null"/>, are left unkeyed.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="serviceKeySelector">A function that maps the matching attribute to the service key.</param>
        /// <example>
        ///     <code>
        ///     Classes.FromThisAssembly().BasedOn&lt;IPaymentGateway&gt;().AsInterface()
        ///         .KeyedByAttribute(typeof(ServiceKeyAttribute), attribute => ((ServiceKeyAttribute)attribute).Key)
        ///     </code>
        /// </example>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one attribute assignable to
        ///     <paramref name="attributeType"/>.
        /// </exception>
        public ServiceLifetimeSelector KeyedByAttribute(Type attributeType, Func<Attribute, object?> serviceKeySelector)
        {
            return selector.KeyedByAttribute(attributeType, true, serviceKeySelector);
        }

        /// <summary>
        ///     Assigns a service key to each registration by projecting an attribute assignable to
        ///     <paramref name="attributeType"/> applied to the implementation type through
        ///     <paramref name="serviceKeySelector"/>. Implementation types without a matching attribute, or for
        ///     which the selector returns <see langword="null"/>, are left unkeyed.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="serviceKeySelector">A function that maps the matching attribute to the service key.</param>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one attribute assignable to
        ///     <paramref name="attributeType"/>.
        /// </exception>
        public ServiceLifetimeSelector KeyedByAttribute(
            Type attributeType,
            bool inherited,
            Func<Attribute, object?> serviceKeySelector
        )
        {
            ArgumentNullException.ThrowIfNull(serviceKeySelector);
            return selector.Keyed(type =>
            {
                var attribute = type.GetAttribute(attributeType, inherited);
                return attribute != null ? serviceKeySelector(attribute) : null;
            });
        }
    }
}
