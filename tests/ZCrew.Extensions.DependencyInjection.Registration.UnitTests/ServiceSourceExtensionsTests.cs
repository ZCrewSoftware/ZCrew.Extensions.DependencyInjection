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
    public void AsLifetime_WhenCalledWithLifetime_ShouldDefaultToIgnoreSingletonImplementations()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Singleton<ICustomerService>(new CustomerService());
        IServiceSource source = new ServiceCollectionSource([descriptor]);

        // Act
        var result = source.AsLifetime(ServiceLifetime.Scoped);

        // Assert
        var single = Assert.Single(result);
        Assert.Same(descriptor, single);
    }

    [Fact]
    public void AsLifetime_WhenCalled_ShouldReturnNewServiceSource()
    {
        // Arrange
        IServiceSource source = new ServiceCollectionSource(
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
        IServiceSource source = new ServiceCollectionSource(
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
        IServiceSource source = new ServiceCollectionSource(
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

    [Theory]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void AsLifetime_WhenDescriptorsAlreadyLifetime_ShouldReturnSameDescriptors(ServiceLifetime lifetime)
    {
        // Arrange
        var descriptor = ServiceDescriptor.Describe(typeof(ICustomerService), typeof(CustomerService), lifetime);
        var source = new ServiceCollectionSource([descriptor]);

        // Act
        var result = source.AsLifetime(lifetime);

        // Assert
        var single = Assert.Single(result);
        Assert.Same(descriptor, single);
    }
}
