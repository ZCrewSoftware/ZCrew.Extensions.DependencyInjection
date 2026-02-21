using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.UnitTests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSingleton_WhenCalled_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(ServiceA), ServiceLifetime.Transient),
        };

        // Act
        services.AddSingleton(descriptors);

        // Assert
        var added = Assert.Single(services);
        Assert.Equal(typeof(IServiceA), added.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, added.Lifetime);
    }

    [Fact]
    public void AddScoped_WhenCalled_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(ServiceA), ServiceLifetime.Transient),
        };

        // Act
        services.AddScoped(descriptors);

        // Assert
        var added = Assert.Single(services);
        Assert.Equal(typeof(IServiceA), added.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, added.Lifetime);
    }

    [Fact]
    public void AddTransient_WhenCalled_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(ServiceA), ServiceLifetime.Singleton),
        };

        // Act
        services.AddTransient(descriptors);

        // Assert
        var added = Assert.Single(services);
        Assert.Equal(typeof(IServiceA), added.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, added.Lifetime);
    }

    [Fact]
    public void AddSingleton_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddSingleton((IEnumerable<ServiceDescriptor>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddScoped_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddScoped((IEnumerable<ServiceDescriptor>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddTransient_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddTransient((IEnumerable<ServiceDescriptor>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddSingleton_WhenMultipleDescriptors_ShouldAddAllWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(ServiceA), ServiceLifetime.Transient),
            new ServiceDescriptor(typeof(IServiceB), typeof(ServiceB), ServiceLifetime.Scoped),
        };

        // Act
        services.AddSingleton(descriptors);

        // Assert
        Assert.Equal(2, services.Count);
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void TryAddSingleton_WhenServiceTypeNotRegistered_ShouldAddDescriptorsWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(ServiceA), ServiceLifetime.Transient),
        };

        // Act
        services.TryAddSingleton(descriptors);

        // Assert
        var added = Assert.Single(services);
        Assert.Equal(typeof(IServiceA), added.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, added.Lifetime);
    }

    [Fact]
    public void TryAddScoped_WhenServiceTypeNotRegistered_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(ServiceA), ServiceLifetime.Transient),
        };

        // Act
        services.TryAddScoped(descriptors);

        // Assert
        var added = Assert.Single(services);
        Assert.Equal(typeof(IServiceA), added.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, added.Lifetime);
    }

    [Fact]
    public void TryAddTransient_WhenServiceTypeNotRegistered_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(ServiceA), ServiceLifetime.Singleton),
        };

        // Act
        services.TryAddTransient(descriptors);

        // Assert
        var added = Assert.Single(services);
        Assert.Equal(typeof(IServiceA), added.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, added.Lifetime);
    }

    [Fact]
    public void TryAddSingleton_WhenServiceTypeAlreadyRegistered_ShouldNotAddDescriptor()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IServiceA, ServiceA>();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(ServiceA), ServiceLifetime.Transient),
        };

        // Act
        services.TryAddSingleton(descriptors);

        // Assert
        Assert.Single(services);
    }

    [Fact]
    public void TryAddSingleton_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.TryAddSingleton((IEnumerable<ServiceDescriptor>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void TryAddScoped_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.TryAddScoped((IEnumerable<ServiceDescriptor>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void TryAddTransient_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.TryAddTransient((IEnumerable<ServiceDescriptor>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void TryAddSingleton_WhenCalled_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(ServiceA), ServiceLifetime.Transient),
        };

        // Act
        var result = services.TryAddSingleton(descriptors);

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void Replace_WhenCalled_ShouldReplaceExistingRegistrations()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IServiceA, ServiceA>();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(AlternateServiceA), ServiceLifetime.Transient),
        };

        // Act
        services.Replace(descriptors);

        // Assert
        var replaced = Assert.Single(services);
        Assert.Equal(typeof(IServiceA), replaced.ServiceType);
        Assert.Equal(typeof(AlternateServiceA), replaced.ImplementationType);
        Assert.Equal(ServiceLifetime.Transient, replaced.Lifetime);
    }

    [Fact]
    public void Replace_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.Replace((IEnumerable<ServiceDescriptor>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void ReplaceSingleton_WhenCalled_ShouldReplaceWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IServiceA, ServiceA>();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(AlternateServiceA), ServiceLifetime.Transient),
        };

        // Act
        services.ReplaceSingleton(descriptors);

        // Assert
        var replaced = Assert.Single(services);
        Assert.Equal(typeof(IServiceA), replaced.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, replaced.Lifetime);
    }

    [Fact]
    public void ReplaceScoped_WhenCalled_ShouldReplaceWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IServiceA, ServiceA>();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(AlternateServiceA), ServiceLifetime.Transient),
        };

        // Act
        services.ReplaceScoped(descriptors);

        // Assert
        var replaced = Assert.Single(services);
        Assert.Equal(typeof(IServiceA), replaced.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, replaced.Lifetime);
    }

    [Fact]
    public void ReplaceTransient_WhenCalled_ShouldReplaceWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IServiceA, ServiceA>();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IServiceA), typeof(AlternateServiceA), ServiceLifetime.Singleton),
        };

        // Act
        services.ReplaceTransient(descriptors);

        // Assert
        var replaced = Assert.Single(services);
        Assert.Equal(typeof(IServiceA), replaced.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, replaced.Lifetime);
    }

    [Fact]
    public void ReplaceSingleton_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.ReplaceSingleton((IEnumerable<ServiceDescriptor>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    private interface IServiceA;

    private interface IServiceB;

    private class ServiceA : IServiceA;

    private class ServiceB : IServiceB;

    private class AlternateServiceA : IServiceA;
}
