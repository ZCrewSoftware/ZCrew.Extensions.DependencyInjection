using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Persistence;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceExtensionsTypeTests
{
    [Fact]
    public void AsAllTypes_WhenCalled_ShouldAddNonAbstractBaseClassesAndInterfaces()
    {
        // Arrange
        var service = Service.From<SqlCustomerRepository>();

        // Act
        var result = service.AsAllTypes();

        // Assert
        Assert.Equal(typeof(SqlCustomerRepository), result.ServiceTypes[0]);
        Assert.Contains(typeof(ICustomerRepository), result.ServiceTypes);
        Assert.Contains(typeof(IRepository<Customer>), result.ServiceTypes);
        Assert.Contains(typeof(IDisposable), result.ServiceTypes);
        Assert.DoesNotContain(typeof(RepositoryBase<Customer>), result.ServiceTypes);
    }

    [Fact]
    public void AsAllTypes_WhenCalled_ShouldRepeatTheSeededImplementation()
    {
        // Arrange
        var service = Service.From<SqlCustomerRepository>();

        // Act
        var result = service.AsAllTypes();

        // Assert — GetTypes() yields the implementation first, which the service already carries. The duplicate is
        // collapsed when the service is registered.
        Assert.Equal(2, result.ServiceTypes.Count(service => service == typeof(SqlCustomerRepository)));
    }

    [Fact]
    public void AsAllNonSystemTypes_WhenCalled_ShouldExcludeSystemTypesAndAbstractBaseClasses()
    {
        // Arrange
        var service = Service.From<SqlCustomerRepository>();

        // Act
        var result = service.AsAllNonSystemTypes();

        // Assert
        Assert.Contains(typeof(ICustomerRepository), result.ServiceTypes);
        Assert.DoesNotContain(typeof(IDisposable), result.ServiceTypes);
        Assert.DoesNotContain(typeof(IAsyncDisposable), result.ServiceTypes);
        Assert.DoesNotContain(typeof(object), result.ServiceTypes);
        Assert.DoesNotContain(typeof(RepositoryBase<Customer>), result.ServiceTypes);
    }

    [Fact]
    public void AsAllTypes_WhenTypeExtendsConcreteBase_ShouldAddBaseAndInheritedInterfaces()
    {
        // Arrange
        var service = Service.From<CachingPayPalPaymentGateway>();

        // Act
        var result = service.AsAllTypes();

        // Assert
        Assert.Contains(typeof(PayPalPaymentGateway), result.ServiceTypes);
        Assert.Contains(typeof(IPaymentGateway), result.ServiceTypes);
        Assert.Contains(typeof(IDisposable), result.ServiceTypes);
    }

    [Fact]
    public void AsDefaultTypes_WhenCalled_ShouldMatchByNamingConvention()
    {
        // Arrange
        var service = Service.From<CustomerService>();

        // Act
        var result = service.AsDefaultTypes();

        // Assert
        Assert.Contains(typeof(ICustomerService), result.ServiceTypes);
        Assert.DoesNotContain(typeof(object), result.ServiceTypes);
    }

    [Fact]
    public void AsDefaultTypes_WhenNoConventionMatch_ShouldRegisterImplementationAlone()
    {
        // Arrange
        var service = Service.From<Customer>();

        // Act
        var result = service.AsDefaultTypes();

        // Assert — GetTypes() still yields Customer itself, which matches its own name by convention.
        Assert.Equal([typeof(Customer), typeof(Customer)], result.ServiceTypes);
    }

    [Fact]
    public void AsDefaultNonSystemTypes_WhenCalled_ShouldCombineBothFilters()
    {
        // Arrange
        var service = Service.From<SqlCustomerRepository>();

        // Act
        var result = service.AsDefaultNonSystemTypes();

        // Assert
        Assert.Contains(typeof(ICustomerRepository), result.ServiceTypes);
        Assert.DoesNotContain(typeof(IDisposable), result.ServiceTypes);
        Assert.DoesNotContain(typeof(IAsyncDisposable), result.ServiceTypes);
        Assert.DoesNotContain(typeof(RepositoryBase<Customer>), result.ServiceTypes);
    }

    [Fact]
    public void AsDefaultNonSystemTypes_WhenBaseClassNameMatchesConvention_ShouldAddBase()
    {
        // Arrange
        var service = Service.From<CachingPayPalPaymentGateway>();

        // Act
        var result = service.AsDefaultNonSystemTypes();

        // Assert
        Assert.Contains(typeof(PayPalPaymentGateway), result.ServiceTypes);
        Assert.Contains(typeof(IPaymentGateway), result.ServiceTypes);
        Assert.DoesNotContain(typeof(IDisposable), result.ServiceTypes);
    }

    [Fact]
    public void AsAllTypes_WhenCalled_ShouldNotChangeImplementationType()
    {
        // Arrange
        var service = Service.From<SqlCustomerRepository>();

        // Act
        var result = service.AsAllTypes();

        // Assert
        Assert.Equal(typeof(SqlCustomerRepository), result.ImplementationType);
    }
}
