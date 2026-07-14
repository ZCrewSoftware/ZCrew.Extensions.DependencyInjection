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
            return filter.Where(type => type.HasAttribute(attributeType));
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
            return filter.Where(type => type.HasAttribute<TAttribute>());
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
            return filter.Where(type => type.HasAttribute(attributeType, inherited));
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
            return filter.Where(type => type.HasAttribute<TAttribute>(inherited));
        }

        /// <summary>
        ///     Filters to types decorated with an attribute assignable to <paramref name="attributeType"/>
        ///     that satisfies <paramref name="condition"/>, including inherited attributes.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="condition">The additional condition to evaluate on a matching attribute.</param>
        public TypeFilter HasAttribute(Type attributeType, Func<Attribute, bool> condition)
        {
            return filter.Where(type => type.HasAttribute(attributeType, condition));
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
            return filter.Where(type => type.HasAttribute(condition));
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
            return filter.Where(type => type.HasAttribute(attributeType, inherited, condition));
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
            return filter.Where(type => type.HasAttribute(inherited, condition));
        }

        /// <summary>
        ///     Filters to types decorated with one or more attributes assignable to <paramref name="attributeType"/>
        ///     whose set satisfies <paramref name="condition"/>, including inherited attributes.
        /// </summary>
        /// <param name="attributeType">The attribute type or marker interface to search for.</param>
        /// <param name="condition">The additional condition to evaluate on the matching attributes, if any were found.</param>
        public TypeFilter HasAttributes(Type attributeType, Func<IEnumerable<Attribute>, bool> condition)
        {
            return filter.Where(type => type.HasAttributes(attributeType, condition));
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
            return filter.Where(type => type.HasAttributes(condition));
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
            return filter.Where(type => type.HasAttributes(attributeType, inherited, condition));
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
            return filter.Where(type => type.HasAttributes(inherited, condition));
        }
    }
}
