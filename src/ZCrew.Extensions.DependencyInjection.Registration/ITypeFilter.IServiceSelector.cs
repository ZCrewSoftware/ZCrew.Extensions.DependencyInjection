namespace ZCrew.Extensions.DependencyInjection.Registration;

// Default interface implementations that bridge ITypeFilter to IServiceSelector.
// When a service selector method is called on an ITypeFilter, the implementation
// first calls AllTypes() to accept all remaining types, then delegates to the
// corresponding IServiceSelector method.
public partial interface ITypeFilter
{
    IKeyedServiceSelector IServiceSelector.As(Func<Type, IEnumerable<Type>> serviceSelector)
    {
        return AllTypes().As(serviceSelector);
    }

    IKeyedServiceSelector IServiceSelector.As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>> serviceSelector)
    {
        return AllTypes().As(serviceSelector);
    }
}
