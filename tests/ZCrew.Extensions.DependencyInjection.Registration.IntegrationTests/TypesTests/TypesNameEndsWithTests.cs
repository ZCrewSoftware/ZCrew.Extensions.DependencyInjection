using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Persistence;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.TypesTests;

public class TypesNameEndsWithTests
{
    [Fact]
    public void NameEndsWith_WithSuffix_ShouldFilterToMatchingTypes()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .NameEndsWith("Service")
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderService), registeredTypes);
        Assert.Contains(typeof(ProductService), registeredTypes);
        Assert.Contains(typeof(ICustomerService), registeredTypes);
        Assert.Contains(typeof(IOrderService), registeredTypes);
        Assert.Contains(typeof(IProductService), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
        Assert.DoesNotContain(typeof(PayPalPaymentGateway), registeredTypes);
    }

    [Fact]
    public void NameEndsWith_WithNoMatches_ShouldReturnEmpty()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .NameEndsWith("Nonexistent")
            .AsSelf();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void NameEndsWith_WithWrongCase_ShouldNotMatch()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .NameEndsWith("service")
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.DoesNotContain(typeof(CustomerService), registeredTypes);
    }

    [Fact]
    public void NameEndsWith_WithIgnoreCaseTrue_ShouldMatchRegardlessOfCase()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .NameEndsWith("service", ignoreCase: true)
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderService), registeredTypes);
        Assert.Contains(typeof(ICustomerService), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void NameEndsWith_WithIgnoreCaseFalse_ShouldRequireExactCase()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .NameEndsWith("service", ignoreCase: false)
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.DoesNotContain(typeof(CustomerService), registeredTypes);
    }

    [Fact]
    public void NameEndsWith_WithOrdinalIgnoreCase_ShouldMatchRegardlessOfCase()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .NameEndsWith("gateway", StringComparison.OrdinalIgnoreCase)
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(PayPalPaymentGateway), registeredTypes);
        Assert.Contains(typeof(StripePaymentGateway), registeredTypes);
        Assert.Contains(typeof(IPaymentGateway), registeredTypes);
        Assert.DoesNotContain(typeof(CustomerService), registeredTypes);
    }

    [Fact]
    public void NameEndsWith_WithGenericTypes_ShouldStripArityBeforeMatching()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .NameEndsWith("Repository")
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(InMemoryRepository<>), registeredTypes);
        Assert.Contains(typeof(SqlCustomerRepository), registeredTypes);
        Assert.Contains(typeof(SqlOrderRepository), registeredTypes);
        Assert.Contains(typeof(IRepository<>), registeredTypes);
    }

    [Fact]
    public void NameEndsWith_AfterWhere_ShouldApplyBothFilters()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .Where(t => !t.Name.StartsWith("Sql"))
            .NameEndsWith("Repository")
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(InMemoryRepository<>), registeredTypes);
        Assert.Contains(typeof(IRepository<>), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
    }
}
