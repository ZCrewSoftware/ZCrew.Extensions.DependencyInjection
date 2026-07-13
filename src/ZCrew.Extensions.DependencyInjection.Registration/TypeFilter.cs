namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Filters a set of types using predicates and base type constraints before service selection. This is the
///     filtering stage of the registration fluent API, analogous to Castle Windsor's <c>If</c>/<c>Where</c> and
///     <c>BasedOn</c> methods. Maintains an immutable chain: each filter method returns a new <see cref="TypeFilter"/>
///     instance. When the stage is skipped, all types pass through unfiltered.
/// </summary>
public class TypeFilter : ServiceSelector
{
    private static readonly IEnumerable<Type> DefaultBases = [typeof(object)];
    private readonly IEnumerable<Type> types;
    private readonly IEnumerable<Type> baseTypes;

    internal TypeFilter(IEnumerable<Type> types)
        : this(types, DefaultBases) { }

    // Single walk per terminal is verified by MultiEnumerationTests.
    // ReSharper disable PossibleMultipleEnumeration
    internal TypeFilter(IEnumerable<Type> types, IEnumerable<Type> baseTypes)
        : base(ApplyBases(types, baseTypes), baseTypes)
    {
        this.types = types;
        this.baseTypes = baseTypes;
    }
    // ReSharper restore PossibleMultipleEnumeration

    /// <summary>
    ///     Accepts all remaining types without further filtering and transitions to service selection.
    /// </summary>
    public ServiceSelector AllTypes()
    {
        return this;
    }

    /// <summary>
    ///     Filters types using a custom predicate. Can be chained to combine multiple filters.
    /// </summary>
    /// <param name="filter">
    ///     A predicate that returns <see langword="true"/> for types to keep.
    /// </param>
    public TypeFilter Where(Func<Type, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return new TypeFilter(this.types.Where(filter), this.baseTypes);
    }

    /// <summary>
    ///     Restricts to types that implement or inherit from any of the specified <paramref name="baseTypes"/>. Also
    ///     sets the base type context used by
    ///     <see cref="ServiceSelector.As(Func{Type, IReadOnlyList{Type}, IEnumerable{Type}})"/>.
    /// </summary>
    /// <param name="baseTypes">The base types or interfaces to filter on.</param>
    public TypeFilter BasedOn(params Type[] baseTypes)
    {
        ArgumentNullException.ThrowIfNull(baseTypes);
        if (ReferenceEquals(this.baseTypes, DefaultBases))
        {
            return new TypeFilter(this.types, baseTypes);
        }

        return new TypeFilter(this.types, this.baseTypes.Concat(baseTypes));
    }

    private static IEnumerable<Type> ApplyBases(IEnumerable<Type> types, IEnumerable<Type> baseTypes)
    {
        // Default base is object, which every type matches, so the whole set passes through untouched at no cost.
        if (ReferenceEquals(baseTypes, DefaultBases))
        {
            return types;
        }

        // Otherwise defer the filtering entirely: this runs during construction, so nothing is materialized or
        // enumerated until the chain is terminated.
        return FilterByBases(types, baseTypes);
    }

    private static IEnumerable<Type> FilterByBases(IEnumerable<Type> types, IEnumerable<Type> baseTypes)
    {
        var baseTypesArray = baseTypes as Type[] ?? baseTypes.ToArray();
        foreach (var type in types)
        {
            if (IsAssignableToAnyBase(type, baseTypesArray))
            {
                yield return type;
            }
        }
    }

    private static bool IsAssignableToAnyBase(Type type, Type[] baseTypes)
    {
        foreach (var baseType in baseTypes)
        {
            if (type.IsBasedOn(baseType))
            {
                return true;
            }
        }
        return false;
    }
}
