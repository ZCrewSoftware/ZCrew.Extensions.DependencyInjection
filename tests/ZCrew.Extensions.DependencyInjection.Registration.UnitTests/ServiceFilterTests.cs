using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceFilterTests
{
    [Fact]
    public void ToServiceCollection_WhenCalledWithServices_ShouldAddDescriptorsForEach()
    {
        // Arrange
        var filter = new ServiceFilter(
            [
                Service.From(typeof(CustomerService), new ServiceAttribute(typeof(ICustomerService))),
                Service.From(typeof(OrderService), new ServiceAttribute(typeof(IOrderService))),
            ]
        );
        var services = new ServiceCollection();

        // Act
        filter.ToServiceCollection(services);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(ICustomerService));
        Assert.Contains(services, d => d.ServiceType == typeof(IOrderService));
    }

    [Fact]
    public void ToServiceCollection_WhenCalled_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var filter = new ServiceFilter([Service.From(typeof(CustomerService), new ServiceAttribute())]);
        var services = new ServiceCollection();

        // Act
        var result = filter.ToServiceCollection(services);

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void ToServiceCollection_WhenNoArgument_ShouldReturnNewPopulatedCollection()
    {
        // Arrange
        var filter = new ServiceFilter(
            [Service.From(typeof(CustomerService), new ServiceAttribute(typeof(ICustomerService)))]
        );

        // Act
        var services = filter.ToServiceCollection();

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(ICustomerService));
    }

    [Fact]
    public void ToServiceCollection_WhenSequenceIsEmpty_ShouldAddNothing()
    {
        // Arrange
        var filter = new ServiceFilter([]);
        var services = new ServiceCollection();

        // Act
        filter.ToServiceCollection(services);

        // Assert
        Assert.Empty(services);
    }

    [Fact]
    public void Where_WhenFiltered_ShouldAddOnlyMatchingServices()
    {
        // Arrange — mirrors Services.FromThisAssembly().Where(...).ToServiceCollection(services).
        var filter = new ServiceFilter(
            [
                Service.From(typeof(CustomerService), new ServiceAttribute(typeof(ICustomerService))),
                Service.From(typeof(OrderService), new ServiceAttribute(typeof(IOrderService))),
            ]
        );
        var services = new ServiceCollection();

        // Act
        filter.Where(service => service.ImplementationType == typeof(CustomerService)).ToServiceCollection(services);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(ICustomerService));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IOrderService));
    }

    [Fact]
    public void Add_WhenCalledWithFilter_ShouldAddDescriptors()
    {
        // Arrange
        var filter = new ServiceFilter(
            [
                Service.From(typeof(CustomerService), new ServiceAttribute(typeof(ICustomerService))),
                Service.From(typeof(OrderService), new ServiceAttribute(typeof(IOrderService))),
            ]
        );
        var services = new ServiceCollection();

        // Act
        services.Add(filter.Where(service => service.ImplementationType == typeof(OrderService)));

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(IOrderService));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(ICustomerService));
    }

    [Fact]
    public void BasedOn_WhenFiltered_ShouldKeepOnlyImplementationsBasedOnTheType()
    {
        // Arrange
        var filter = new ServiceFilter(
            [
                Service.From(typeof(CustomerService), new ServiceAttribute(typeof(ICustomerService))),
                Service.From(typeof(OrderService), new ServiceAttribute(typeof(IOrderService))),
            ]
        );
        var services = new ServiceCollection();

        // Act
        filter.BasedOn<ICustomerService>().ToServiceCollection(services);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(ICustomerService));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IOrderService));
    }

    [Fact]
    public void NameEndsWith_WhenFiltered_ShouldKeepMatchingImplementations()
    {
        // Arrange
        var filter = new ServiceFilter(
            [
                Service.From(typeof(CustomerService), new ServiceAttribute(typeof(ICustomerService))),
                Service.From(typeof(OrderService), new ServiceAttribute(typeof(IOrderService))),
            ]
        );
        var services = new ServiceCollection();

        // Act
        filter.NameEndsWith("OrderService").ToServiceCollection(services);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(IOrderService));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(ICustomerService));
    }

    [Fact]
    public void InSameNamespaceAs_WhenFiltered_ShouldKeepImplementationsInThatNamespace()
    {
        // Arrange
        var filter = new ServiceFilter(
            [
                Service.From(typeof(CustomerService), new ServiceAttribute(typeof(ICustomerService))),
                Service.From(typeof(OrderService), new ServiceAttribute(typeof(IOrderService))),
            ]
        );
        var services = new ServiceCollection();

        // Act
        filter.InSameNamespaceAs<CustomerService>().ToServiceCollection(services);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(ICustomerService));
        Assert.Contains(services, d => d.ServiceType == typeof(IOrderService));
    }

    [Fact]
    public void ToServiceCollection_WhenServiceHasKeyAndLifetime_ShouldRegisterAsDeclared()
    {
        // Arrange
        var attribute = new ServiceAttribute(typeof(ICustomerService))
        {
            Lifetime = ServiceLifetime.Scoped,
            Key = "primary",
        };
        var filter = new ServiceFilter([Service.From(typeof(CustomerService), attribute)]);
        var services = new ServiceCollection();

        // Act
        filter.ToServiceCollection(services);

        // Assert
        Assert.NotEmpty(services);
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
        Assert.All(services, d => Assert.Equal("primary", d.ServiceKey));
    }
}
