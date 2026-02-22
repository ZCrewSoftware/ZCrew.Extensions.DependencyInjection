using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Notifications;
using Fixtures.SmallProject.Infrastructure.Persistence;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.ClassesTests;

public class ClassesServiceSelectionTests
{
    [Fact]
    public void AsSelf_WhenCalled_ShouldRegisterAsImplementationType()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsSelf();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ServiceType);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
    }

    [Fact]
    public void AsAllInterfaces_WhenCalled_ShouldRegisterAllInterfaces()
    {
        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(IPaymentGateway), serviceTypes);
        Assert.Contains(typeof(IDisposable), serviceTypes);
        Assert.All(result, d => Assert.Equal(typeof(PayPalPaymentGateway), d.ImplementationType));
    }

    [Fact]
    public void AsAllNonSystemInterfaces_WhenCalled_ShouldExcludeSystemInterfaces()
    {
        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsAllNonSystemInterfaces();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(IPaymentGateway), serviceTypes);
        Assert.DoesNotContain(typeof(IDisposable), serviceTypes);
    }

    [Fact]
    public void AsDefaultInterfaces_WhenCalled_ShouldMatchByNamingConvention()
    {
        // Act
        var result = Classes.From(typeof(CustomerService), typeof(EmailNotificationSender)).AsDefaultInterfaces();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(CustomerService) && d.ServiceType == typeof(ICustomerService)
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(EmailNotificationSender) && d.ServiceType == typeof(INotificationSender)
        );
    }

    [Fact]
    public void AsDefaultInterfaces_WhenNoConventionMatch_ShouldNotRegister()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsDefaultInterfaces();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsDefaultNonSystemInterfaces_WhenCalled_ShouldCombineBothFilters()
    {
        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsDefaultNonSystemInterfaces();

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(IPaymentGateway), serviceTypes);
        Assert.DoesNotContain(typeof(IDisposable), serviceTypes);
    }

    [Fact]
    public void AsFirstInterface_WhenCalled_ShouldRegisterFirstInterface()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).AsFirstInterface();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Contains(descriptor.ServiceType, typeof(CustomerService).GetInterfaces());
    }

    [Fact]
    public void AsFirstInterface_WhenNoInterfaces_ShouldNotRegister()
    {
        // Act
        var result = Classes.From(typeof(Customer)).AsFirstInterface();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AsInterface_WithBasedOn_ShouldRegisterTopLevelDerivedInterfaces()
    {
        // Act
        var result = Classes.From(typeof(SqlCustomerRepository)).BasedOn(typeof(IRepository<>)).AsInterface();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(SqlCustomerRepository), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerRepository), descriptor.ServiceType);
    }

    [Fact]
    public void AsInterface_WithGenericTypeArg_ShouldRegisterDerivedInterfaces()
    {
        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsInterface<IPaymentGateway>();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(PayPalPaymentGateway), descriptor.ImplementationType);
        Assert.Equal(typeof(IPaymentGateway), descriptor.ServiceType);
    }

    [Fact]
    public void AsInterface_WithExplicitType_ShouldRegisterDerivedInterfaces()
    {
        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsInterface(typeof(IPaymentGateway));

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(PayPalPaymentGateway), descriptor.ImplementationType);
        Assert.Equal(typeof(IPaymentGateway), descriptor.ServiceType);
    }

    [Fact]
    public void AsBase_WithBasedOn_ShouldRegisterAsBaseTypes()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).BasedOn<ICustomerService>().AsBase();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerService), descriptor.ServiceType);
    }

    [Fact]
    public void As_WithCustomSelector_ShouldUseProvidedFunction()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).As(type => type.GetInterfaces());

        // Assert
        var serviceTypes = result.Select(d => d.ServiceType).ToArray();
        Assert.Contains(typeof(ICustomerService), serviceTypes);
        Assert.All(result, d => Assert.Equal(typeof(CustomerService), d.ImplementationType));
    }

    [Fact]
    public void As_WithBaseTypeContext_ShouldReceiveResolvedBaseTypes()
    {
        // Act
        var result = Classes.From(typeof(CustomerService)).BasedOn<ICustomerService>().As((_, bases) => bases);

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerService), descriptor.ServiceType);
    }

    [Fact]
    public void AsInterfaces_WithMultipleBaseTypes_ShouldRegisterDerivedInterfacesFromAll()
    {
        // Act
        var result = Classes
            .From(typeof(CustomerService), typeof(OrderService))
            .AsInterfaces(typeof(ICustomerService), typeof(IOrderService));

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(CustomerService) && d.ServiceType == typeof(ICustomerService)
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(OrderService) && d.ServiceType == typeof(IOrderService)
        );
    }

    [Fact]
    public void AsInterfaces_WithOpenGeneric_ShouldRegisterTopLevelDerivedInterfaces()
    {
        // Act
        var result = Classes
            .From(typeof(SqlCustomerRepository), typeof(SqlOrderRepository))
            .AsInterfaces(typeof(IRepository<>));

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
    public void AsInterface_WithOpenGenericTypeArg_ShouldRegisterTopLevelDerivedInterfaces()
    {
        // Act
        var result = Classes.From(typeof(SqlCustomerRepository)).AsInterface(typeof(IRepository<>));

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(SqlCustomerRepository), descriptor.ImplementationType);
        Assert.Equal(typeof(ICustomerRepository), descriptor.ServiceType);
    }
}
