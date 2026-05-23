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
        ///     Combines the <see cref="Type.IsAbstract"/> and <see cref="Type.IsInterface"/> to check if a type is
        ///     abstract and not an interface.
        /// </summary>
        public bool IsAbstractClass
        {
            get => type is { IsAbstract: true, IsInterface: false };
        }

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
        ///     Returns the type itself, all classes the current type extends, and all interfaces the current type
        ///     implements.
        /// </summary>
        /// <returns>The list of all types the current type represents.</returns>
        public IEnumerable<Type> GetTypes()
        {
            // Every type is itself
            yield return type;

            // And all base types
            var baseType = type.BaseType;
            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }

            // And all interfaces
            foreach (var @interface in type.GetInterfaces())
            {
                yield return @interface;
            }
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
            var topLevel = new HashSet<Type>(interfaces);

            foreach (var @interface in interfaces)
            {
                foreach (var parent in @interface.GetInterfaces())
                {
                    topLevel.Remove(parent);
                }
            }

            return topLevel;
        }

        /// <summary>
        ///     Returns the name without any generic arity.
        /// </summary>
        /// <returns>The name of the type without the generic arity.</returns>
        /// <example>
        ///     <see cref="List{T}"/> would be return the string <c>"List"</c>.
        /// </example>
        public string GetNonGenericName()
        {
            var backtick = type.Name.IndexOf('`');
            return backtick > 0 ? type.Name[..backtick] : type.Name;
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type is assignable to, inherits from, or implements
        ///     <paramref name="baseType"/>. Open generic <paramref name="baseType"/> values match any
        ///     constructed form found on the type itself, its interfaces, or its base class chain.
        /// </summary>
        /// <param name="baseType">The base type, interface, or open generic type definition to test against.</param>
        public bool IsBasedOn(Type baseType)
        {
            if (baseType.IsAssignableFrom(type))
            {
                return true;
            }

            if (!baseType.IsGenericTypeDefinition)
            {
                return false;
            }

            return type.GetGenericFormsMatching(baseType).Any();
        }

        /// <summary>
        ///     Returns the most-derived interfaces of <paramref name="type"/> based on at least one of
        ///     <paramref name="baseTypes"/>. Interfaces with unbound generic parameters collapse to their
        ///     generic type definition so the DI container can resolve them.
        /// </summary>
        /// <param name="baseTypes">The base types to match against.</param>
        public IEnumerable<Type> GetTopLevelInterfacesMatchingBaseTypes(IEnumerable<Type> baseTypes)
        {
            var matches = new HashSet<Type>();
            var baseTypeArray = baseTypes.ToArray();
            foreach (var topLevelInterface in type.GetTopLevelInterfaces())
            {
                if (!baseTypeArray.Any(baseType => topLevelInterface.IsBasedOn(baseType)))
                {
                    continue;
                }

                if (topLevelInterface.ContainsGenericParameters)
                {
                    matches.Add(topLevelInterface.GetGenericTypeDefinition());
                    continue;
                }

                matches.Add(topLevelInterface);
            }
            return matches;
        }

        /// <summary>
        ///     Returns the forms of <paramref name="baseTypes"/> that <paramref name="type"/> derives from.
        ///     Open generic bases collapse to the constructed form found on the hierarchy, or back to the
        ///     open definition when the implementing type still has unbound parameters.
        /// </summary>
        /// <param name="baseTypes">The base types to match against.</param>
        public IEnumerable<Type> GetMatchingBaseTypes(IEnumerable<Type> baseTypes)
        {
            var results = new HashSet<Type>();
            foreach (var baseType in baseTypes)
            {
                if (baseType.IsAssignableFrom(type))
                {
                    results.Add(baseType);
                    continue;
                }

                if (!baseType.IsGenericTypeDefinition)
                {
                    continue;
                }

                foreach (var candidate in type.GetGenericFormsMatching(baseType))
                {
                    results.Add(candidate.ContainsGenericParameters ? baseType : candidate);
                }
            }
            return results;
        }

        private IEnumerable<Type> GetGenericFormsMatching(Type genericDefinition)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericDefinition)
            {
                yield return type;
            }

            foreach (var @interface in type.GetInterfaces())
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == genericDefinition)
                {
                    yield return @interface;
                }
            }

            for (var current = type.BaseType; current != null; current = current.BaseType)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == genericDefinition)
                {
                    yield return current;
                }
            }
        }
    }
}
