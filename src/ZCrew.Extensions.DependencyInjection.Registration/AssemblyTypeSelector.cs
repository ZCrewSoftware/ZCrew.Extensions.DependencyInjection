using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Selects types from an assembly with optional visibility scoping. This is the type selection stage of the
///     registration fluent API. By default, only publicly exported types are included; use
///     <see cref="IncludeInternalTypes"/> or <see cref="IncludeAllTypes"/> to broaden the scope. The assembly is not
///     scanned until the chain is terminated.
/// </summary>
[UnconditionalSuppressMessage(
    "Trimming",
    "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
    Justification = Aot.Justification
)]
public sealed class AssemblyTypeSelector : TypeFilter
{
    private readonly Assembly assembly;
    private readonly Func<Type, bool>? filter;

    internal AssemblyTypeSelector(Assembly assembly, Func<Type, bool>? filter)
        : base(ScanAndFilter(assembly, static scanned => scanned.GetExportedTypes(), filter))
    {
        this.assembly = assembly;
        this.filter = filter;
    }

    /// <summary>
    ///     Includes only publicly exported types from the assembly.
    /// </summary>
    public TypeFilter IncludePublicTypes()
    {
        return new TypeFilter(ScanAndFilter(this.assembly, static scanned => scanned.GetExportedTypes(), this.filter));
    }

    /// <summary>
    ///     Includes public and top-level internal types from the assembly.
    /// </summary>
    public TypeFilter IncludeInternalTypes()
    {
        return new TypeFilter(
            ScanAndFilter(this.assembly, static scanned => scanned.GetTypes().Where(t => t.IsPublic || t.IsNotPublic), this.filter)
        );
    }

    /// <summary>
    ///     Includes all types from the assembly regardless of visibility.
    /// </summary>
    public TypeFilter IncludeAllTypes()
    {
        return new TypeFilter(ScanAndFilter(this.assembly, static scanned => scanned.GetTypes(), this.filter));
    }

    private static IEnumerable<Type> ScanAndFilter(
        Assembly assembly,
        Func<Assembly, IEnumerable<Type>> scan,
        Func<Type, bool>? filter
    )
    {
        // Iterator body: the assembly scan (and any filter) does not run until the sequence is enumerated at the
        // terminal, keeping construction free of reflection work.
        foreach (var type in scan(assembly))
        {
            if (filter is null || filter(type))
            {
                yield return type;
            }
        }
    }
}
