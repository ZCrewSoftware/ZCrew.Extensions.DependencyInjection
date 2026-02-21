using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Filters a set of types using predicates and base type constraints, then transitions to service selection.
///     Maintains an immutable chain: each filter method returns a new <see cref="TypeFilter"/> instance.
/// </summary>
public sealed class TypeFilter : ServiceSource, ITypeFilter
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
    public IServiceSelector AllTypes()
    {
        return new ServiceSelector(this.types, this.baseTypes);
    }

    /// <inheritdoc />
    public IServiceSelector InNamespace(string @namespace)
    {
        ArgumentNullException.ThrowIfNull(@namespace);
        return Where(type => type.IsInNamespace(@namespace));
    }

    /// <inheritdoc />
    public IServiceSelector InNamespace(string @namespace, bool includeSubnamespaces)
    {
        ArgumentNullException.ThrowIfNull(@namespace);
        return Where(type => type.IsInNamespace(@namespace, includeSubnamespaces));
    }

    /// <inheritdoc />
    public IServiceSelector InSameNamespaceAs(Type otherType)
    {
        ArgumentNullException.ThrowIfNull(otherType);
        return Where(type => type.IsInSameNamespaceAs(otherType));
    }

    /// <inheritdoc />
    public IServiceSelector InSameNamespaceAs(Type otherType, bool includeSubnamespaces)
    {
        ArgumentNullException.ThrowIfNull(otherType);
        return Where(type => type.IsInSameNamespaceAs(otherType, includeSubnamespaces));
    }

    /// <inheritdoc />
    public IServiceSelector InSameNamespaceAs<T>()
    {
        return Where(type => type.IsInSameNamespaceAs<T>());
    }

    /// <inheritdoc />
    public IServiceSelector InSameNamespaceAs<T>(bool includeSubnamespaces)
    {
        return Where(type => type.IsInSameNamespaceAs<T>(includeSubnamespaces));
    }

    /// <inheritdoc />
    public ITypeFilter Where(Func<Type, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return new TypeFilter(this.types.Where(filter), this.baseTypes);
    }

    /// <inheritdoc />
    public ITypeFilter BasedOn<T>()
    {
        return BasedOn([typeof(T)]);
    }

    /// <inheritdoc />
    public ITypeFilter BasedOn(Type baseType)
    {
        ArgumentNullException.ThrowIfNull(baseType);
        return BasedOn([baseType]);
    }

    /// <inheritdoc />
    public ITypeFilter BasedOn(params Type[] baseTypes)
    {
        ArgumentNullException.ThrowIfNull(baseTypes);
        if (ReferenceEquals(this.baseTypes, defaultBases))
        {
            return new TypeFilter(this.types, baseTypes);
        }

        return new TypeFilter(this.types, this.baseTypes.Concat(baseTypes));
    }

    /// <inheritdoc />
    protected override IEnumerable<ServiceDescriptor> SelectServices()
    {
        return this.types.Select(type => new ServiceDescriptor(type, type, ServiceLifetime.Singleton));
    }
}
