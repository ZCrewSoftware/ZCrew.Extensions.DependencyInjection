using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ZCrew.Extensions.DependencyInjection.IntegrationTests.Decorators;

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
            typeof(IAuditService),
            (_, service) => new AuditServiceDecorator((IAuditService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.AddSingleton<IAuditService>(new AuditService());
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        var service = serviceProvider.GetRequiredService<IAuditService>();
        Assert.Collection(
            service.GetInstanceData(),
            instance => Assert.Equal(typeof(AuditServiceDecorator), instance.InstanceType),
            instance => Assert.Equal(typeof(AuditService), instance.InstanceType)
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
            typeof(IAuditService),
            (_, service) => new AuditServiceDecorator((IAuditService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.AddKeyedSingleton<IAuditService>("service-key", new AuditService());
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
        var serviceDescriptor = new ServiceDescriptor(typeof(IAuditService), typeof(AuditService), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IAuditService),
            (_, service) => new AuditServiceDecorator((IAuditService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        var service = serviceProvider.GetRequiredService<IAuditService>();
        Assert.Collection(
            service.GetInstanceData(),
            instance => Assert.Equal(typeof(AuditServiceDecorator), instance.InstanceType),
            instance => Assert.Equal(typeof(AuditService), instance.InstanceType)
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
        var serviceDescriptor = new ServiceDescriptor(typeof(IAuditService), typeof(AuditService), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IAuditService),
            (_, service) => new AuditServiceDecorator((IAuditService)service),
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
            typeof(IAuditService),
            "service-key",
            typeof(AuditService),
            serviceLifetime
        );
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IAuditService),
            (_, service) => new AuditServiceDecorator((IAuditService)service),
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
        var serviceDescriptor = new ServiceDescriptor(typeof(IAuditService), _ => new AuditService(), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IAuditService),
            (_, service) => new AuditServiceDecorator((IAuditService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        var service = serviceProvider.GetRequiredService<IAuditService>();
        Assert.Collection(
            service.GetInstanceData(),
            instance => Assert.Equal(typeof(AuditServiceDecorator), instance.InstanceType),
            instance => Assert.Equal(typeof(AuditService), instance.InstanceType)
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
        var serviceDescriptor = new ServiceDescriptor(typeof(IAuditService), _ => new AuditService(), serviceLifetime);
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IAuditService),
            (_, service) => new AuditServiceDecorator((IAuditService)service),
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
            typeof(IAuditService),
            "service-key",
            (_, _) => new AuditService(),
            serviceLifetime
        );
        var decoratorServiceDescriptor = new DecoratorServiceDescriptor(
            typeof(IAuditService),
            (_, service) => new AuditServiceDecorator((IAuditService)service),
            decoratorLifetime
        );

        // Act
        serviceCollection.Add(serviceDescriptor);
        var addDecorator = () => serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(addDecorator);
    }
}
