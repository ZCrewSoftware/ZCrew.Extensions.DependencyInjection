using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceLifetimeSelectorExtensionsTests
{
    [Fact]
    public void AsSingleton_WhenCalled_ShouldSetAllDescriptorsToSingletonLifetime()
    {
        // Arrange
        var source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();

        // Act
        var result = source.AsSingleton().ToServiceCollection();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void AsScoped_WhenCalled_ShouldSetAllDescriptorsToScopedLifetime()
    {
        // Arrange
        var source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();

        // Act
        var result = source.AsScoped().ToServiceCollection();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
    }

    [Fact]
    public void AsTransient_WhenCalled_ShouldSetAllDescriptorsToTransientLifetime()
    {
        // Arrange
        var source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();

        // Act
        var result = source.AsTransient().ToServiceCollection();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void AsLifetime_WhenCalled_ShouldIncludeRequestedServiceType()
    {
        // Arrange
        var source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();

        // Act
        var result = source.AsLifetime(ServiceLifetime.Scoped).ToServiceCollection();

        // Assert
        Assert.Contains(result, d => d.ServiceType == typeof(ICustomerService));
    }
}
