namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceFilterExtensions
{
    extension(ServiceFilter filter)
    {
        /// <summary>
        ///     Filters to services whose implementation type is in the specified namespace.
        /// </summary>
        /// <param name="namespace">The exact namespace to match.</param>
        public ServiceFilter InNamespace(string @namespace)
        {
            ArgumentNullException.ThrowIfNull(@namespace);
            return filter.Where(service => service.ImplementationType.IsInNamespace(@namespace));
        }

        /// <summary>
        ///     Filters to services whose implementation type is in the specified namespace, optionally including
        ///     sub-namespaces.
        /// </summary>
        /// <param name="namespace">The namespace to match.</param>
        /// <param name="includeSubnamespaces">
        ///     <see langword="true"/> to include implementations in sub-namespaces.
        /// </param>
        public ServiceFilter InNamespace(string @namespace, bool includeSubnamespaces)
        {
            ArgumentNullException.ThrowIfNull(@namespace);
            return filter.Where(service => service.ImplementationType.IsInNamespace(@namespace, includeSubnamespaces));
        }

        /// <summary>
        ///     Filters to services whose implementation type is in the same namespace as <paramref name="otherType"/>.
        /// </summary>
        /// <param name="otherType">The type whose namespace to match.</param>
        public ServiceFilter InSameNamespaceAs(Type otherType)
        {
            ArgumentNullException.ThrowIfNull(otherType);
            return filter.Where(service => service.ImplementationType.IsInSameNamespaceAs(otherType));
        }

        /// <summary>
        ///     Filters to services whose implementation type is in the same namespace as <paramref name="otherType"/>,
        ///     optionally including sub-namespaces.
        /// </summary>
        /// <param name="otherType">The type whose namespace to match.</param>
        /// <param name="includeSubnamespaces">
        ///     <see langword="true"/> to include implementations in sub-namespaces.
        /// </param>
        public ServiceFilter InSameNamespaceAs(Type otherType, bool includeSubnamespaces)
        {
            ArgumentNullException.ThrowIfNull(otherType);
            return filter.Where(service =>
                service.ImplementationType.IsInSameNamespaceAs(otherType, includeSubnamespaces)
            );
        }

        /// <summary>
        ///     Filters to services whose implementation type is in the same namespace as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type whose namespace to match.</typeparam>
        public ServiceFilter InSameNamespaceAs<T>()
        {
            return filter.Where(service => service.ImplementationType.IsInSameNamespaceAs<T>());
        }

        /// <summary>
        ///     Filters to services whose implementation type is in the same namespace as <typeparamref name="T"/>,
        ///     optionally including sub-namespaces.
        /// </summary>
        /// <typeparam name="T">The type whose namespace to match.</typeparam>
        /// <param name="includeSubnamespaces">
        ///     <see langword="true"/> to include implementations in sub-namespaces.
        /// </param>
        public ServiceFilter InSameNamespaceAs<T>(bool includeSubnamespaces)
        {
            return filter.Where(service => service.ImplementationType.IsInSameNamespaceAs<T>(includeSubnamespaces));
        }
    }
}
