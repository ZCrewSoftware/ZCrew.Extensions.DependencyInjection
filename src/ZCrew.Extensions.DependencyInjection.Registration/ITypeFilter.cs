namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Defines methods for filtering types before service selection. Filters narrow the set of types that will be
///     registered. This is the filtering stage of the registration fluent API, analogous to Castle Windsor's
///     <c>If</c>/<c>Where</c> and <c>BasedOn</c> methods. Maintains an immutable chain: each filter method returns a
///     new instance.
/// </summary>
public interface ITypeFilter : IServiceSelector
{
    /// <summary>
    ///     Accepts all remaining types without further filtering and transitions to service selection.
    /// </summary>
    IServiceSelector AllTypes();

    /// <summary>
    ///     Filters types using a custom predicate. Can be chained to combine multiple filters.
    /// </summary>
    /// <param name="filter">
    ///     A predicate that returns <see langword="true"/> for types to keep.
    /// </param>
    ITypeFilter Where(Func<Type, bool> filter);

    /// <summary>
    ///     Restricts to types that implement or inherit from any of the specified <paramref name="baseTypes"/>. Also
    ///     sets the base type context used by
    ///     <see cref="IServiceSelector.As(Func{Type, IReadOnlyList{Type}, IEnumerable{Type}})"/>.
    /// </summary>
    /// <param name="baseTypes">The base types or interfaces to filter on.</param>
    ITypeFilter BasedOn(params Type[] baseTypes);
}
