using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Services;
using Fixtures.SmallProject.Infrastructure.External;
using Microsoft.Extensions.DependencyInjection;
using ZCrew.Extensions.DependencyInjection.Registration;

namespace ZCrew.Extensions.DependencyInjection.IntegrationTests.Registration.ClassesTests;

public class ClassesWhereFilterTests
{
    [Fact]
    public void Where_WithPredicate_ShouldFilterTypes()
    {
        // Act
        var result = Classes
            .From(
                typeof(CustomerService),
                typeof(OrderService),
                typeof(OrderValidator),
                typeof(PayPalPaymentGateway)
            )
            .Where(t => t.Name.EndsWith("Service"))
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderService), registeredTypes);
        Assert.DoesNotContain(typeof(OrderValidator), registeredTypes);
        Assert.DoesNotContain(typeof(PayPalPaymentGateway), registeredTypes);
    }

    [Fact]
    public void Where_WithChainedPredicates_ShouldApplyAll()
    {
        // Act
        var result = Classes
            .From(
                typeof(CustomerService),
                typeof(OrderService),
                typeof(ProductService),
                typeof(OrderValidator)
            )
            .Where(t => t.Name.EndsWith("Service"))
            .Where(t => t.Name.StartsWith("Customer"))
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Single(registeredTypes);
        Assert.Contains(typeof(CustomerService), registeredTypes);
    }

    [Fact]
    public void BasedOn_WithInterface_ShouldFilterToImplementors()
    {
        // Act
        var result = Classes
            .From(
                typeof(CustomerService),
                typeof(CachingCustomerService),
                typeof(OrderService),
                typeof(ProductService)
            )
            .BasedOn<ICustomerService>()
            .AsBase();

        // Assert
        var descriptors = result.ToArray();
        Assert.Equal(2, descriptors.Length);
        Assert.All(descriptors, d => Assert.Equal(typeof(ICustomerService), d.ServiceType));
        var implementationTypes = descriptors.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), implementationTypes);
        Assert.Contains(typeof(CachingCustomerService), implementationTypes);
    }

    [Fact]
    public void BasedOn_WithOpenGeneric_ShouldFilterToImplementors()
    {
        // Act
        var result = Classes
            .From(
                typeof(OrderValidator),
                typeof(CustomerValidator),
                typeof(CustomerService),
                typeof(PayPalPaymentGateway)
            )
            .BasedOn(typeof(IValidator<>))
            .AsBase();

        // Assert
        var descriptors = result.ToArray();
        Assert.Equal(2, descriptors.Length);
        Assert.Contains(
            descriptors,
            d =>
                d.ImplementationType == typeof(OrderValidator)
                && d.ServiceType == typeof(IValidator<Order>)
        );
        Assert.Contains(
            descriptors,
            d =>
                d.ImplementationType == typeof(CustomerValidator)
                && d.ServiceType == typeof(IValidator<Customer>)
        );
    }

    [Fact]
    public void BasedOn_WithMultipleTypes_ShouldFilterToUnion()
    {
        // Act
        var result = Classes
            .From(
                typeof(CustomerService),
                typeof(OrderService),
                typeof(ProductService),
                typeof(PayPalPaymentGateway)
            )
            .BasedOn(typeof(ICustomerService), typeof(IOrderService))
            .AsBase();

        // Assert
        var implementationTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), implementationTypes);
        Assert.Contains(typeof(OrderService), implementationTypes);
        Assert.DoesNotContain(typeof(ProductService), implementationTypes);
        Assert.DoesNotContain(typeof(PayPalPaymentGateway), implementationTypes);
    }

    [Fact]
    public void BasedOn_WhenCombinedWithWhere_ShouldApplyBoth()
    {
        // Act
        var result = Classes
            .From(
                typeof(CustomerService),
                typeof(CachingCustomerService),
                typeof(OrderService),
                typeof(ProductService)
            )
            .Where(t => !t.Name.StartsWith("Caching"))
            .BasedOn<ICustomerService>()
            .AsBase();

        // Assert
        var descriptors = result.ToArray();
        Assert.Single(descriptors);
        Assert.Equal(typeof(CustomerService), descriptors[0].ImplementationType);
        Assert.Equal(typeof(ICustomerService), descriptors[0].ServiceType);
    }

    [Fact]
    public void AllTypes_WhenCalled_ShouldIncludeAllSelectedTypes()
    {
        // Act
        var result = Classes
            .From(typeof(CustomerService), typeof(OrderService), typeof(ProductService))
            .AllTypes()
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Equal(3, registeredTypes.Length);
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderService), registeredTypes);
        Assert.Contains(typeof(ProductService), registeredTypes);
    }

    [Fact]
    public void BasedOn_WhenChainedMultipleTimes_ShouldAppendBaseTypes()
    {
        // Act
        var result = Classes
            .From(
                typeof(CustomerService),
                typeof(OrderService),
                typeof(PayPalPaymentGateway)
            )
            .BasedOn<ICustomerService>()
            .BasedOn<IOrderService>()
            .AsBase();

        // Assert
        var implementationTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), implementationTypes);
        Assert.Contains(typeof(OrderService), implementationTypes);
        Assert.DoesNotContain(typeof(PayPalPaymentGateway), implementationTypes);
    }

    [Fact]
    public void BasedOn_WhenChainedWithDifferentOverloads_ShouldAppendAllBaseTypes()
    {
        // Act
        var result = Classes
            .From(
                typeof(CustomerService),
                typeof(OrderValidator),
                typeof(PayPalPaymentGateway)
            )
            .BasedOn<ICustomerService>()
            .BasedOn(typeof(IValidator<>))
            .AsBase();

        // Assert
        Assert.Contains(
            result,
            d =>
                d.ImplementationType == typeof(CustomerService)
                && d.ServiceType == typeof(ICustomerService)
        );
        Assert.Contains(
            result,
            d =>
                d.ImplementationType == typeof(OrderValidator)
                && d.ServiceType == typeof(IValidator<Order>)
        );
        Assert.DoesNotContain(result, d => d.ImplementationType == typeof(PayPalPaymentGateway));
    }

    [Fact]
    public void BasedOn_WhenEnumeratedWithoutTerminalMethod_ShouldDefaultToSelfRegistration()
    {
        // Arrange
        var filter = Classes
            .From(typeof(CustomerService), typeof(OrderService))
            .BasedOn<ICustomerService>();

        // Act
        var result = filter.AsServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ServiceType);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void Where_WhenEnumeratedWithoutTerminalMethod_ShouldDefaultToSelfRegistration()
    {
        var filter = Classes
            .FromAssemblyContaining<CustomerService>()
            .Where(t => t == typeof(CustomerService));

        // Act
        var result = filter.AsServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ServiceType);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }
}
