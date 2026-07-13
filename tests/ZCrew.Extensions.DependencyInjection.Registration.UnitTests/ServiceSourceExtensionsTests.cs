using Fixtures.SmallProject.Application.Services;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceSourceExtensionsTests
{
    [Fact]
    public void ToServiceCollection_WhenCalled_ShouldReturnCollectionContainingChainDescriptors()
    {
        // Arrange
        var source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>().Unkeyed();

        // Act
        var result = source.ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(ICustomerService), descriptor.ServiceType);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
    }

    [Fact]
    public void ToServiceCollection_WhenCalledTwice_ShouldReturnIndependentCollections()
    {
        // Arrange
        var source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>().Unkeyed();

        // Act
        var first = source.ToServiceCollection();
        var second = source.ToServiceCollection();

        // Assert
        Assert.NotSame(first, second);
        Assert.Equal(first.Count, second.Count);
    }
}
