namespace ZCrew.Extensions.DependencyInjection.Registration;

// Default interface implementations that bridge ITypeFilter to IServiceSelector.
// When a service selector method is called on an ITypeFilter, the implementation
// first calls AllTypes() to accept all remaining types, then delegates to the
// corresponding IServiceSelector method.
public partial interface ITypeFilter
{
    IKeyedServiceSelector IServiceSelector.AsAllInterfaces()
    {
        return AllTypes().AsAllInterfaces();
    }

    IKeyedServiceSelector IServiceSelector.AsAllNonSystemInterfaces()
    {
        return AllTypes().AsAllNonSystemInterfaces();
    }

    IKeyedServiceSelector IServiceSelector.AsDefaultInterfaces()
    {
        return AllTypes().AsDefaultInterfaces();
    }

    IKeyedServiceSelector IServiceSelector.AsDefaultNonSystemInterfaces()
    {
        return AllTypes().AsDefaultNonSystemInterfaces();
    }

    IKeyedServiceSelector IServiceSelector.AsFirstInterface()
    {
        return AllTypes().AsFirstInterface();
    }

    IKeyedServiceSelector IServiceSelector.AsInterface()
    {
        return AllTypes().AsInterface();
    }

    IKeyedServiceSelector IServiceSelector.AsInterface<T>()
    {
        return AllTypes().AsInterface<T>();
    }

    IKeyedServiceSelector IServiceSelector.AsInterface(Type interfaceType)
    {
        return AllTypes().AsInterface(interfaceType);
    }

    IKeyedServiceSelector IServiceSelector.AsInterfaces(params Type[] interfaceTypes)
    {
        return AllTypes().AsInterfaces(interfaceTypes);
    }

    IKeyedServiceSelector IServiceSelector.As(Func<Type, Type[]> typeSelector)
    {
        return AllTypes().As(typeSelector);
    }

    IKeyedServiceSelector IServiceSelector.As(Func<Type, Type[], Type[]> typeWithBaseTypesSelector)
    {
        return AllTypes().As(typeWithBaseTypesSelector);
    }

    IKeyedServiceSelector IServiceSelector.AsSelf()
    {
        return AllTypes().AsSelf();
    }

    IKeyedServiceSelector IServiceSelector.AsBase()
    {
        return AllTypes().AsBase();
    }
}
