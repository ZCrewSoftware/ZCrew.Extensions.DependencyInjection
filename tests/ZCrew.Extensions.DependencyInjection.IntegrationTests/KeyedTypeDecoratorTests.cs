using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ZCrew.Extensions.DependencyInjection.IntegrationTests.Fixtures;

namespace ZCrew.Extensions.DependencyInjection.IntegrationTests;

public class KeyedTypeDecoratorTests : DecoratorTestBase
{
    [Theory]
    [InlineData(null)]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void AddDecorator_WithKeyedDecoratorTypeAndKeyedServiceInstance_ShouldThrowInvalidOperationException(
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            "service-key",
            typeof(DecoratorService),
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
    public void AddDecorator_WithKeyedDecoratorTypeAndServiceInstance_ShouldThrowInvalidOperationException(
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            "service-key",
            typeof(DecoratorService),
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
    public void AddDecorator_WithKeyedDecoratorTypeKeyedServiceTypeAndValidLifetimes_ShouldApplyDecorator(
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
            typeof(DecoratorService),
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
    public void AddDecorator_WithKeyedDecoratorTypeKeyedServiceTypeAndInvalidLifetimes_ShouldThrowInvalidOperationException(
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
            typeof(DecoratorService),
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
    public void AddDecorator_WithKeyedDecoratorTypeServiceType_ShouldThrowInvalidOperationException(
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
            typeof(DecoratorService),
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
    public void AddDecorator_WithKeyedDecoratorTypeKeyedServiceFactoryAndValidLifetimes_ShouldApplyDecorator(
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
            typeof(DecoratorService),
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
    public void AddDecorator_WithKeyedDecoratorTypeKeyedServiceFactory_ShouldUseOriginalServiceKey(
        ServiceLifetime serviceLifetime,
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var concreteServiceKey = default(object?);
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
            typeof(DecoratorService),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        _ = serviceProvider.GetRequiredKeyedService<IService>("service-key");
        Assert.Equal("service-key", concreteServiceKey);
    }

    [Theory]
    [MemberData(nameof(InvalidServiceDecoratorLifetimePairs))]
    public void AddDecorator_WithKeyedDecoratorTypeKeyedServiceFactoryAndInvalidLifetimes_ShouldThrowInvalidOperationException(
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
            typeof(DecoratorService),
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
    public void AddDecorator_WithKeyedDecoratorTypeServiceFactory_ShouldThrowInvalidOperationException(
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
            typeof(DecoratorService),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        var addDecorator = () => serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(addDecorator);
    }
}
