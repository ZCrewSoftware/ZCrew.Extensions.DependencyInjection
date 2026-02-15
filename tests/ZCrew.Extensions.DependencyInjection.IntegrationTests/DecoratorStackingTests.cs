using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ZCrew.Extensions.DependencyInjection.IntegrationTests.Fixtures;

namespace ZCrew.Extensions.DependencyInjection.IntegrationTests;

public class DecoratorStackingTests : DecoratorTestBase
{
    [Theory]
    [InlineData(null)]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void AddDecorator_WhenCalledTwice_ShouldStackDecorators(ServiceLifetime? decoratorLifetime)
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
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        var service = serviceProvider.GetRequiredService<IService>();
        var instanceData = service.GetInstanceData().ToArray();
        Assert.Collection(
            instanceData,
            instance => Assert.Equal(typeof(DecoratorService), instance.InstanceType),
            instance => Assert.Equal(typeof(DecoratorService), instance.InstanceType),
            instance => Assert.Equal(typeof(ConcreteService), instance.InstanceType)
        );
        // The two decorators should be separate instances
        Assert.NotEqual(instanceData[0].InstanceId, instanceData[1].InstanceId);
    }

    [Theory]
    [InlineData(ServiceLifetime.Singleton, null)]
    [InlineData(ServiceLifetime.Singleton, ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Singleton, ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Singleton, ServiceLifetime.Transient)]
    [InlineData(ServiceLifetime.Scoped, null)]
    [InlineData(ServiceLifetime.Scoped, ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Scoped, ServiceLifetime.Transient)]
    [InlineData(ServiceLifetime.Transient, null)]
    [InlineData(ServiceLifetime.Transient, ServiceLifetime.Transient)]
    public void AddDecorator_WithMultipleServices_ShouldApplyDecoratorToAllServices(
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
        serviceCollection.Add(serviceDescriptor);
        serviceCollection.AddDecorator(decoratorServiceDescriptor);

        // Assert
        var serviceProvider = ServiceProviderFactory.CreateServiceProvider(serviceCollection);
        var services = serviceProvider.GetRequiredService<IEnumerable<IService>>().ToArray();
        var instanceData = services.Select(service => service.GetInstanceData().ToArray()).ToArray();
        Assert.Equal(2, services.Length);
        Assert.Collection(
            instanceData[0],
            instance => Assert.Equal(typeof(DecoratorService), instance.InstanceType),
            instance => Assert.Equal(typeof(ConcreteService), instance.InstanceType)
        );
        Assert.Collection(
            instanceData[1],
            instance => Assert.Equal(typeof(DecoratorService), instance.InstanceType),
            instance => Assert.Equal(typeof(ConcreteService), instance.InstanceType)
        );
        // The two decorators should be separate instances
        Assert.NotEqual(instanceData[0][0].InstanceId, instanceData[1][0].InstanceId);
        // The two services should be separate instances
        Assert.NotEqual(instanceData[0][1].InstanceId, instanceData[1][1].InstanceId);
    }
}
