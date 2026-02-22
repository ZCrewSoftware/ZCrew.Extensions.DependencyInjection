using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Notifications;
using Fixtures.SmallProject.Infrastructure.Persistence;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.TypesTests;

public class TypesNamespaceFilterTests
{
    [Fact]
    public void InNamespace_WithExactMatch_ShouldFilterToNamespace()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .InNamespace("Fixtures.SmallProject.Application.Services")
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderService), registeredTypes);
        Assert.Contains(typeof(ProductService), registeredTypes);
        Assert.Contains(typeof(ICustomerService), registeredTypes);
        Assert.Contains(typeof(IOrderService), registeredTypes);
        Assert.Contains(typeof(IProductService), registeredTypes);
        Assert.DoesNotContain(typeof(PayPalPaymentGateway), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void InNamespace_WithSubnamespaces_ShouldIncludeSubnamespaces()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .InNamespace("Fixtures.SmallProject.Infrastructure", includeSubnamespaces: true)
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(PayPalPaymentGateway), registeredTypes);
        Assert.Contains(typeof(EmailNotificationSender), registeredTypes);
        Assert.Contains(typeof(SqlCustomerRepository), registeredTypes);
        Assert.DoesNotContain(typeof(CustomerService), registeredTypes);
    }

    [Fact]
    public void InNamespace_WithoutSubnamespaces_ShouldExcludeSubnamespaces()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .InNamespace("Fixtures.SmallProject.Infrastructure")
            .AsSelf();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void InSameNamespaceAs_WithType_ShouldFilterToSameNamespace()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .InSameNamespaceAs(typeof(CustomerService))
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderService), registeredTypes);
        Assert.Contains(typeof(ICustomerService), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void InSameNamespaceAs_WithGeneric_ShouldFilterToSameNamespace()
    {
        // Act
        var result = Types.FromAssemblyContaining<CustomerService>().InSameNamespaceAs<CustomerService>().AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderService), registeredTypes);
        Assert.Contains(typeof(ICustomerService), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void InSameNamespaceAs_WithSubnamespaces_ShouldIncludeSubnamespaces()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .InSameNamespaceAs<CustomerService>(includeSubnamespaces: true)
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(CachingCustomerService), registeredTypes);
    }

    [Fact]
    public void InSameNamespaceAs_WithTypeAndSubnamespaces_ShouldIncludeSubnamespaces()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .InSameNamespaceAs(typeof(PayPalPaymentGateway), includeSubnamespaces: true)
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(PayPalPaymentGateway), registeredTypes);
        Assert.Contains(typeof(StripePaymentGateway), registeredTypes);
        Assert.DoesNotContain(typeof(CustomerService), registeredTypes);
    }

    [Fact]
    public void InSameNamespaceAs_AfterWhere_WithGenericAndSubnamespaces_ShouldFilterCorrectly()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .Where(t => !t.Name.StartsWith("Stripe"))
            .InSameNamespaceAs<PayPalPaymentGateway>(includeSubnamespaces: true)
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(PayPalPaymentGateway), registeredTypes);
        Assert.DoesNotContain(typeof(StripePaymentGateway), registeredTypes);
    }
}
