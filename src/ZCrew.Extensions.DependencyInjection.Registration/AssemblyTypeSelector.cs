using System.Reflection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Selects types from an assembly with optional visibility scoping. By default, only publicly exported types
///     are included; use <see cref="IncludeInternalTypes"/> or <see cref="IncludeAllTypes"/> to broaden the scope.
/// </summary>
public sealed class AssemblyTypeSelector : TypeSelectorBase, IAssemblyTypeSelector
{
    private readonly Assembly assembly;
    private readonly Func<Type, bool>? filter;

    internal AssemblyTypeSelector(Assembly assembly)
    {
        this.assembly = assembly;
        this.filter = null;
    }

    internal AssemblyTypeSelector(Assembly assembly, Func<Type, bool>? filter)
    {
        this.assembly = assembly;
        this.filter = filter;
    }

    /// <inheritdoc />
    public ITypeSelector IncludePublicTypes()
    {
        return new EnumerableTypeSelector(this.assembly.GetExportedTypes(), this.filter);
    }

    /// <inheritdoc />
    public ITypeSelector IncludeInternalTypes()
    {
        return new EnumerableTypeSelector(this.assembly.GetTypes().Where(t => t.IsPublic || t.IsNotPublic), this.filter);
    }

    /// <inheritdoc />
    public ITypeSelector IncludeAllTypes()
    {
        return new EnumerableTypeSelector(this.assembly.GetTypes(), this.filter);
    }

    /// <inheritdoc />
    public override IEnumerable<Type> SelectTypes()
    {
        if (this.filter != null)
        {
            return this.assembly.GetExportedTypes().Where(this.filter);
        }

        return this.assembly.GetExportedTypes();
    }
}
