using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceSourceExtensionsTests
{
    [Fact]
    public void AsSingleton_WhenCalled_ShouldReturnDescriptorsWithSingletonLifetime()
    {
        // Arrange
        var source = new ServiceCollectionSource(
        [
            ServiceDescriptor.Transient<ICustomerService, CustomerService>(),
        ]);

        // Act
        var result = source.AsSingleton();

        // Assert
        var single = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Singleton, single.Lifetime);
    }

    [Fact]
    public void AsSingleton_WhenDescriptorsAlreadySingleton_ShouldReturnSameDescriptors()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Singleton<ICustomerService, CustomerService>();
        var source = new ServiceCollectionSource([descriptor]);

        // Act
        var result = source.AsSingleton();

        // Assert
        var single = Assert.Single(result);
        Assert.Same(descriptor, single);
    }

    [Fact]
    public void AsScoped_WhenCalled_ShouldReturnDescriptorsWithScopedLifetime()
    {
        // Arrange
        var source = new ServiceCollectionSource(
        [
            ServiceDescriptor.Transient<ICustomerService, CustomerService>(),
        ]);

        // Act
        var result = source.AsScoped();

        // Assert
        var single = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Scoped, single.Lifetime);
    }

    [Fact]
    public void AsScoped_WhenIgnoreSingletonIsFalseAndInstanceDescriptor_ShouldThrow()
    {
        // Arrange
        var source = new ServiceCollectionSource(
        [
            ServiceDescriptor.Singleton<ICustomerService>(new CustomerService()),
        ]);

        // Act
        var act = () => source.AsScoped(ignoreSingletonImplementations: false);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AsScoped_WhenIgnoreSingletonIsTrueAndInstanceDescriptor_ShouldKeepDescriptorUnchanged()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Singleton<ICustomerService>(new CustomerService());
        var source = new ServiceCollectionSource([descriptor]);

        // Act
        var result = source.AsScoped(ignoreSingletonImplementations: true);

        // Assert
        var single = Assert.Single(result);
        Assert.Same(descriptor, single);
        Assert.Equal(ServiceLifetime.Singleton, single.Lifetime);
    }

    [Fact]
    public void AsTransient_WhenCalled_ShouldReturnDescriptorsWithTransientLifetime()
    {
        // Arrange
        var source = new ServiceCollectionSource(
        [
            ServiceDescriptor.Scoped<ICustomerService, CustomerService>(),
        ]);

        // Act
        var result = source.AsTransient();

        // Assert
        var single = Assert.Single(result);
        Assert.Equal(ServiceLifetime.Transient, single.Lifetime);
    }

    [Fact]
    public void AsTransient_WhenIgnoreSingletonIsFalseAndInstanceDescriptor_ShouldThrow()
    {
        // Arrange
        var source = new ServiceCollectionSource(
        [
            ServiceDescriptor.Singleton<ICustomerService>(new CustomerService()),
        ]);

        // Act
        var act = () => source.AsTransient(ignoreSingletonImplementations: false);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AsTransient_WhenIgnoreSingletonIsTrueAndInstanceDescriptor_ShouldKeepDescriptorUnchanged()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Singleton<ICustomerService>(new CustomerService());
        var source = new ServiceCollectionSource([descriptor]);

        // Act
        var result = source.AsTransient(ignoreSingletonImplementations: true);

        // Assert
        var single = Assert.Single(result);
        Assert.Same(descriptor, single);
        Assert.Equal(ServiceLifetime.Singleton, single.Lifetime);
    }

    [Fact]
    public void AsLifetime_WhenCalledWithLifetime_ShouldDefaultToIgnoreSingletonImplementations()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Singleton<ICustomerService>(new CustomerService());
        var source = new ServiceCollectionSource([descriptor]);

        // Act
        var result = source.AsLifetime(ServiceLifetime.Scoped);

        // Assert
        var single = Assert.Single(result);
        Assert.Same(descriptor, single);
    }

    [Fact]
    public void AsLifetime_WhenIgnoreFalseAndInstanceDescriptor_ShouldThrow()
    {
        // Arrange
        var source = new ServiceCollectionSource(
        [
            ServiceDescriptor.Singleton<ICustomerService>(new CustomerService()),
        ]);

        // Act
        var act = () => source.AsLifetime(ServiceLifetime.Transient, ignoreSingletonImplementations: false);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AsLifetime_WhenCalled_ShouldReturnNewServiceSource()
    {
        // Arrange
        var source = new ServiceCollectionSource(
        [
            ServiceDescriptor.Transient<ICustomerService, CustomerService>(),
        ]);

        // Act
        var result = source.AsLifetime(ServiceLifetime.Singleton);

        // Assert
        Assert.NotSame(source, result);
    }

    [Fact]
    public void AsLifetime_WhenCalled_ShouldPreserveDescriptorCount()
    {
        // Arrange
        var source = new ServiceCollectionSource(
        [
            ServiceDescriptor.Transient<ICustomerService, CustomerService>(),
            ServiceDescriptor.Transient<ICustomerService, CustomerService>(),
        ]);

        // Act
        var result = source.AsLifetime(ServiceLifetime.Scoped);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void AsLifetime_WhenCalled_ShouldPreserveServiceType()
    {
        // Arrange
        var source = new ServiceCollectionSource(
        [
            ServiceDescriptor.Transient<ICustomerService, CustomerService>(),
        ]);

        // Act
        var result = source.AsLifetime(ServiceLifetime.Scoped);

        // Assert
        var single = Assert.Single(result);
        Assert.Equal(typeof(ICustomerService), single.ServiceType);
        Assert.Equal(typeof(CustomerService), single.ImplementationType);
    }

}
