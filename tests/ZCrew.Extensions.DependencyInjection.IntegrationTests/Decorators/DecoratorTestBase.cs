using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.IntegrationTests.Decorators;

public abstract class DecoratorTestBase
{
    protected static readonly DefaultServiceProviderFactory ServiceProviderFactory = new(
        new ServiceProviderOptions { ValidateOnBuild = true }
    );

    public static readonly TheoryData<ServiceLifetime, ServiceLifetime?> ValidServiceDecoratorLifetimePairs =
        new (
            (ServiceLifetime.Singleton, null),
            (ServiceLifetime.Singleton, ServiceLifetime.Singleton),
            (ServiceLifetime.Singleton, ServiceLifetime.Scoped),
            (ServiceLifetime.Singleton, ServiceLifetime.Transient),
            (ServiceLifetime.Scoped, null),
            (ServiceLifetime.Scoped, ServiceLifetime.Scoped),
            (ServiceLifetime.Scoped, ServiceLifetime.Transient),
            (ServiceLifetime.Transient, null),
            (ServiceLifetime.Transient, ServiceLifetime.Transient)
        );

    public static readonly TheoryData<ServiceLifetime, ServiceLifetime> InvalidServiceDecoratorLifetimePairs =
        new (
            (ServiceLifetime.Scoped, ServiceLifetime.Singleton),
            (ServiceLifetime.Transient, ServiceLifetime.Singleton),
            (ServiceLifetime.Transient, ServiceLifetime.Scoped)
        );
}
