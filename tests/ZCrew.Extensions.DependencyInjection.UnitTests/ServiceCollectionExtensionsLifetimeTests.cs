using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.UnitTests;

public class ServiceCollectionExtensionsLifetimeTests
{
    [Fact]
    public void AsSingleton_WhenCalled_ShouldReturnCollectionWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();

        // Act
        var result = services.AsSingleton();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void AsScoped_WhenCalled_ShouldReturnCollectionWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddTransient<IOrderService, OrderService>();

        // Act
        var result = services.AsScoped();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
    }

    [Fact]
    public void AsTransient_WhenCalled_ShouldReturnCollectionWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();

        // Act
        var result = services.AsTransient();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void AsLifetime_WhenCalled_ShouldReturnNewCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<ICustomerService, CustomerService>();

        // Act
        var result = services.AsLifetime(ServiceLifetime.Singleton);

        // Assert
        Assert.NotSame(services, result);
    }

    [Fact]
    public void AsLifetime_WhenCalled_ShouldNotModifyOriginalCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<ICustomerService, CustomerService>();

        // Act
        services.AsLifetime(ServiceLifetime.Singleton);

        // Assert
        Assert.Equal(ServiceLifetime.Transient, services[0].Lifetime);
    }

    [Fact]
    public void AsLifetime_WhenCalledWithoutIgnoreFlag_ShouldDefaultToIgnoringSingletonImplementations()
    {
        // Arrange
        var services = new ServiceCollection();
        var instance = new CustomerService();
        services.AddSingleton<ICustomerService>(instance);

        // Act
        var result = services.AsLifetime(ServiceLifetime.Transient);

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.Same(instance, descriptor.ImplementationInstance);
    }

    [Fact]
    public void AsLifetime_WhenNotIgnoringAndHasInstanceDescriptor_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ICustomerService>(new CustomerService());

        // Act
        var act = () => services.AsLifetime(ServiceLifetime.Transient, ignoreSingletonImplementations: false);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AsScoped_WhenIgnoringSingletonImplementations_ShouldKeepInstanceDescriptorsUnchanged()
    {
        // Arrange
        var services = new ServiceCollection();
        var instance = new CustomerService();
        services.AddSingleton<ICustomerService>(instance);
        services.AddSingleton<IOrderService, OrderService>();

        // Act
        var result = services.AsScoped(ignoreSingletonImplementations: true);

        // Assert
        Assert.Equal(2, result.Count);
        var instanceDescriptor = result.First(d => d.ServiceType == typeof(ICustomerService));
        Assert.Equal(ServiceLifetime.Singleton, instanceDescriptor.Lifetime);
        Assert.Same(instance, instanceDescriptor.ImplementationInstance);
        var typeDescriptor = result.First(d => d.ServiceType == typeof(IOrderService));
        Assert.Equal(ServiceLifetime.Scoped, typeDescriptor.Lifetime);
    }

    [Fact]
    public void AsTransient_WhenIgnoringSingletonImplementations_ShouldKeepInstanceDescriptorsUnchanged()
    {
        // Arrange
        var services = new ServiceCollection();
        var instance = new CustomerService();
        services.AddSingleton<ICustomerService>(instance);
        services.AddSingleton<IOrderService, OrderService>();

        // Act
        var result = services.AsTransient(ignoreSingletonImplementations: true);

        // Assert
        Assert.Equal(2, result.Count);
        var instanceDescriptor = result.First(d => d.ServiceType == typeof(ICustomerService));
        Assert.Equal(ServiceLifetime.Singleton, instanceDescriptor.Lifetime);
        var typeDescriptor = result.First(d => d.ServiceType == typeof(IOrderService));
        Assert.Equal(ServiceLifetime.Transient, typeDescriptor.Lifetime);
    }

    [Fact]
    public void AsScoped_WhenNotIgnoringAndHasInstanceDescriptor_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ICustomerService>(new CustomerService());

        // Act
        var act = () => services.AsScoped(ignoreSingletonImplementations: false);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AsTransient_WhenNotIgnoringAndHasInstanceDescriptor_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ICustomerService>(new CustomerService());

        // Act
        var act = () => services.AsTransient(ignoreSingletonImplementations: false);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
}
