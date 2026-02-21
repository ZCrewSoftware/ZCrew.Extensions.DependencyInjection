namespace ZCrew.Extensions.DependencyInjection.Registration;

public partial interface ITypeFilter
{
    IServiceSource IServiceSelector.AsAllInterfaces()
    {
        return AllTypes().AsAllInterfaces();
    }

    IServiceSource IServiceSelector.AsAllNonSystemInterfaces()
    {
        return AllTypes().AsAllNonSystemInterfaces();
    }

    IServiceSource IServiceSelector.AsDefaultInterfaces()
    {
        return AllTypes().AsDefaultInterfaces();
    }

    IServiceSource IServiceSelector.AsDefaultNonSystemInterfaces()
    {
        return AllTypes().AsDefaultNonSystemInterfaces();
    }

    IServiceSource IServiceSelector.AsFirstInterface()
    {
        return AllTypes().AsFirstInterface();
    }

    IServiceSource IServiceSelector.AsInterface()
    {
        return AllTypes().AsInterface();
    }

    IServiceSource IServiceSelector.AsInterface<T>()
    {
        return AllTypes().AsInterface<T>();
    }

    IServiceSource IServiceSelector.AsInterface(Type interfaceType)
    {
        return AllTypes().AsInterface(interfaceType);
    }

    IServiceSource IServiceSelector.AsInterfaces(params Type[] interfaceTypes)
    {
        return AllTypes().AsInterfaces(interfaceTypes);
    }

    IServiceSource IServiceSelector.As(Func<Type, Type[]> typeSelector)
    {
        return AllTypes().As(typeSelector);
    }

    IServiceSource IServiceSelector.As(Func<Type, Type[], Type[]> typeWithBaseTypesSelector)
    {
        return AllTypes().As(typeWithBaseTypesSelector);
    }

    IServiceSource IServiceSelector.AsSelf()
    {
        return AllTypes().AsSelf();
    }

    IServiceSource IServiceSelector.AsBase()
    {
        return AllTypes().AsBase();
    }
}
