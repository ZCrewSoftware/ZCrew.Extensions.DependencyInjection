using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ZCrew.Extensions.DependencyInjection.IntegrationTests.Fixtures;

namespace ZCrew.Extensions.DependencyInjection.IntegrationTests;

public class FactoryDecoratorTests : DecoratorTestBase
{
    [Theory]
    [InlineData(null)]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void AddDecorator_WithDecoratorFactoryAndServiceInstance_ShouldApplyDecorator(
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            (_, service) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.AddSingleton<IService>(new ConcreteService());
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        var service = serviceProvider.GetRequiredService<IService>();
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
    public void AddDecorator_WithDecoratorFactoryAndKeyedServiceInstance_ShouldThrowInvalidOperationException(
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            (_, service) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.AddKeyedSingleton<IService>("service-key", new ConcreteService());
        var addDecorator = () => serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(addDecorator);
    }

    [Theory]
    [MemberData(nameof(ValidServiceDecoratorLifetimePairs))]
    public void AddDecorator_WithDecoratorFactoryServiceTypeAndValidLifetimes_ShouldApplyDecorator(
        ServiceLifetime serviceLifetime,
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(typeof(IService), typeof(ConcreteService), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            (_, service) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        var service = serviceProvider.GetRequiredService<IService>();
        Assert.Collection(
            service.GetInstanceData(),
            instance => Assert.Equal(typeof(DecoratorService), instance.InstanceType),
            instance => Assert.Equal(typeof(ConcreteService), instance.InstanceType)
        );
    }

    [Theory]
    [MemberData(nameof(InvalidServiceDecoratorLifetimePairs))]
    public void AddDecorator_WithDecoratorFactoryServiceTypeAndInvalidLifetimes_ShouldThrowInvalidOperationException(
        ServiceLifetime serviceLifetime,
        ServiceLifetime decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(typeof(IService), typeof(ConcreteService), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            (_, service) => new DecoratorService((IService)service),
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
    public void AddDecorator_WithDecoratorFactoryKeyedServiceType_ShouldThrowInvalidOperationException(
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
            (_, service) => new DecoratorService((IService)service),
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
    public void AddDecorator_WithDecoratorFactoryServiceFactoryAndValidLifetimes_ShouldApplyDecorator(
        ServiceLifetime serviceLifetime,
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(typeof(IService), _ => new ConcreteService(), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            (_, service) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        var service = serviceProvider.GetRequiredService<IService>();
        Assert.Collection(
            service.GetInstanceData(),
            instance => Assert.Equal(typeof(DecoratorService), instance.InstanceType),
            instance => Assert.Equal(typeof(ConcreteService), instance.InstanceType)
        );
    }

    [Theory]
    [MemberData(nameof(InvalidServiceDecoratorLifetimePairs))]
    public void AddDecorator_WithDecoratorFactoryServiceFactoryAndInvalidLifetimes_ShouldThrowInvalidOperationException(
        ServiceLifetime serviceLifetime,
        ServiceLifetime decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(typeof(IService), _ => new ConcreteService(), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            (_, service) => new DecoratorService((IService)service),
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
    public void AddDecorator_WithDecoratorFactoryKeyedServiceFactory_ShouldThrowInvalidOperationException(
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
            (_, service) => new DecoratorService((IService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        var addDecorator = () => serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(addDecorator);
    }
}
