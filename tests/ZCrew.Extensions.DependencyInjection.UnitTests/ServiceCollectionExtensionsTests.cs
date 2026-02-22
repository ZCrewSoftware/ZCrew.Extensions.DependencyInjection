using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Infrastructure.External;
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
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(PayPalPaymentGateway), ServiceLifetime.Transient),
        };

        // Act
        services.AddSingleton(descriptors);

        // Assert
        var added = Assert.Single(services);
        Assert.Equal(typeof(IPaymentGateway), added.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, added.Lifetime);
    }

    [Fact]
    public void AddScoped_WhenCalled_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(PayPalPaymentGateway), ServiceLifetime.Transient),
        };

        // Act
        services.AddScoped(descriptors);

        // Assert
        var added = Assert.Single(services);
        Assert.Equal(typeof(IPaymentGateway), added.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, added.Lifetime);
    }

    [Fact]
    public void AddTransient_WhenCalled_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(PayPalPaymentGateway), ServiceLifetime.Singleton),
        };

        // Act
        services.AddTransient(descriptors);

        // Assert
        var added = Assert.Single(services);
        Assert.Equal(typeof(IPaymentGateway), added.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, added.Lifetime);
    }

    [Fact]
    public void AddSingleton_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddSingleton(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddScoped_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddScoped(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddTransient_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddTransient(null!);

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
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(PayPalPaymentGateway), ServiceLifetime.Transient),
            new ServiceDescriptor(typeof(IOrderService), typeof(OrderService), ServiceLifetime.Scoped),
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
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(PayPalPaymentGateway), ServiceLifetime.Transient),
        };

        // Act
        services.TryAddSingleton(descriptors);

        // Assert
        var added = Assert.Single(services);
        Assert.Equal(typeof(IPaymentGateway), added.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, added.Lifetime);
    }

    [Fact]
    public void TryAddScoped_WhenServiceTypeNotRegistered_ShouldAddDescriptorsWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(PayPalPaymentGateway), ServiceLifetime.Transient),
        };

        // Act
        services.TryAddScoped(descriptors);

        // Assert
        var added = Assert.Single(services);
        Assert.Equal(typeof(IPaymentGateway), added.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, added.Lifetime);
    }

    [Fact]
    public void TryAddTransient_WhenServiceTypeNotRegistered_ShouldAddDescriptorsWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(PayPalPaymentGateway), ServiceLifetime.Singleton),
        };

        // Act
        services.TryAddTransient(descriptors);

        // Assert
        var added = Assert.Single(services);
        Assert.Equal(typeof(IPaymentGateway), added.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, added.Lifetime);
    }

    [Fact]
    public void TryAddSingleton_WhenServiceTypeAlreadyRegistered_ShouldNotAddDescriptor()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IPaymentGateway, PayPalPaymentGateway>();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(PayPalPaymentGateway), ServiceLifetime.Transient),
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
        var act = () => services.TryAddSingleton(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void TryAddScoped_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.TryAddScoped(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void TryAddTransient_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.TryAddTransient(null!);

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
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(PayPalPaymentGateway), ServiceLifetime.Transient),
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
        services.AddSingleton<IPaymentGateway, PayPalPaymentGateway>();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(StripePaymentGateway), ServiceLifetime.Transient),
        };

        // Act
        services.Replace(descriptors);

        // Assert
        var replaced = Assert.Single(services);
        Assert.Equal(typeof(IPaymentGateway), replaced.ServiceType);
        Assert.Equal(typeof(StripePaymentGateway), replaced.ImplementationType);
        Assert.Equal(ServiceLifetime.Transient, replaced.Lifetime);
    }

    [Fact]
    public void Replace_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.Replace(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void ReplaceSingleton_WhenCalled_ShouldReplaceWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IPaymentGateway, PayPalPaymentGateway>();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(StripePaymentGateway), ServiceLifetime.Transient),
        };

        // Act
        services.ReplaceSingleton(descriptors);

        // Assert
        var replaced = Assert.Single(services);
        Assert.Equal(typeof(IPaymentGateway), replaced.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, replaced.Lifetime);
    }

    [Fact]
    public void ReplaceScoped_WhenCalled_ShouldReplaceWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IPaymentGateway, PayPalPaymentGateway>();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(StripePaymentGateway), ServiceLifetime.Transient),
        };

        // Act
        services.ReplaceScoped(descriptors);

        // Assert
        var replaced = Assert.Single(services);
        Assert.Equal(typeof(IPaymentGateway), replaced.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, replaced.Lifetime);
    }

    [Fact]
    public void ReplaceTransient_WhenCalled_ShouldReplaceWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IPaymentGateway, PayPalPaymentGateway>();
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(IPaymentGateway), typeof(StripePaymentGateway), ServiceLifetime.Singleton),
        };

        // Act
        services.ReplaceTransient(descriptors);

        // Assert
        var replaced = Assert.Single(services);
        Assert.Equal(typeof(IPaymentGateway), replaced.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, replaced.Lifetime);
    }

    [Fact]
    public void ReplaceSingleton_WhenDescriptorsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.ReplaceSingleton(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }
}
