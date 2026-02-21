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

    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void WithLifetime_WhenSourceHasImplementationType_ShouldReturnDescriptorWithNewLifetime(
        ServiceLifetime lifetime
    )
    {
        // Arrange
        var descriptor = new ServiceDescriptor(typeof(IService), typeof(ConcreteService), ServiceLifetime.Singleton);

        // Act
        var result = descriptor.WithLifetime(lifetime);

        // Assert
        Assert.Equal(typeof(IService), result.ServiceType);
        Assert.Equal(typeof(ConcreteService), result.ImplementationType);
        Assert.Equal(lifetime, result.Lifetime);
    }

    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void WithLifetime_WhenSourceHasImplementationFactory_ShouldReturnDescriptorWithNewLifetime(
        ServiceLifetime lifetime
    )
    {
        // Arrange
        var instance = new ConcreteService();
        var descriptor = new ServiceDescriptor(typeof(IService), _ => instance, ServiceLifetime.Singleton);

        // Act
        var result = descriptor.WithLifetime(lifetime);

        // Assert
        Assert.Equal(typeof(IService), result.ServiceType);
        Assert.NotNull(result.ImplementationFactory);
        Assert.Equal(lifetime, result.Lifetime);
    }

    [Fact]
    public void WithLifetime_WhenSourceHasImplementationInstance_AndTargetIsSingleton_ShouldReturnSameDescriptor()
    {
        // Arrange
        var instance = new ConcreteService();
        var descriptor = new ServiceDescriptor(typeof(IService), instance);

        // Act
        var result = descriptor.WithLifetime(ServiceLifetime.Singleton);

        // Assert
        Assert.Same(descriptor, result);
    }

    [Theory]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void WithLifetime_WhenSourceHasImplementationInstance_AndTargetIsNotSingleton_ShouldThrow(
        ServiceLifetime lifetime
    )
    {
        // Arrange
        var descriptor = new ServiceDescriptor(typeof(IService), new ConcreteService());

        // Act
        var act = () => descriptor.WithLifetime(lifetime);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Theory]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void WithLifetime_WhenIgnoringAndSourceHasImplementationInstance_ShouldReturnSameDescriptor(
        ServiceLifetime lifetime
    )
    {
        // Arrange
        var descriptor = new ServiceDescriptor(typeof(IService), new ConcreteService());

        // Act
        var result = descriptor.WithLifetime(lifetime, ignoreSingletonImplementations: true);

        // Assert
        Assert.Same(descriptor, result);
    }

    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void WithLifetime_WhenSourceHasKeyedImplementationType_ShouldReturnDescriptorWithNewLifetime(
        ServiceLifetime lifetime
    )
    {
        // Arrange
        var serviceKey = "my-key";
        var descriptor = new ServiceDescriptor(
            typeof(IService),
            serviceKey,
            typeof(ConcreteService),
            ServiceLifetime.Singleton
        );

        // Act
        var result = descriptor.WithLifetime(lifetime);

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
    public void WithLifetime_WhenSourceHasKeyedImplementationFactory_ShouldReturnDescriptorWithNewLifetime(
        ServiceLifetime lifetime
    )
    {
        // Arrange
        var serviceKey = "my-key";
        var instance = new ConcreteService();
        var descriptor = new ServiceDescriptor(
            typeof(IService),
            serviceKey,
            (_, _) => instance,
            ServiceLifetime.Singleton
        );

        // Act
        var result = descriptor.WithLifetime(lifetime);

        // Assert
        Assert.Equal(typeof(IService), result.ServiceType);
        Assert.Equal(serviceKey, result.ServiceKey);
        Assert.NotNull(result.KeyedImplementationFactory);
        Assert.Equal(lifetime, result.Lifetime);
    }

    [Fact]
    public void WithLifetime_WhenSourceHasKeyedImplementationInstance_AndTargetIsSingleton_ShouldReturnSameDescriptor()
    {
        // Arrange
        var instance = new ConcreteService();
        var descriptor = new ServiceDescriptor(typeof(IService), "my-key", instance);

        // Act
        var result = descriptor.WithLifetime(ServiceLifetime.Singleton);

        // Assert
        Assert.Same(descriptor, result);
    }

    [Theory]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void WithLifetime_WhenSourceHasKeyedImplementationInstance_AndTargetIsNotSingleton_ShouldThrow(
        ServiceLifetime lifetime
    )
    {
        // Arrange
        var descriptor = new ServiceDescriptor(typeof(IService), "my-key", new ConcreteService());

        // Act
        var act = () => descriptor.WithLifetime(lifetime);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Theory]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void WithLifetime_WhenIgnoringAndSourceHasKeyedImplementationInstance_ShouldReturnSameDescriptor(
        ServiceLifetime lifetime
    )
    {
        // Arrange
        var descriptor = new ServiceDescriptor(typeof(IService), "my-key", new ConcreteService());

        // Act
        var result = descriptor.WithLifetime(lifetime, ignoreSingletonImplementations: true);

        // Assert
        Assert.Same(descriptor, result);
    }

    private interface IService;

    private class ConcreteService : IService;
}
