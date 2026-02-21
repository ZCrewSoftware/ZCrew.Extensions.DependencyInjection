namespace ZCrew.Extensions.DependencyInjection.Registration;

public interface IAssemblyTypeSelector : ITypeSelector
{
    ITypeSelector IncludePublicTypes();
    ITypeSelector IncludeInternalTypes();
    ITypeSelector IncludeAllTypes();
}
