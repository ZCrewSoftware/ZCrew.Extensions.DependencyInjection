using System.Reflection;

namespace ZCrew.Extensions.DependencyInjection;

public static partial class TypeExtensions
{
    extension(Type type)
    {
        /// <summary>
        ///     Returns <see langword="true"/> if the type has the specified attribute applied.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        public bool HasAttribute(Type attributeType)
        {
            return type.HasAttribute(attributeType, inherited: true);
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type has the specified attribute applied.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public bool HasAttribute<TAttribute>()
            where TAttribute : class
        {
            return type.HasAttribute(typeof(TAttribute), inherited: true);
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type has the specified attribute applied.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        public bool HasAttribute(Type attributeType, bool inherited)
        {
            ArgumentNullException.ThrowIfNull(attributeType);
            if (attributeType == typeof(Attribute) || attributeType.IsSubclassOf(typeof(Attribute)))
            {
                return type.IsDefined(attributeType, inherited);
            }
            ValidateInterface(attributeType);

            return type.GetCustomAttributes(inherited).Any(attributeType.IsInstanceOfType);
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type has the specified attribute applied.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public bool HasAttribute<TAttribute>(bool inherited)
            where TAttribute : class
        {
            return type.HasAttribute(typeof(TAttribute), inherited);
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type has an attribute assignable to
        ///     <paramref name="attributeType"/> that satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        public bool HasAttribute(Type attributeType, Func<Attribute, bool> condition)
        {
            ArgumentNullException.ThrowIfNull(condition);
            return type.HasAttributes(attributeType, attributes => attributes.Any(condition));
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type has an attribute assignable to
        ///     <typeparamref name="TAttribute"/> that satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public bool HasAttribute<TAttribute>(Func<TAttribute, bool> condition)
            where TAttribute : class
        {
            ArgumentNullException.ThrowIfNull(condition);
            return type.HasAttributes<TAttribute>(attributes => attributes.Any(condition));
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type has an attribute assignable to
        ///     <paramref name="attributeType"/> that satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        public bool HasAttribute(Type attributeType, bool inherited, Func<Attribute, bool> condition)
        {
            ArgumentNullException.ThrowIfNull(condition);
            return type.HasAttributes(attributeType, inherited, attributes => attributes.Any(condition));
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type has an attribute assignable to
        ///     <typeparamref name="TAttribute"/> that satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public bool HasAttribute<TAttribute>(bool inherited, Func<TAttribute, bool> condition)
            where TAttribute : class
        {
            ArgumentNullException.ThrowIfNull(condition);
            return type.HasAttributes<TAttribute>(inherited, attributes => attributes.Any(condition));
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type has one or more attributes assignable to
        ///     <paramref name="attributeType"/> whose set satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="condition">The additional condition to evaluate on the matching attributes, if any were found.</param>
        public bool HasAttributes(Type attributeType, Func<IEnumerable<Attribute>, bool> condition)
        {
            return type.HasAttributes(attributeType, inherited: true, condition);
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type has one or more attributes assignable to
        ///     <typeparamref name="TAttribute"/> whose set satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="condition">The additional condition to evaluate on the matching attributes, if any were found.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public bool HasAttributes<TAttribute>(Func<IEnumerable<TAttribute>, bool> condition)
            where TAttribute : class
        {
            return type.HasAttributes(inherited: true, condition);
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type has one or more attributes assignable to
        ///     <paramref name="attributeType"/> whose set satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="condition">The additional condition to evaluate on the matching attributes, if any were found.</param>
        public bool HasAttributes(Type attributeType, bool inherited, Func<IEnumerable<Attribute>, bool> condition)
        {
            ValidateAttributeType(attributeType);
            ArgumentNullException.ThrowIfNull(condition);
            var attributes = type.GetCustomAttributes(inherited)
                .Cast<Attribute>()
                .Where(attributeType.IsInstanceOfType)
                .ToArray();
            return attributes.Length > 0 && condition(attributes);
        }

        /// <summary>
        ///     Returns <see langword="true"/> if the type has one or more attributes assignable to
        ///     <typeparamref name="TAttribute"/> whose set satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="condition">The additional condition to evaluate on the matching attributes, if any were found.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public bool HasAttributes<TAttribute>(bool inherited, Func<IEnumerable<TAttribute>, bool> condition)
            where TAttribute : class
        {
            ValidateAttributeType(typeof(TAttribute));
            ArgumentNullException.ThrowIfNull(condition);
            var attributes = type.GetCustomAttributes(inherited).OfType<TAttribute>().ToArray();
            return attributes.Length > 0 && condition(attributes);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <paramref name="attributeType"/> applied to the type, or
        ///     <see langword="null"/> if none is found.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        public Attribute? GetAttribute(Type attributeType)
        {
            return type.GetAttribute(attributeType, inherited: true);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <typeparamref name="TAttribute"/> applied to the type, or
        ///     <see langword="null"/> if none is found.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        public TAttribute? GetAttribute<TAttribute>()
            where TAttribute : class
        {
            return type.GetAttribute<TAttribute>(inherited: true, _ => true);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <paramref name="attributeType"/> applied to the type, or
        ///     <see langword="null"/> if none is found.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        public Attribute? GetAttribute(Type attributeType, bool inherited)
        {
            return type.GetAttribute(attributeType, inherited, _ => true);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <typeparamref name="TAttribute"/> applied to the type, or
        ///     <see langword="null"/> if none is found.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        public TAttribute? GetAttribute<TAttribute>(bool inherited)
            where TAttribute : class
        {
            return type.GetAttribute<TAttribute>(inherited, _ => true);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <paramref name="attributeType"/> that satisfies
        ///     <paramref name="filter"/>, or <see langword="null"/> if none is found.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="filter">The additional condition to evaluate on a matching attribute.</param>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        public Attribute? GetAttribute(Type attributeType, Func<Attribute, bool> filter)
        {
            return type.GetAttribute(attributeType, inherited: true, filter);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <typeparamref name="TAttribute"/> that satisfies
        ///     <paramref name="filter"/>, or <see langword="null"/> if none is found.
        /// </summary>
        /// <param name="filter">The additional condition to evaluate on a matching attribute.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        public TAttribute? GetAttribute<TAttribute>(Func<TAttribute, bool> filter)
            where TAttribute : class
        {
            return type.GetAttribute(inherited: true, filter);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <paramref name="attributeType"/> that satisfies
        ///     <paramref name="filter"/>, or <see langword="null"/> if none is found.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="filter">The additional condition to evaluate on a matching attribute.</param>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        public Attribute? GetAttribute(Type attributeType, bool inherited, Func<Attribute, bool> filter)
        {
            if (attributeType == typeof(Attribute) || attributeType.IsSubclassOf(typeof(Attribute)))
            {
                var matches = type.GetCustomAttributes(attributeType, inherited).Cast<Attribute>().Where(filter).ToArray();
                return GetSingle(matches);
            }
            ValidateInterface(attributeType);
            var attributes = type.GetCustomAttributes(inherited)
                .Cast<Attribute>()
                .Where(attributeType.IsInstanceOfType)
                .Where(filter)
                .ToArray();
            return GetSingle(attributes);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <typeparamref name="TAttribute"/> that satisfies
        ///     <paramref name="filter"/>, or <see langword="null"/> if none is found.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="filter">The additional condition to evaluate on a matching attribute.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        public TAttribute? GetAttribute<TAttribute>(bool inherited, Func<TAttribute, bool> filter)
            where TAttribute : class
        {
            var attributeType = typeof(TAttribute);
            if (attributeType == typeof(Attribute) || attributeType.IsSubclassOf(typeof(Attribute)))
            {
                var matches = type.GetCustomAttributes(attributeType, inherited).OfType<TAttribute>().Where(filter).ToArray();
                return GetSingle(matches);
            }
            ValidateInterface(attributeType);
            var attributes = type.GetCustomAttributes(inherited)
                .OfType<TAttribute>()
                .Where(filter)
                .ToArray();
            return GetSingle(attributes);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <paramref name="attributeType"/> applied to the type.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no matching attribute is found.</exception>
        public Attribute GetRequiredAttribute(Type attributeType)
        {
            return type.GetAttribute(attributeType) ?? throw AttributeNotFound(type, attributeType);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <typeparamref name="TAttribute"/> applied to the type.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no matching attribute is found.</exception>
        public TAttribute GetRequiredAttribute<TAttribute>()
            where TAttribute : class
        {
            return type.GetAttribute<TAttribute>() ?? throw AttributeNotFound(type, typeof(TAttribute));
        }

        /// <summary>
        ///     Returns the single attribute assignable to <paramref name="attributeType"/> applied to the type.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no matching attribute is found.</exception>
        public Attribute GetRequiredAttribute(Type attributeType, bool inherited)
        {
            return type.GetAttribute(attributeType, inherited) ?? throw AttributeNotFound(type, attributeType);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <typeparamref name="TAttribute"/> applied to the type.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no matching attribute is found.</exception>
        public TAttribute GetRequiredAttribute<TAttribute>(bool inherited)
            where TAttribute : class
        {
            return type.GetAttribute<TAttribute>(inherited) ?? throw AttributeNotFound(type, typeof(TAttribute));
        }

        /// <summary>
        ///     Returns the single attribute assignable to <paramref name="attributeType"/> that satisfies
        ///     <paramref name="filter"/>.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="filter">The additional condition to evaluate on a matching attribute.</param>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no matching attribute is found.</exception>
        public Attribute GetRequiredAttribute(Type attributeType, Func<Attribute, bool> filter)
        {
            return type.GetAttribute(attributeType, filter) ?? throw AttributeNotFound(type, attributeType);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <typeparamref name="TAttribute"/> that satisfies
        ///     <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">The additional condition to evaluate on a matching attribute.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no matching attribute is found.</exception>
        public TAttribute GetRequiredAttribute<TAttribute>(Func<TAttribute, bool> filter)
            where TAttribute : class
        {
            return type.GetAttribute(filter) ?? throw AttributeNotFound(type, typeof(TAttribute));
        }

        /// <summary>
        ///     Returns the single attribute assignable to <paramref name="attributeType"/> that satisfies
        ///     <paramref name="filter"/>.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="filter">The additional condition to evaluate on a matching attribute.</param>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no matching attribute is found.</exception>
        public Attribute GetRequiredAttribute(Type attributeType, bool inherited, Func<Attribute, bool> filter)
        {
            return type.GetAttribute(attributeType, inherited, filter)
                ?? throw AttributeNotFound(type, attributeType);
        }

        /// <summary>
        ///     Returns the single attribute assignable to <typeparamref name="TAttribute"/> that satisfies
        ///     <paramref name="filter"/>.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="filter">The additional condition to evaluate on a matching attribute.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one attribute matches.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no matching attribute is found.</exception>
        public TAttribute GetRequiredAttribute<TAttribute>(bool inherited, Func<TAttribute, bool> filter)
            where TAttribute : class
        {
            return type.GetAttribute(inherited, filter) ?? throw AttributeNotFound(type, typeof(TAttribute));
        }

        /// <summary>
        ///     Returns all attributes assignable to <paramref name="attributeType"/> applied to the type, or an
        ///     empty sequence if none are found.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        public IEnumerable<Attribute> GetAttributes(Type attributeType)
        {
            return type.GetAttributes(attributeType, inherited: true);
        }

        /// <summary>
        ///     Returns all attributes assignable to <typeparamref name="TAttribute"/> applied to the type, or an
        ///     empty sequence if none are found.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public IEnumerable<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : class
        {
            return type.GetAttributes<TAttribute>(inherited: true, _ => true);
        }

        /// <summary>
        ///     Returns all attributes assignable to <paramref name="attributeType"/> applied to the type, or an
        ///     empty sequence if none are found.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        public IEnumerable<Attribute> GetAttributes(Type attributeType, bool inherited)
        {
            return type.GetAttributes(attributeType, inherited, _ => true);
        }

        /// <summary>
        ///     Returns all attributes assignable to <typeparamref name="TAttribute"/> applied to the type, or an
        ///     empty sequence if none are found.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public IEnumerable<TAttribute> GetAttributes<TAttribute>(bool inherited)
            where TAttribute : class
        {
            return type.GetAttributes<TAttribute>(inherited, _ => true);
        }

        /// <summary>
        ///     Returns all attributes assignable to <paramref name="attributeType"/> that satisfy
        ///     <paramref name="filter"/>, or an empty sequence if none are found.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="filter">The additional condition to evaluate on each matching attribute.</param>
        public IEnumerable<Attribute> GetAttributes(Type attributeType, Func<Attribute, bool> filter)
        {
            return type.GetAttributes(attributeType, inherited: true, filter);
        }

        /// <summary>
        ///     Returns all attributes assignable to <typeparamref name="TAttribute"/> that satisfy
        ///     <paramref name="filter"/>, or an empty sequence if none are found.
        /// </summary>
        /// <param name="filter">The additional condition to evaluate on each matching attribute.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public IEnumerable<TAttribute> GetAttributes<TAttribute>(Func<TAttribute, bool> filter)
            where TAttribute : class
        {
            return type.GetAttributes(inherited: true, filter);
        }

        /// <summary>
        ///     Returns all attributes assignable to <paramref name="attributeType"/> that satisfy
        ///     <paramref name="filter"/>, or an empty sequence if none are found.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="filter">The additional condition to evaluate on each matching attribute.</param>
        public IEnumerable<Attribute> GetAttributes(Type attributeType, bool inherited, Func<Attribute, bool> filter)
        {
            ValidateAttributeType(attributeType);
            ArgumentNullException.ThrowIfNull(filter);
            return type.GetCustomAttributes(inherited)
                .Cast<Attribute>()
                .Where(attributeType.IsInstanceOfType)
                .Where(filter)
                .ToArray();
        }

        /// <summary>
        ///     Returns all attributes assignable to <typeparamref name="TAttribute"/> that satisfy
        ///     <paramref name="filter"/>, or an empty sequence if none are found.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="filter">The additional condition to evaluate on each matching attribute.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public IEnumerable<TAttribute> GetAttributes<TAttribute>(bool inherited, Func<TAttribute, bool> filter)
            where TAttribute : class
        {
            ValidateAttributeType(typeof(TAttribute));
            ArgumentNullException.ThrowIfNull(filter);
            return type.GetCustomAttributes(inherited).OfType<TAttribute>().Where(filter).ToArray();
        }
    }

    /// <summary>
    ///     Ensures a requested attribute type can actually match a custom attribute: it must be an attribute
    ///     type or an interface an attribute could implement. Anything else (e.g. <see cref="string"/>) can
    ///     never match, so it is rejected rather than silently filtering everything out.
    /// </summary>
    private static void ValidateAttributeType(Type attributeType)
    {
        ArgumentNullException.ThrowIfNull(attributeType);
        if (
            !attributeType.IsInterface
            && !attributeType.IsSubclassOf(typeof(Attribute))
            && attributeType != typeof(Attribute)
        )
        {
            throw new ArgumentException(
                $"'{attributeType}' must be an attribute type or an interface.",
                nameof(attributeType)
            );
        }
    }

    /// <summary>
    ///     Similar to <see cref="ValidateAttributeType"/> but after the <see cref="Attribute"/> checks have already
    ///     been done for a fast-path.
    /// </summary>
    private static void ValidateInterface(Type attributeType)
    {
        ArgumentNullException.ThrowIfNull(attributeType);
        if (!attributeType.IsInterface)
        {
            throw new ArgumentException(
                $"'{attributeType}' must be an attribute type or an interface.",
                nameof(attributeType)
            );
        }
    }

    private static TAttribute? GetSingle<TAttribute>(IReadOnlyList<TAttribute> attributes)
    {
        if (attributes.Count > 1)
        {
            throw new AmbiguousMatchException($"Ambiguous match found for {typeof(TAttribute).FullName}");
        }
        return attributes.Count == 0 ? default : attributes[0];
    }

    private static InvalidOperationException AttributeNotFound(Type type, Type attributeType)
    {
        return new InvalidOperationException(
            $"No attribute of type '{attributeType.FullName}' was found on '{type.FullName}'."
        );
    }
}
