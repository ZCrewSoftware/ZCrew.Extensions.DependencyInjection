using System.Reflection;

namespace ZCrew.Extensions.DependencyInjection;

/// <summary>
///     Extension methods on <see cref="Type"/> for namespace matching, attribute checking, and interface hierarchy
///     inspection used by the registration API.
/// </summary>
public static class TypeExtensions
{
    extension(Type type)
    {
        /// <summary>
        ///     Returns <see langword="true"/> if the type has the specified attribute applied.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type to check for.</typeparam>
        public bool HasAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            return type.GetTypeInfo().IsDefined(typeof(TAttribute));
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type has the specified attribute applied and the attribute matches
        ///     the <paramref name="filter"/> predicate.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type to check for.</typeparam>
        /// <param name="filter">A predicate to evaluate against the attribute instance.</param>
        public bool HasAttribute<TAttribute>(Func<TAttribute, bool> filter)
            where TAttribute : Attribute
        {
            var attribute = type.GetTypeInfo().GetCustomAttribute<TAttribute>();
            return attribute != null && filter(attribute);
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type is in the specified namespace (exact match).
        /// </summary>
        /// <param name="namespace">The namespace to match.</param>
        public bool IsInNamespace(string? @namespace)
        {
            return type.IsInNamespace(@namespace, false);
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type is in the specified namespace, optionally including
        ///     sub-namespaces.
        /// </summary>
        /// <param name="namespace">The namespace to match.</param>
        /// <param name="includeSubnamespaces">
        ///     <see langword="true"/> to include types in sub-namespaces.
        /// </param>
        public bool IsInNamespace(string? @namespace, bool includeSubnamespaces)
        {
            if (includeSubnamespaces)
            {
                return type.Namespace == @namespace
                    || type.Namespace != null && type.Namespace.StartsWith(@namespace + ".");
            }

            return type.Namespace == @namespace;
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type is in the same namespace as <paramref name="otherType"/>
        ///     (exact match).
        /// </summary>
        /// <param name="otherType">The type whose namespace to match.</param>
        public bool IsInSameNamespaceAs(Type otherType)
        {
            return type.IsInNamespace(otherType.Namespace);
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type is in the same namespace as <paramref name="otherType"/>,
        ///     optionally including sub-namespaces.
        /// </summary>
        /// <param name="otherType">The type whose namespace to match.</param>
        /// <param name="includeSubnamespaces">
        ///     <see langword="true"/> to include types in sub-namespaces.
        /// </param>
        public bool IsInSameNamespaceAs(Type otherType, bool includeSubnamespaces)
        {
            return type.IsInNamespace(otherType.Namespace, includeSubnamespaces);
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type is in the same namespace as <typeparamref name="T"/>
        ///     (exact match).
        /// </summary>
        /// <typeparam name="T">The type whose namespace to match.</typeparam>
        public bool IsInSameNamespaceAs<T>()
        {
            return type.IsInSameNamespaceAs(typeof(T));
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type is in the same namespace as <typeparamref name="T"/>,
        ///     optionally including sub-namespaces.
        /// </summary>
        /// <typeparam name="T">The type whose namespace to match.</typeparam>
        /// <param name="includeSubnamespaces">
        ///     <see langword="true"/> to include types in sub-namespaces.
        /// </param>
        public bool IsInSameNamespaceAs<T>(bool includeSubnamespaces)
        {
            return type.IsInSameNamespaceAs(typeof(T), includeSubnamespaces);
        }

        /// <summary>
        ///     Returns the interface name with the conventional leading <c>I</c> prefix stripped. For example,
        ///     <c>IRepository</c> returns <c>Repository</c>.
        /// </summary>
        public string GetInterfaceName()
        {
            var name = type.Name;
            if (name.Length > 1 && name[0] == 'I' && char.IsUpper(name[1]))
            {
                return name[1..];
            }
            return name;
        }

        /// <summary>
        ///     Returns the most-derived (top-level) interfaces implemented by the type, excluding interfaces that are
        ///     inherited by other interfaces the type implements.
        /// </summary>
        /// <example>
        ///     Given the following hierarchy:
        ///     <code>
        ///     interface IRepository { }
        ///     interface IUserRepository : IRepository { }
        ///     class UserRepository : IUserRepository { }
        ///     </code>
        ///     Calling <c>typeof(UserRepository).GetTopLevelInterfaces()</c> returns only
        ///     <c>IUserRepository</c>, because <c>IRepository</c> is already inherited by
        ///     <c>IUserRepository</c>.
        /// </example>
        public IEnumerable<Type> GetTopLevelInterfaces()
        {
            var interfaces = type.GetInterfaces();
            var topLevel = new List<Type>(interfaces);

            foreach (var @interface in interfaces)
            {
                foreach (var parent in @interface.GetInterfaces())
                {
                    topLevel.Remove(parent);
                }
            }

            return topLevel;
        }
    }
}
