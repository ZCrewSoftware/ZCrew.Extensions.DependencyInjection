using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.UnitTests;

public class EnumerableServiceDescriptorExtensionsTests
{
    [Fact]
    public void AsServiceCollection_WhenCalled_ShouldReturnServiceCollectionContainingAllDescriptors()
    {
        // Arrange
        var descriptor1 = new ServiceDescriptor(
            typeof(ICustomerService),
            typeof(CustomerService),
            ServiceLifetime.Singleton
        );
        var descriptor2 = new ServiceDescriptor(typeof(IOrderService), typeof(OrderService), ServiceLifetime.Transient);
        var descriptors = new[] { descriptor1, descriptor2 }.AsEnumerable();

        // Act
        var result = descriptors.AsServiceCollection();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Same(descriptor1, result[0]);
        Assert.Same(descriptor2, result[1]);
    }

    [Fact]
    public void AsServiceCollection_WhenEmpty_ShouldReturnEmptyServiceCollection()
    {
        // Arrange
        var descriptors = Enumerable.Empty<ServiceDescriptor>();

        // Act
        var result = descriptors.AsServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsServiceCollection_WhenCalled_ShouldReturnServiceCollectionType()
    {
        // Arrange
        var descriptors = new[]
        {
            new ServiceDescriptor(typeof(ICustomerService), typeof(CustomerService), ServiceLifetime.Singleton),
        }.AsEnumerable();

        // Act
        var result = descriptors.AsServiceCollection();

        // Assert
        Assert.IsType<ServiceCollection>(result);
    }
}
