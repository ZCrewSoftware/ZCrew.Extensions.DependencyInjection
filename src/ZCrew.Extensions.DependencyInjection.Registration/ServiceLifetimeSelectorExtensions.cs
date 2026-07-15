using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Extensions for the <see cref="ServiceLifetimeSelector"/> type to extend existing functionality with convenient
///     helpers.
/// </summary>
public static class ServiceLifetimeSelectorExtensions
{
    extension(ServiceLifetimeSelector selector)
    {
        /// <summary>
        ///     Returns a new <see cref="IServiceCollection"/> with all descriptors set to the specified
        ///     <paramref name="lifetime"/>. Instance-based descriptors that cannot change lifetime are kept unchanged.
        ///     For <see cref="ServiceLifetime.Singleton"/> and <see cref="ServiceLifetime.Scoped"/>: if the
        ///     implementation has been selected as one of the services then one instance is shared across every
        ///     selected service type.
        /// </summary>
        /// <param name="lifetime">The target service lifetime.</param>
        public ServiceSource AsLifetime(ServiceLifetime lifetime)
        {
            return selector.AsLifetime(lifetime);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Singleton"/>. If the implementation has been
        ///     selected as one of the services then one singleton instance is shared across every selected service
        ///     type.
        /// </summary>
        public ServiceSource AsSingleton()
        {
            return selector.AsLifetime(ServiceLifetime.Singleton);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Scoped"/>. If the implementation has been
        ///     selected as one of the services then one scoped instance (per scope) is shared across every selected
        ///     service type.
        /// </summary>
        public ServiceSource AsScoped()
        {
            return selector.AsLifetime(ServiceLifetime.Scoped);
        }

        /// <summary>
        ///     Registers all descriptors as <see cref="ServiceLifetime.Transient"/>. A new instance is constructed on
        ///     every resolution.
        /// </summary>
        public ServiceSource AsTransient()
        {
            return selector.AsLifetime(ServiceLifetime.Transient);
        }

        /// <summary>
        ///     Assigns a service lifetime to each registration from the <see cref="IServiceLifetimeProvider.Lifetime"/>
        ///     of an <see cref="IServiceLifetimeProvider"/> attribute applied to the implementation type, inspecting
        ///     inherited attributes. Implementation types without such an attribute fall back to
        ///     <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        /// <example>
        ///     <code>
        ///     Classes.FromThisAssembly().BasedOn&lt;IPaymentGateway&gt;().AsInterface().AsLifetimeByAttribute()
        ///     </code>
        /// </example>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one <see cref="IServiceLifetimeProvider"/> attribute.
        /// </exception>
        public ServiceSource AsLifetimeByAttribute()
        {
            return selector.AsLifetimeByAttribute(true);
        }

        /// <summary>
        ///     Assigns a service lifetime to each registration from the <see cref="IServiceLifetimeProvider.Lifetime"/>
        ///     of an <see cref="IServiceLifetimeProvider"/> attribute applied to the implementation type.
        ///     Implementation types without such an attribute fall back to <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one <see cref="IServiceLifetimeProvider"/> attribute.
        /// </exception>
        public ServiceSource AsLifetimeByAttribute(bool inherited)
        {
            return selector.AsLifetime(type =>
                type.GetAttribute<IServiceLifetimeProvider>(inherited)?.Lifetime ?? ServiceLifetime.Singleton
            );
        }

        /// <summary>
        ///     Assigns a service lifetime to each registration by projecting a <typeparamref name="TAttribute"/>
        ///     applied to the implementation type through <paramref name="lifetimeSelector"/>, inspecting inherited
        ///     attributes. Implementation types without the attribute fall back to
        ///     <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        /// <param name="lifetimeSelector">A function that maps the matching attribute to the service lifetime.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <example>
        ///     <code>
        ///     Classes.FromThisAssembly().BasedOn&lt;IPaymentGateway&gt;().AsInterface()
        ///         .AsLifetimeByAttribute&lt;LifestyleAttribute&gt;(attribute => attribute.Lifetime)
        ///     </code>
        /// </example>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one matching <typeparamref name="TAttribute"/>.
        /// </exception>
        public ServiceSource AsLifetimeByAttribute<TAttribute>(Func<TAttribute, ServiceLifetime> lifetimeSelector)
            where TAttribute : class
        {
            return selector.AsLifetimeByAttribute(true, lifetimeSelector);
        }

        /// <summary>
        ///     Assigns a service lifetime to each registration by projecting a <typeparamref name="TAttribute"/>
        ///     applied to the implementation type through <paramref name="lifetimeSelector"/>. Implementation types
        ///     without the attribute fall back to <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="lifetimeSelector">A function that maps the matching attribute to the service lifetime.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one matching <typeparamref name="TAttribute"/>.
        /// </exception>
        public ServiceSource AsLifetimeByAttribute<TAttribute>(
            bool inherited,
            Func<TAttribute, ServiceLifetime> lifetimeSelector
        )
            where TAttribute : class
        {
            ArgumentNullException.ThrowIfNull(lifetimeSelector);
            return selector.AsLifetime(type =>
            {
                var attribute = type.GetAttribute<TAttribute>(inherited);
                return attribute != null ? lifetimeSelector(attribute) : ServiceLifetime.Singleton;
            });
        }

        /// <summary>
        ///     Assigns a service lifetime to each registration by projecting an attribute assignable to
        ///     <paramref name="attributeType"/> applied to the implementation type through
        ///     <paramref name="lifetimeSelector"/>, inspecting inherited attributes. Implementation types without a
        ///     matching attribute fall back to <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="lifetimeSelector">A function that maps the matching attribute to the service lifetime.</param>
        /// <example>
        ///     <code>
        ///     Classes.FromThisAssembly().BasedOn&lt;IPaymentGateway&gt;().AsInterface()
        ///         .AsLifetimeByAttribute(typeof(LifestyleAttribute), attribute => ((LifestyleAttribute)attribute).Lifetime)
        ///     </code>
        /// </example>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one attribute assignable to
        ///     <paramref name="attributeType"/>.
        /// </exception>
        public ServiceSource AsLifetimeByAttribute(
            Type attributeType,
            Func<Attribute, ServiceLifetime> lifetimeSelector
        )
        {
            return selector.AsLifetimeByAttribute(attributeType, true, lifetimeSelector);
        }

        /// <summary>
        ///     Assigns a service lifetime to each registration by projecting an attribute assignable to
        ///     <paramref name="attributeType"/> applied to the implementation type through
        ///     <paramref name="lifetimeSelector"/>. Implementation types without a matching attribute fall back to
        ///     <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the implementation type; otherwise,
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="lifetimeSelector">A function that maps the matching attribute to the service lifetime.</param>
        /// <exception cref="AmbiguousMatchException">
        ///     Thrown when an implementation type has more than one attribute assignable to
        ///     <paramref name="attributeType"/>.
        /// </exception>
        public ServiceSource AsLifetimeByAttribute(
            Type attributeType,
            bool inherited,
            Func<Attribute, ServiceLifetime> lifetimeSelector
        )
        {
            ArgumentNullException.ThrowIfNull(lifetimeSelector);
            return selector.AsLifetime(type =>
            {
                var attribute = type.GetAttribute(attributeType, inherited);
                return attribute != null ? lifetimeSelector(attribute) : ServiceLifetime.Singleton;
            });
        }
    }
}
