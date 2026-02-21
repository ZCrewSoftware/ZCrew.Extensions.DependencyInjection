namespace ZCrew.Extensions.DependencyInjection.Registration;

public interface IServiceSelector : IServiceSource
{
    IServiceSource AsAllInterfaces();

    IServiceSource AsAllNonSystemInterfaces();

    IServiceSource AsDefaultInterfaces();

    IServiceSource AsDefaultNonSystemInterfaces();

    IServiceSource AsFirstInterface();

    IServiceSource AsInterface();

    IServiceSource AsInterface<T>();

    IServiceSource AsInterface(Type interfaceType);

    IServiceSource AsInterfaces(params Type[] interfaceTypes);

    IServiceSource As(Func<Type, Type[]> typeSelector);

    IServiceSource As(Func<Type, Type[], Type[]> typeWithBaseTypesSelector);

    IServiceSource AsSelf();

    IServiceSource AsBase();
}
