namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class TypeFilterExtensions
{
    extension(TypeFilter filter)
    {
        /// <summary>
        ///     Filters to types in the specified namespace.
        /// </summary>
        /// <param name="namespace">The exact namespace to match.</param>
        public ServiceSelector InNamespace(string @namespace)
        {
            ArgumentNullException.ThrowIfNull(@namespace);
            return filter.Where(type => type.IsInNamespace(@namespace));
        }

        /// <summary>
        ///     Filters to types in the specified namespace, optionally including sub-namespaces.
        /// </summary>
        /// <param name="namespace">The namespace to match.</param>
        /// <param name="includeSubnamespaces">
        ///     <see langword="true"/> to include types in sub-namespaces.
        /// </param>
        public ServiceSelector InNamespace(string @namespace, bool includeSubnamespaces)
        {
            ArgumentNullException.ThrowIfNull(@namespace);
            return filter.Where(type => type.IsInNamespace(@namespace, includeSubnamespaces));
        }

        /// <summary>
        ///     Filters to types in the same namespace as <paramref name="otherType"/>.
        /// </summary>
        /// <param name="otherType">The type whose namespace to match.</param>
        public ServiceSelector InSameNamespaceAs(Type otherType)
        {
            ArgumentNullException.ThrowIfNull(otherType);
            return filter.Where(type => type.IsInSameNamespaceAs(otherType));
        }

        /// <summary>
        ///     Filters to types in the same namespace as <paramref name="otherType"/>, optionally including
        ///     sub-namespaces.
        /// </summary>
        /// <param name="otherType">The type whose namespace to match.</param>
        /// <param name="includeSubnamespaces">
        ///     <see langword="true"/> to include types in sub-namespaces.
        /// </param>
        public ServiceSelector InSameNamespaceAs(Type otherType, bool includeSubnamespaces)
        {
            ArgumentNullException.ThrowIfNull(otherType);
            return filter.Where(type => type.IsInSameNamespaceAs(otherType, includeSubnamespaces));
        }

        /// <summary>
        ///     Filters to types in the same namespace as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type whose namespace to match.</typeparam>
        public ServiceSelector InSameNamespaceAs<T>()
        {
            return filter.Where(type => type.IsInSameNamespaceAs<T>());
        }

        /// <summary>
        ///     Filters to types in the same namespace as <typeparamref name="T"/>, optionally including sub-namespaces.
        /// </summary>
        /// <typeparam name="T">The type whose namespace to match.</typeparam>
        /// <param name="includeSubnamespaces">
        ///     <see langword="true"/> to include types in sub-namespaces.
        /// </param>
        public ServiceSelector InSameNamespaceAs<T>(bool includeSubnamespaces)
        {
            return filter.Where(type => type.IsInSameNamespaceAs<T>(includeSubnamespaces));
        }
    }
}
