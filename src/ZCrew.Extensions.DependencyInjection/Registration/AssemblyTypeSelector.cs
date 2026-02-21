using System.Reflection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

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

    public ITypeSelector IncludePublicTypes()
    {
        return new EnumerableTypeSelector(this.assembly.GetExportedTypes(), this.filter);
    }

    public ITypeSelector IncludeInternalTypes()
    {
        return new EnumerableTypeSelector(this.assembly.GetTypes().Where(t => t.IsPublic || t.IsNotPublic), this.filter);
    }

    public ITypeSelector IncludeAllTypes()
    {
        return new EnumerableTypeSelector(this.assembly.GetTypes(), this.filter);
    }

    public override IEnumerable<Type> SelectTypes()
    {
        if (this.filter != null)
        {
            return this.assembly.GetExportedTypes().Where(this.filter);
        }

        return this.assembly.GetExportedTypes();
    }
}
