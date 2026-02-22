using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Domain.Services;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Notifications;
using Fixtures.SmallProject.Infrastructure.Persistence;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.TypesTests;

public class TypesEndToEndTests
{
    [Fact]
    public void RepositoryRegistration_FromAssembly_ShouldRegisterAllRepositoryImplementations()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<SqlCustomerRepository>()
            .BasedOn(typeof(IRepository<>))
            .InNamespace("Fixtures.SmallProject.Infrastructure.Persistence", includeSubnamespaces: true)
            .AsInterface();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(SqlCustomerRepository) && d.ServiceType == typeof(ICustomerRepository)
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(SqlOrderRepository) && d.ServiceType == typeof(IOrderRepository)
        );
    }

    [Fact]
    public void ServiceRegistration_ByConvention_ShouldRegisterByNamingConvention()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .InNamespace("Fixtures.SmallProject.Application.Services")
            .AsDefaultNonSystemInterfaces();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(CustomerService) && d.ServiceType == typeof(ICustomerService)
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(OrderService) && d.ServiceType == typeof(IOrderService)
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(ProductService) && d.ServiceType == typeof(IProductService)
        );
        Assert.DoesNotContain(result, d => d.ImplementationType == typeof(LegacyOrderProcessor));
    }

    [Fact]
    public void ValidatorRegistration_WithOpenGenericBase_ShouldRegisterClosedImplementations()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<OrderValidator>()
            .BasedOn(typeof(IValidator<>))
            .InNamespace("Fixtures.SmallProject.Domain.Services")
            .AsInterface();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(OrderValidator) && d.ServiceType == typeof(IValidator<Order>)
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(CustomerValidator) && d.ServiceType == typeof(IValidator<Customer>)
        );
    }

    [Fact]
    public void InfrastructureRegistration_MultipleInterfaces_ShouldRegisterAllNonSystemInterfaces()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<PayPalPaymentGateway>()
            .InNamespace("Fixtures.SmallProject.Infrastructure", includeSubnamespaces: true)
            .AsAllNonSystemInterfaces();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(PayPalPaymentGateway) && d.ServiceType == typeof(IPaymentGateway)
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(EmailNotificationSender) && d.ServiceType == typeof(INotificationSender)
        );
        Assert.DoesNotContain(result, d => d.ServiceType == typeof(IDisposable));
    }

    [Fact]
    public void InterfaceDiscovery_FromAssembly_ShouldRegisterAllInterfacesInNamespace()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .Where(t => t.IsInterface)
            .InNamespace("Fixtures.SmallProject.Application.Services")
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(ICustomerService), registeredTypes);
        Assert.Contains(typeof(IOrderService), registeredTypes);
        Assert.Contains(typeof(IProductService), registeredTypes);
        Assert.DoesNotContain(typeof(CustomerService), registeredTypes);
    }

    [Fact]
    public void AllTypesInNamespace_FromAssembly_ShouldIncludeInterfacesAndStaticAndConcreteClasses()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<OrderValidator>()
            .InNamespace("Fixtures.SmallProject.Domain.Services")
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(IValidator<>), registeredTypes);
        Assert.Contains(typeof(IPricingStrategy), registeredTypes);
        Assert.Contains(typeof(PricingDefaults), registeredTypes);
        Assert.Contains(typeof(OrderValidator), registeredTypes);
        Assert.Contains(typeof(CustomerValidator), registeredTypes);
    }
}
