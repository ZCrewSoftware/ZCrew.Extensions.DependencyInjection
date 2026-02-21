namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Extends <see cref="ITypeSelector"/> with methods for controlling which types from an assembly are included
///     based on their visibility.
/// </summary>
public interface IAssemblyTypeSelector : ITypeSelector
{
    /// <summary>
    ///     Includes only publicly exported types from the assembly.
    /// </summary>
    ITypeSelector IncludePublicTypes();

    /// <summary>
    ///     Includes public and top-level internal types from the assembly.
    /// </summary>
    ITypeSelector IncludeInternalTypes();

    /// <summary>
    ///     Includes all types from the assembly regardless of visibility.
    /// </summary>
    ITypeSelector IncludeAllTypes();
}
