namespace ZCrew.Extensions.DependencyInjection.Registration;

public partial interface ITypeSelector : ITypeFilter
{
    IEnumerable<Type> SelectTypes();
}
