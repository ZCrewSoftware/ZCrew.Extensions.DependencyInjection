namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Filters a set of types using predicates and base type constraints, then transitions to service selection.
///     Maintains an immutable chain: each filter method returns a new <see cref="TypeFilter"/> instance.
/// </summary>
internal sealed class TypeFilter : TypeFilterBase
{
    private static readonly IEnumerable<Type> defaultBases = [typeof(object)];
    private readonly IEnumerable<Type> types;
    private readonly IEnumerable<Type> baseTypes;

    internal TypeFilter(IEnumerable<Type> types)
    {
        this.types = types;
        this.baseTypes = defaultBases;
    }

    internal TypeFilter(IEnumerable<Type> types, IEnumerable<Type> baseTypes)
    {
        this.types = types;
        this.baseTypes = baseTypes;
    }

    /// <inheritdoc />
    public override IServiceSelector AllTypes()
    {
        // If we're just checking the default (based on object) then all types will pass
        if (ReferenceEquals(this.baseTypes, defaultBases))
        {
            return new ServiceSelector(this.types, this.baseTypes);
        }

        var baseTypesArray = this.baseTypes as Type[] ?? this.baseTypes.ToArray();
        var filteredTypes = this.types.Where(type => IsAssignableToAnyBase(type, baseTypesArray));
        return new ServiceSelector(filteredTypes, baseTypesArray);
    }

    /// <inheritdoc />
    public override ITypeFilter Where(Func<Type, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return new TypeFilter(this.types.Where(filter), this.baseTypes);
    }

    /// <inheritdoc />
    public override ITypeFilter BasedOn(params Type[] baseTypes)
    {
        ArgumentNullException.ThrowIfNull(baseTypes);
        if (ReferenceEquals(this.baseTypes, defaultBases))
        {
            return new TypeFilter(this.types, baseTypes);
        }

        return new TypeFilter(this.types, this.baseTypes.Concat(baseTypes));
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
