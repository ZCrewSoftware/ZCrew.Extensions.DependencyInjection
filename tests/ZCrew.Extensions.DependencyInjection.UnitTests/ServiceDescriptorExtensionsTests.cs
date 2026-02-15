using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.UnitTests;

public class ServiceDescriptorExtensionsTests
{
    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void WithServiceKey_WhenSourceHasImplementationType_ShouldReturnKeyedDescriptorWithSameType(
        ServiceLifetime lifetime
    )
    {
        // Arrange
        var serviceKey = new object();
        var descriptor = new ServiceDescriptor(typeof(IService), typeof(ConcreteService), lifetime);

        // Act
        var result = descriptor.WithServiceKey(serviceKey);

        // Assert
        Assert.Equal(typeof(IService), result.ServiceType);
        Assert.Equal(serviceKey, result.ServiceKey);
        Assert.Equal(typeof(ConcreteService), result.KeyedImplementationType);
        Assert.Equal(lifetime, result.Lifetime);
    }

    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void WithServiceKey_WhenSourceHasImplementationFactory_ShouldReturnKeyedDescriptorWithFactory(
        ServiceLifetime lifetime
    )
    {
        // Arrange
        var serviceKey = new object();
        var instance = new ConcreteService();
        var descriptor = new ServiceDescriptor(typeof(IService), _ => instance, lifetime);

        // Act
        var result = descriptor.WithServiceKey(serviceKey);

        // Assert
        Assert.Equal(typeof(IService), result.ServiceType);
        Assert.Equal(serviceKey, result.ServiceKey);
        Assert.NotNull(result.KeyedImplementationFactory);
        Assert.Equal(lifetime, result.Lifetime);
    }

    [Fact]
    public void WithServiceKey_WhenSourceHasImplementationFactory_ShouldInvokeOriginalFactory()
    {
        // Arrange
        var serviceKey = new object();
        var instance = new ConcreteService();
        var descriptor = new ServiceDescriptor(typeof(IService), _ => instance, ServiceLifetime.Singleton);

        // Act
        var result = descriptor.WithServiceKey(serviceKey);
        var resolved = result.KeyedImplementationFactory!(null!, null);

        // Assert
        Assert.Same(instance, resolved);
    }

    [Fact]
    public void WithServiceKey_WhenSourceHasImplementationInstance_ShouldReturnKeyedDescriptorWithSameInstance()
    {
        // Arrange
        var serviceKey = new object();
        var instance = new ConcreteService();
        var descriptor = new ServiceDescriptor(typeof(IService), instance);

        // Act
        var result = descriptor.WithServiceKey(serviceKey);

        // Assert
        Assert.Equal(typeof(IService), result.ServiceType);
        Assert.Equal(serviceKey, result.ServiceKey);
        Assert.Same(instance, result.KeyedImplementationInstance);
    }

    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void WithServiceKey_WhenSourceHasKeyedImplementationType_ShouldReturnDescriptorWithNewKey(
        ServiceLifetime lifetime
    )
    {
        // Arrange
        var serviceKey = new object();
        var originalKey = "original-key";
        var descriptor = new ServiceDescriptor(
            typeof(IService),
            originalKey,
            typeof(ConcreteService),
            lifetime
        );

        // Act
        var result = descriptor.WithServiceKey(serviceKey);

        // Assert
        Assert.Equal(typeof(IService), result.ServiceType);
        Assert.Equal(serviceKey, result.ServiceKey);
        Assert.Equal(typeof(ConcreteService), result.KeyedImplementationType);
        Assert.Equal(lifetime, result.Lifetime);
    }

    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void WithServiceKey_WhenSourceHasKeyedImplementationFactory_ShouldReturnDescriptorWithNewKey(
        ServiceLifetime lifetime
    )
    {
        // Arrange
        var serviceKey = new object();
        var originalKey = "original-key";
        var instance = new ConcreteService();
        var descriptor = new ServiceDescriptor(
            typeof(IService),
            originalKey,
            (_, _) => instance,
            lifetime
        );

        // Act
        var result = descriptor.WithServiceKey(serviceKey);

        // Assert
        Assert.Equal(typeof(IService), result.ServiceType);
        Assert.Equal(serviceKey, result.ServiceKey);
        Assert.NotNull(result.KeyedImplementationFactory);
        Assert.Equal(lifetime, result.Lifetime);
    }

    [Fact]
    public void WithServiceKey_WhenSourceHasKeyedImplementationFactory_ShouldInvokeOriginalFactoryWithOriginalKey()
    {
        // Arrange
        var serviceKey = new object();
        var originalKey = "original-key";
        object? capturedKey = null;
        var instance = new ConcreteService();
        var descriptor = new ServiceDescriptor(
            typeof(IService),
            originalKey,
            (_, key) =>
            {
                capturedKey = key;
                return instance;
            },
            ServiceLifetime.Singleton
        );

        // Act
        var result = descriptor.WithServiceKey(serviceKey);
        var resolved = result.KeyedImplementationFactory!(null!, null);

        // Assert
        Assert.Same(instance, resolved);
        Assert.Equal(originalKey, capturedKey);
    }

    [Fact]
    public void WithServiceKey_WhenSourceHasKeyedImplementationInstance_ShouldReturnDescriptorWithNewKey()
    {
        // Arrange
        var serviceKey = new object();
        var originalKey = "original-key";
        var instance = new ConcreteService();
        var descriptor = new ServiceDescriptor(typeof(IService), originalKey, instance);

        // Act
        var result = descriptor.WithServiceKey(serviceKey);

        // Assert
        Assert.Equal(typeof(IService), result.ServiceType);
        Assert.Equal(serviceKey, result.ServiceKey);
        Assert.Same(instance, result.KeyedImplementationInstance);
    }

    private interface IService;

    private class ConcreteService : IService;
}
