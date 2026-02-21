using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Notifications;
using Fixtures.SmallProject.Infrastructure.Persistence;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.ClassesTests;

public class ClassesNamespaceFilterTests
{
    [Fact]
    public void InNamespace_WithExactMatch_ShouldFilterToNamespace()
    {
        // Act
        var result = Classes
            .FromAssemblyContaining<CustomerService>()
            .InNamespace("Fixtures.SmallProject.Application.Services")
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderService), registeredTypes);
        Assert.Contains(typeof(ProductService), registeredTypes);
        Assert.DoesNotContain(typeof(PayPalPaymentGateway), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void InNamespace_WithSubnamespaces_ShouldIncludeSubnamespaces()
    {
        // Act
        var result = Classes
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
        var result = Classes
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
        var result = Classes
            .FromAssemblyContaining<CustomerService>()
            .InSameNamespaceAs(typeof(CustomerService))
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderService), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void InSameNamespaceAs_WithGeneric_ShouldFilterToSameNamespace()
    {
        // Act
        var result = Classes
            .FromAssemblyContaining<CustomerService>()
            .InSameNamespaceAs<CustomerService>()
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderService), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void InSameNamespaceAs_WithSubnamespaces_ShouldIncludeSubnamespaces()
    {
        // Act
        var result = Classes
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
        var result = Classes
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
        // Act - .Where() produces a TypeFilter, ensuring TypeFilter.InSameNamespaceAs<T>(bool) is invoked
        var result = Classes
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
