namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class TypeFilterExtensions
{
    extension(TypeFilter filter)
    {
        /// <summary>
        ///     Filters to types decorated with an attribute assignable to <paramref name="attributeType"/>,
        ///     including inherited attributes. The type may be a concrete attribute type or an interface
        ///     implemented by one or more attributes.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        public TypeFilter HasAttribute(Type attributeType)
        {
            return filter.HasAttribute(attributeType, inherited: true);
        }

        /// <summary>
        ///     Filters to types decorated with an attribute assignable to <typeparamref name="TAttribute"/>,
        ///     including inherited attributes. <typeparamref name="TAttribute"/> may be a concrete attribute
        ///     type or an interface implemented by one or more attributes.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public TypeFilter HasAttribute<TAttribute>()
            where TAttribute : class
        {
            return filter.HasAttribute<TAttribute>(inherited: true);
        }

        /// <summary>
        ///     Filters to types decorated with an attribute assignable to <paramref name="attributeType"/>.
        ///     The type may be a concrete attribute type or an interface implemented by one or more attributes.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        public TypeFilter HasAttribute(Type attributeType, bool inherited)
        {
            ValidateAttributeType(attributeType);
            return filter.Where(type => type.GetCustomAttributes(inherited).Any(attributeType.IsInstanceOfType));
        }

        /// <summary>
        ///     Filters to types decorated with an attribute assignable to <typeparamref name="TAttribute"/>.
        ///     <typeparamref name="TAttribute"/> may be a concrete attribute type or an interface implemented
        ///     by one or more attributes.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public TypeFilter HasAttribute<TAttribute>(bool inherited)
            where TAttribute : class
        {
            ValidateAttributeType(typeof(TAttribute));
            return filter.Where(type => type.GetCustomAttributes(inherited).OfType<TAttribute>().Any());
        }

        /// <summary>
        ///     Filters to types decorated with an attribute assignable to <paramref name="attributeType"/>
        ///     that satisfies <paramref name="condition"/>, including inherited attributes.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        public TypeFilter HasAttribute(Type attributeType, Func<Attribute, bool> condition)
        {
            ArgumentNullException.ThrowIfNull(condition);
            return filter.HasAttributes(attributeType, attributes => attributes.Any(condition));
        }

        /// <summary>
        ///     Filters to types decorated with an attribute assignable to <typeparamref name="TAttribute"/>
        ///     that satisfies <paramref name="condition"/>, including inherited attributes.
        /// </summary>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public TypeFilter HasAttribute<TAttribute>(Func<TAttribute, bool> condition)
            where TAttribute : class
        {
            ArgumentNullException.ThrowIfNull(condition);
            return filter.HasAttributes<TAttribute>(attributes => attributes.Any(condition));
        }

        /// <summary>
        ///     Filters to types decorated with an attribute assignable to <paramref name="attributeType"/>
        ///     that satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        public TypeFilter HasAttribute(Type attributeType, bool inherited, Func<Attribute, bool> condition)
        {
            ArgumentNullException.ThrowIfNull(condition);
            return filter.HasAttributes(attributeType, inherited, attributes => attributes.Any(condition));
        }

        /// <summary>
        ///     Filters to types decorated with an attribute assignable to <typeparamref name="TAttribute"/>
        ///     that satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public TypeFilter HasAttribute<TAttribute>(bool inherited, Func<TAttribute, bool> condition)
            where TAttribute : class
        {
            ArgumentNullException.ThrowIfNull(condition);
            return filter.HasAttributes<TAttribute>(inherited, attributes => attributes.Any(condition));
        }

        /// <summary>
        ///     Filters to types decorated with one or more attributes assignable to <paramref name="attributeType"/>
        ///     whose set satisfies <paramref name="condition"/>, including inherited attributes.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="condition">The additional condition to evaluate on the matching attributes, if any were found.</param>
        public TypeFilter HasAttributes(Type attributeType, Func<IEnumerable<Attribute>, bool> condition)
        {
            return filter.HasAttributes(attributeType, inherited: true, condition);
        }

        /// <summary>
        ///     Filters to types decorated with one or more attributes assignable to <typeparamref name="TAttribute"/>
        ///     whose set satisfies <paramref name="condition"/>, including inherited attributes.
        /// </summary>
        /// <param name="condition">The additional condition to evaluate on the matching attributes, if any were found.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public TypeFilter HasAttributes<TAttribute>(Func<IEnumerable<TAttribute>, bool> condition)
            where TAttribute : class
        {
            return filter.HasAttributes(inherited: true, condition);
        }

        /// <summary>
        ///     Filters to types decorated with one or more attributes assignable to <paramref name="attributeType"/>
        ///     whose set satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="condition">The additional condition to evaluate on the matching attributes, if any were found.</param>
        public TypeFilter HasAttributes(Type attributeType, bool inherited, Func<IEnumerable<Attribute>, bool> condition)
        {
            ValidateAttributeType(attributeType);
            ArgumentNullException.ThrowIfNull(condition);
            return filter.Where(type =>
            {
                var attributes = type.GetCustomAttributes(inherited)
                    .Cast<Attribute>()
                    .Where(attributeType.IsInstanceOfType)
                    .ToArray();
                return attributes.Length > 0 && condition(attributes);
            });
        }

        /// <summary>
        ///     Filters to types decorated with one or more attributes assignable to <typeparamref name="TAttribute"/>
        ///     whose set satisfies <paramref name="condition"/>.
        /// </summary>
        /// <param name="inherited">
        ///     <see langword="true"/> to inspect the ancestors of the type; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="condition">The additional condition to evaluate on the matching attributes, if any were found.</param>
        /// <typeparam name="TAttribute">The attribute type or marker interface to search for.</typeparam>
        public TypeFilter HasAttributes<TAttribute>(bool inherited, Func<IEnumerable<TAttribute>, bool> condition)
            where TAttribute : class
        {
            ValidateAttributeType(typeof(TAttribute));
            ArgumentNullException.ThrowIfNull(condition);
            return filter.Where(type =>
            {
                var attributes = type.GetCustomAttributes(inherited).OfType<TAttribute>().ToArray();
                return attributes.Length > 0 && condition(attributes);
            });
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
}
