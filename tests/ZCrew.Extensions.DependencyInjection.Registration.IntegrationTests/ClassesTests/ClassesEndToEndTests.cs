using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Domain.Services;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Notifications;
using Fixtures.SmallProject.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            .AsInterface()
            .ToServiceCollection();

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
        var result = Classes
            .FromAssemblyContaining<CustomerService>()
            .InNamespace("Fixtures.SmallProject.Application.Services")
            .AsDefaultNonSystemInterfaces()
            .ToServiceCollection();

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
        var result = Classes
            .FromAssemblyContaining<OrderValidator>()
            .BasedOn(typeof(IValidator<>))
            .Where(t => !t.IsGenericTypeDefinition)
            .InNamespace("Fixtures.SmallProject.Domain.Services")
            .AsInterface()
            .ToServiceCollection();

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
        var result = Classes
            .FromAssemblyContaining<PayPalPaymentGateway>()
            .Where(t => !t.IsGenericTypeDefinition)
            .InNamespace("Fixtures.SmallProject.Infrastructure", includeSubnamespaces: true)
            .AsAllNonSystemInterfaces()
            .ToServiceCollection();

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
    public void ToServiceCollection_WhenGivenExistingCollection_ShouldAppendDescriptorsAndReturnSameInstance()
    {
        // Arrange
        var existing = new ServiceCollection();
        existing.Add(ServiceDescriptor.Singleton<IEventPublisher>(_ => null!));

        // Act
        var result = Classes
            .FromAssemblyContaining<SqlCustomerRepository>()
            .BasedOn(typeof(IRepository<>))
            .InNamespace("Fixtures.SmallProject.Infrastructure.Persistence", includeSubnamespaces: true)
            .AsInterface()
            .Unkeyed()
            .ToServiceCollection(existing);

        // Assert
        Assert.Same(existing, result);
        Assert.Contains(result, d => d.ServiceType == typeof(IEventPublisher));
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
    public void ToServiceCollection_WhenGivenExistingCollection_ShouldRegisterAddedDescriptorsAsSingleton()
    {
        // Arrange
        var existing = new ServiceCollection();

        // Act
        var result = Classes
            .FromAssemblyContaining<SqlCustomerRepository>()
            .BasedOn(typeof(IRepository<>))
            .InNamespace("Fixtures.SmallProject.Infrastructure.Persistence", includeSubnamespaces: true)
            .AsInterface()
            .Unkeyed()
            .ToServiceCollection(existing);

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void ToServiceCollection_WhenCalledTwiceOnSameCollection_ShouldAppendBothBatches()
    {
        // Arrange
        var existing = new ServiceCollection();

        // Act
        Classes
            .FromAssemblyContaining<SqlCustomerRepository>()
            .BasedOn(typeof(IRepository<>))
            .InNamespace("Fixtures.SmallProject.Infrastructure.Persistence", includeSubnamespaces: true)
            .AsInterface()
            .Unkeyed()
            .ToServiceCollection(existing);
        Classes
            .FromAssemblyContaining<CustomerService>()
            .InNamespace("Fixtures.SmallProject.Application.Services")
            .AsDefaultNonSystemInterfaces()
            .Unkeyed()
            .ToServiceCollection(existing);

        // Assert
        Assert.Contains(
            existing,
            d => d.ImplementationType == typeof(SqlCustomerRepository) && d.ServiceType == typeof(ICustomerRepository)
        );
        Assert.Contains(
            existing,
            d => d.ImplementationType == typeof(CustomerService) && d.ServiceType == typeof(ICustomerService)
        );
    }

    [Fact]
    public void ToServiceCollection_WhenCalledOnKeyedServiceSelector_ShouldAppendToExistingCollection()
    {
        // Arrange
        var existing = new ServiceCollection();
        existing.Add(ServiceDescriptor.Singleton<IEventPublisher>(_ => null!));

        // Act
        var result = Classes
            .FromAssemblyContaining<SqlCustomerRepository>()
            .BasedOn(typeof(IRepository<>))
            .InNamespace("Fixtures.SmallProject.Infrastructure.Persistence", includeSubnamespaces: true)
            .AsInterface()
            .ToServiceCollection(existing);

        // Assert
        Assert.Same(existing, result);
        Assert.Contains(result, d => d.ServiceType == typeof(IEventPublisher));
        Assert.Contains(result, d => d.ImplementationType == typeof(SqlCustomerRepository));
    }
}
