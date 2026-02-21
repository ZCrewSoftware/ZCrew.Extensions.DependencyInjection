using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Domain.Services;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Notifications;
using Fixtures.SmallProject.Infrastructure.Persistence;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.ClassesTests;

public class ClassesEndToEndTests
{
    [Fact]
    public void RepositoryRegistration_FromAssembly_ShouldRegisterAllRepositoryImplementations()
    {
        // Act
        var result = Classes
            .FromAssemblyContaining<SqlCustomerRepository>()
            .BasedOn(typeof(IRepository<>))
            .InNamespace("Fixtures.SmallProject.Infrastructure.Persistence", includeSubnamespaces: true)
            .AsInterface();

        // Assert
        Assert.Contains(
            result,
            d =>
                d.ImplementationType == typeof(SqlCustomerRepository)
                && d.ServiceType == typeof(ICustomerRepository)
        );
        Assert.Contains(
            result,
            d =>
                d.ImplementationType == typeof(SqlOrderRepository)
                && d.ServiceType == typeof(IOrderRepository)
        );
    }

    [Fact]
    public void ServiceRegistration_ByConvention_ShouldRegisterByNamingConvention()
    {
        // Act
        var result = Classes
            .FromAssemblyContaining<CustomerService>()
            .InNamespace("Fixtures.SmallProject.Application.Services")
            .AsDefaultNonSystemInterfaces();

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
                d.ImplementationType == typeof(OrderService)
                && d.ServiceType == typeof(IOrderService)
        );
        Assert.Contains(
            result,
            d =>
                d.ImplementationType == typeof(ProductService)
                && d.ServiceType == typeof(IProductService)
        );
        Assert.DoesNotContain(
            result,
            d => d.ImplementationType == typeof(LegacyOrderProcessor)
        );
    }

    [Fact]
    public void ValidatorRegistration_WithOpenGenericBase_ShouldRegisterClosedImplementations()
    {
        // Act
        var result = Classes
            .FromAssemblyContaining<OrderValidator>()
            .BasedOn(typeof(IValidator<>))
            .InNamespace("Fixtures.SmallProject.Domain.Services")
            .AsInterface();

        // Assert
        Assert.Contains(
            result,
            d =>
                d.ImplementationType == typeof(OrderValidator)
                && d.ServiceType == typeof(IValidator<Order>)
        );
        Assert.Contains(
            result,
            d =>
                d.ImplementationType == typeof(CustomerValidator)
                && d.ServiceType == typeof(IValidator<Customer>)
        );
    }

    [Fact]
    public void InfrastructureRegistration_MultipleInterfaces_ShouldRegisterAllNonSystemInterfaces()
    {
        // Act
        var result = Classes
            .FromAssemblyContaining<PayPalPaymentGateway>()
            .InNamespace("Fixtures.SmallProject.Infrastructure", includeSubnamespaces: true)
            .AsAllNonSystemInterfaces();

        // Assert
        Assert.Contains(
            result,
            d =>
                d.ImplementationType == typeof(PayPalPaymentGateway)
                && d.ServiceType == typeof(IPaymentGateway)
        );
        Assert.Contains(
            result,
            d =>
                d.ImplementationType == typeof(EmailNotificationSender)
                && d.ServiceType == typeof(INotificationSender)
        );
        Assert.DoesNotContain(result, d => d.ServiceType == typeof(IDisposable));
    }
}
