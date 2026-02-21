namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Defines methods for filtering types before service selection. Filters narrow the set of types that will be
///     registered. This is the filtering stage of the registration fluent API, analogous to Castle Windsor's
///     <c>If</c>/<c>Where</c> and <c>BasedOn</c> methods. Maintains an immutable chain: each filter method returns a
///     new instance.
/// </summary>
public partial interface ITypeFilter : IServiceSelector
{
    /// <summary>
    ///     Accepts all remaining types without further filtering and transitions to service selection.
    /// </summary>
    IServiceSelector AllTypes();

    /// <summary>
    ///     Filters to types in the specified namespace.
    /// </summary>
    /// <param name="namespace">The exact namespace to match.</param>
    IServiceSelector InNamespace(string @namespace);

    /// <summary>
    ///     Filters to types in the specified namespace, optionally including sub-namespaces.
    /// </summary>
    /// <param name="namespace">The namespace to match.</param>
    /// <param name="includeSubnamespaces">
    ///     <see langword="true"/> to include types in sub-namespaces.
    /// </param>
    IServiceSelector InNamespace(string @namespace, bool includeSubnamespaces);

    /// <summary>
    ///     Filters to types in the same namespace as <paramref name="otherType"/>.
    /// </summary>
    /// <param name="otherType">The type whose namespace to match.</param>
    IServiceSelector InSameNamespaceAs(Type otherType);

    /// <summary>
    ///     Filters to types in the same namespace as <paramref name="otherType"/>, optionally including
    ///     sub-namespaces.
    /// </summary>
    /// <param name="otherType">The type whose namespace to match.</param>
    /// <param name="includeSubnamespaces">
    ///     <see langword="true"/> to include types in sub-namespaces.
    /// </param>
    IServiceSelector InSameNamespaceAs(Type otherType, bool includeSubnamespaces);

    /// <summary>
    ///     Filters to types in the same namespace as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type whose namespace to match.</typeparam>
    IServiceSelector InSameNamespaceAs<T>();

    /// <summary>
    ///     Filters to types in the same namespace as <typeparamref name="T"/>, optionally including sub-namespaces.
    /// </summary>
    /// <typeparam name="T">The type whose namespace to match.</typeparam>
    /// <param name="includeSubnamespaces">
    ///     <see langword="true"/> to include types in sub-namespaces.
    /// </param>
    IServiceSelector InSameNamespaceAs<T>(bool includeSubnamespaces);

    /// <summary>
    ///     Filters types using a custom predicate. Can be chained to combine multiple filters.
    /// </summary>
    /// <param name="filter">
    ///     A predicate that returns <see langword="true"/> for types to keep.
    /// </param>
    ITypeFilter Where(Func<Type, bool> filter);

    /// <summary>
    ///     Restricts to types that implement or inherit from <typeparamref name="T"/>. Also sets the base type context
    ///     used by <see cref="IServiceSelector.AsInterface()"/> and <see cref="IServiceSelector.AsBase()"/>.
    /// </summary>
    /// <typeparam name="T">The base type or interface to filter on.</typeparam>
    ITypeFilter BasedOn<T>();

    /// <summary>
    ///     Restricts to types that implement or inherit from <paramref name="baseType"/>. Also sets the base type
    ///     context used by <see cref="IServiceSelector.AsInterface()"/> and <see cref="IServiceSelector.AsBase()"/>.
    /// </summary>
    /// <param name="baseType">The base type or interface to filter on.</param>
    ITypeFilter BasedOn(Type baseType);

    /// <summary>
    ///     Restricts to types that implement or inherit from any of the specified <paramref name="baseTypes"/>. Also
    ///     sets the base type context used by <see cref="IServiceSelector.AsInterface()"/> and
    ///     <see cref="IServiceSelector.AsBase()"/>.
    /// </summary>
    /// <param name="baseTypes">The base types or interfaces to filter on.</param>
    ITypeFilter BasedOn(params Type[] baseTypes);
}
