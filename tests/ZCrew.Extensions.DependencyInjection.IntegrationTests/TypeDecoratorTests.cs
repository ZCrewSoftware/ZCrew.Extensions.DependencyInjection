using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ZCrew.Extensions.DependencyInjection.IntegrationTests.Fixtures;

namespace ZCrew.Extensions.DependencyInjection.IntegrationTests;

public class TypeDecoratorTests : DecoratorTestBase
{
    [Theory]
    [InlineData(null)]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void AddDecorator_WithDecoratorTypeAndServiceInstance_ShouldApplyDecorator(
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            typeof(DecoratorService),
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
    public void AddDecorator_WithDecoratorTypeAndKeyedServiceInstance_ShouldThrowInvalidOperationException(
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            typeof(DecoratorService),
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
    public void AddDecorator_WithDecoratorTypeServiceTypeAndValidLifetimes_ShouldApplyDecorator(
        ServiceLifetime serviceLifetime,
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(typeof(IService), typeof(ConcreteService), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            typeof(DecoratorService),
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
    public void AddDecorator_WithDecoratorTypeServiceTypeAndInvalidLifetimes_ShouldThrowInvalidOperationException(
        ServiceLifetime serviceLifetime,
        ServiceLifetime decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(typeof(IService), typeof(ConcreteService), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
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
    public void AddDecorator_WithDecoratorTypeKeyedServiceType_ShouldThrowInvalidOperationException(
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
    public void AddDecorator_WithDecoratorTypeServiceFactoryAndValidLifetimes_ShouldApplyDecorator(
        ServiceLifetime serviceLifetime,
        ServiceLifetime? decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(typeof(IService), _ => new ConcreteService(), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
            typeof(DecoratorService),
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
    public void AddDecorator_WithDecoratorTypeServiceFactoryAndInvalidLifetimes_ShouldThrowInvalidOperationException(
        ServiceLifetime serviceLifetime,
        ServiceLifetime decoratorLifetime
    )
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceDescriptor = new ServiceDescriptor(typeof(IService), _ => new ConcreteService(), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IService),
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
    public void AddDecorator_WithDecoratorTypeKeyedServiceFactory_ShouldThrowInvalidOperationException(
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
