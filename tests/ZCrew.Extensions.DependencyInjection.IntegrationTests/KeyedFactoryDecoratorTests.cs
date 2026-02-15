using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ZCrew.Extensions.DependencyInjection.IntegrationTests.Fixtures;

namespace ZCrew.Extensions.DependencyInjection.IntegrationTests;

public class KeyedFactoryDecoratorTests : DecoratorTestBase
{
    [Theory]
    [InlineData(null)]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void AddDecorator_WithKeyedDecoratorFactoryAndKeyedServiceInstance_ShouldThrowInvalidOperationException(
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            "service-key",
            (_, service, _) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.AddKeyedSingleton<IService>("service-key", new ConcreteService());
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        var service = serviceProvider.GetRequiredKeyedService<IService>("service-key");
        Assert.Collection(
            service.GetInstanceData(),
            instance => Assert.Equal(typeof(DecoratorService), instance.InstanceType),
            instance => Assert.Equal(typeof(ConcreteService), instance.InstanceType)
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void AddDecorator_WithKeyedDecoratorFactoryAndServiceInstance_ShouldThrowInvalidOperationException(
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            "service-key",
            (_, service, _) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.AddSingleton<IService>(new ConcreteService());
        var addDecorator = () => serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(addDecorator);
    }

    [Theory]
    [MemberData(nameof(ValidServiceDecoratorLifetimePairs))]
    public void AddDecorator_WithKeyedDecoratorFactoryKeyedServiceTypeAndValidLifetimes_ShouldApplyDecorator(
        ServiceLifetime serviceLifetime,
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(
            typeof(IService),
            "service-key",
            typeof(ConcreteService),
            serviceLifetime
        );
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            "service-key",
            (_, service, _) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        var service = serviceProvider.GetRequiredKeyedService<IService>("service-key");
        Assert.Collection(
            service.GetInstanceData(),
            instance => Assert.Equal(typeof(DecoratorService), instance.InstanceType),
            instance => Assert.Equal(typeof(ConcreteService), instance.InstanceType)
        );
    }

    [Theory]
    [MemberData(nameof(InvalidServiceDecoratorLifetimePairs))]
    public void AddDecorator_WithKeyedDecoratorFactoryKeyedServiceTypeAndInvalidLifetimes_ShouldThrowInvalidOperationException(
        ServiceLifetime serviceLifetime,
        ServiceLifetime decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(
            typeof(IService),
            "service-key",
            typeof(ConcreteService),
            serviceLifetime
        );
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            "service-key",
            (_, service, _) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        var addDecorator = () => serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(addDecorator);
    }

    [Theory]
    [MemberData(nameof(ValidServiceDecoratorLifetimePairs))]
    public void AddDecorator_WithKeyedDecoratorFactoryServiceType_ShouldThrowInvalidOperationException(
        ServiceLifetime serviceLifetime,
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(typeof(IService), typeof(ConcreteService), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            "service-key",
            (_, service, _) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        var addDecorator = () => serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(addDecorator);
    }

    [Theory]
    [MemberData(nameof(ValidServiceDecoratorLifetimePairs))]
    public void AddDecorator_WithKeyedDecoratorFactoryKeyedServiceFactoryAndValidLifetimes_ShouldApplyDecorator(
        ServiceLifetime serviceLifetime,
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(
            typeof(IService),
            "service-key",
            (_, _) => new ConcreteService(),
            serviceLifetime
        );
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            "service-key",
            (_, service, _) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        var service = serviceProvider.GetRequiredKeyedService<IService>("service-key");
        Assert.Collection(
            service.GetInstanceData(),
            instance => Assert.Equal(typeof(DecoratorService), instance.InstanceType),
            instance => Assert.Equal(typeof(ConcreteService), instance.InstanceType)
        );
    }

    [Theory]
    [MemberData(nameof(ValidServiceDecoratorLifetimePairs))]
    public void AddDecorator_WithKeyedDecoratorFactoryKeyedServiceFactory_ShouldUseOriginalServiceKey(
        ServiceLifetime serviceLifetime,
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var concreteServiceKey = default(object?);
        var decoratorServiceKey = default(object?);
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(
            typeof(IService),
            "service-key",
            (_, serviceKey) =>
            {
                concreteServiceKey = serviceKey;
                return new ConcreteService();
            },
            serviceLifetime
        );
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            "service-key",
            (_, service, serviceKey) =>
            {
                decoratorServiceKey = serviceKey;
                return new DecoratorService((IService)service);
            },
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        _ = serviceProvider.GetRequiredKeyedService<IService>("service-key");
        Assert.Equal("service-key", concreteServiceKey);
        Assert.Equal("service-key", decoratorServiceKey);
    }

    [Theory]
    [MemberData(nameof(InvalidServiceDecoratorLifetimePairs))]
    public void AddDecorator_WithKeyedDecoratorFactoryKeyedServiceFactoryAndInvalidLifetimes_ShouldThrowInvalidOperationException(
        ServiceLifetime serviceLifetime,
        ServiceLifetime decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(
            typeof(IService),
            "service-key",
            (_, _) => new ConcreteService(),
            serviceLifetime
        );
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            "service-key",
            (_, service, _) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        var addDecorator = () => serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(addDecorator);
    }

    [Theory]
    [MemberData(nameof(ValidServiceDecoratorLifetimePairs))]
    public void AddDecorator_WithKeyedDecoratorFactoryServiceFactory_ShouldThrowInvalidOperationException(
        ServiceLifetime serviceLifetime,
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(typeof(IService), _ => new ConcreteService(), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            "service-key",
            (_, service, _) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        var addDecorator = () => serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(addDecorator);
    }
}
