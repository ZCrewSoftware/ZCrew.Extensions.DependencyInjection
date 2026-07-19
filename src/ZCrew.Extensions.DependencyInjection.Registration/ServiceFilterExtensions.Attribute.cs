namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceFilterExtensions
{
    extension(ServiceFilter filter)
    {
        /// <summary>
        ///     Filters to services whose implementation is decorated with an attribute assignable to
        ///     <paramref name="attributeType"/>, including inherited attributes. The type may be a concrete attribute
        ///     type or an interface implemented by one or more attributes.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        public ServiceFilter HasAttribute(Type attributeType)
        {
            return filter.Where(service => service.ImplementationType.HasAttribute(attributeType));
        }

        /// <summary>
        ///     Filters to services whose implementation is decorated with an attribute assignable to
        ///     <typeparamref name="TAttribute"/>, including inherited attributes. <typeparamref name="TAttribute"/> may
        ///     be a concrete attribute type or an interface implemented by one or more attributes.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public ServiceFilter HasAttribute<TAttribute>()
            where TAttribute : class
        {
            return filter.Where(service => service.ImplementationType.HasAttribute<TAttribute>());
        }

        /// <summary>
        ///     Filters to services whose implementation is decorated with an attribute assignable to
        ///     <paramref name="attributeType"/>. The type may be a concrete attribute type or an interface implemented
        ///     by one or more attributes.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        public ServiceFilter HasAttribute(Type attributeType, bool inherited)
        {
            return filter.Where(service => service.ImplementationType.HasAttribute(attributeType, inherited));
        }

        /// <summary>
        ///     Filters to services whose implementation is decorated with an attribute assignable to
        ///     <typeparamref name="TAttribute"/>. <typeparamref name="TAttribute"/> may be a concrete attribute type or
        ///     an interface implemented by one or more attributes.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public ServiceFilter HasAttribute<TAttribute>(bool inherited)
            where TAttribute : class
        {
            return filter.Where(service => service.ImplementationType.HasAttribute<TAttribute>(inherited));
        }

        /// <summary>
        ///     Filters to services whose implementation is decorated with an attribute assignable to
        ///     <paramref name="attributeType"/> that satisfies <paramref name="condition"/>, including inherited
        ///     attributes.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        public ServiceFilter HasAttribute(Type attributeType, Func<Attribute, bool> condition)
        {
            return filter.Where(service => service.ImplementationType.HasAttribute(attributeType, condition));
        }

        /// <summary>
        ///     Filters to services whose implementation is decorated with an attribute assignable to
        ///     <typeparamref name="TAttribute"/> that satisfies <paramref name="condition"/>, including inherited
        ///     attributes.
        /// </summary>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public ServiceFilter HasAttribute<TAttribute>(Func<TAttribute, bool> condition)
            where TAttribute : class
        {
            return filter.Where(service => service.ImplementationType.HasAttribute(condition));
        }

        /// <summary>
        ///     Filters to services whose implementation is decorated with an attribute assignable to
        ///     <paramref name="attributeType"/> that satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        public ServiceFilter HasAttribute(Type attributeType, bool inherited, Func<Attribute, bool> condition)
        {
            return filter.Where(service =>
                service.ImplementationType.HasAttribute(attributeType, inherited, condition)
            );
        }

        /// <summary>
        ///     Filters to services whose implementation is decorated with an attribute assignable to
        ///     <typeparamref name="TAttribute"/> that satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public ServiceFilter HasAttribute<TAttribute>(bool inherited, Func<TAttribute, bool> condition)
            where TAttribute : class
        {
            return filter.Where(service => service.ImplementationType.HasAttribute(inherited, condition));
        }

        /// <summary>
        ///     Filters to services whose implementation is decorated with one or more attributes assignable to
        ///     <paramref name="attributeType"/> whose set satisfies <paramref name="condition"/>, including inherited
        ///     attributes.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="condition">
        ///     The additional condition to evaluate on the matching attributes, if any were found.
        /// </param>
        public ServiceFilter HasAttributes(Type attributeType, Func<IEnumerable<Attribute>, bool> condition)
        {
            return filter.Where(service => service.ImplementationType.HasAttributes(attributeType, condition));
        }

        /// <summary>
        ///     Filters to services whose implementation is decorated with one or more attributes assignable to
        ///     <typeparamref name="TAttribute"/> whose set satisfies <paramref name="condition"/>, including inherited
        ///     attributes.
        /// </summary>
        /// <param name="condition">
        ///     The additional condition to evaluate on the matching attributes, if any were found.
        /// </param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public ServiceFilter HasAttributes<TAttribute>(Func<IEnumerable<TAttribute>, bool> condition)
            where TAttribute : class
        {
            return filter.Where(service => service.ImplementationType.HasAttributes(condition));
        }

        /// <summary>
        ///     Filters to services whose implementation is decorated with one or more attributes assignable to
        ///     <paramref name="attributeType"/> whose set satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="condition">
        ///     The additional condition to evaluate on the matching attributes, if any were found.
        /// </param>
        public ServiceFilter HasAttributes(
            Type attributeType,
            bool inherited,
            Func<IEnumerable<Attribute>, bool> condition
        )
        {
            return filter.Where(service =>
                service.ImplementationType.HasAttributes(attributeType, inherited, condition)
            );
        }

        /// <summary>
        ///     Filters to services whose implementation is decorated with one or more attributes assignable to
        ///     <typeparamref name="TAttribute"/> whose set satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="condition">
        ///     The additional condition to evaluate on the matching attributes, if any were found.
        /// </param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public ServiceFilter HasAttributes<TAttribute>(bool inherited, Func<IEnumerable<TAttribute>, bool> condition)
            where TAttribute : class
        {
            return filter.Where(service => service.ImplementationType.HasAttributes(inherited, condition));
        }
    }
}
