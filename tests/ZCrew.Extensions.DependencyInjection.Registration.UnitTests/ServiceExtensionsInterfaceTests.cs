using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.ValueObjects;
using Fixtures.SmallProject.Infrastructure.External;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceExtensionsInterfaceTests
{
    [Fact]
    public void AsAllInterfaces_WhenCalled_ShouldKeepImplementationFirstThenAddInterfaces()
    {
        // Arrange
        var service = Service.From<PayPalPaymentGateway>();

        // Act
        var result = service.AsAllInterfaces();

        // Assert — a service is seeded with its implementation, so selection accumulates onto it rather than
        // replacing it the way the Classes chain does.
        Assert.Equal(typeof(PayPalPaymentGateway), result.ServiceTypes[0]);
        Assert.Contains(typeof(IPaymentGateway), result.ServiceTypes);
        Assert.Contains(typeof(IDisposable), result.ServiceTypes);
    }

    [Fact]
    public void AsAllInterfaces_WhenNoInterfaces_ShouldRegisterImplementationAlone()
    {
        // Arrange
        var service = Service.From<Customer>();

        // Act
        var result = service.AsAllInterfaces();

        // Assert — the chain registers nothing here; a service always keeps its implementation.
        Assert.Equal([typeof(Customer)], result.ServiceTypes);
    }

    [Fact]
    public void AsAllNonSystemInterfaces_WhenCalled_ShouldExcludeSystemInterfaces()
    {
        // Arrange
        var service = Service.From<PayPalPaymentGateway>();

        // Act
        var result = service.AsAllNonSystemInterfaces();

        // Assert
        Assert.Equal([typeof(PayPalPaymentGateway), typeof(IPaymentGateway)], result.ServiceTypes);
    }

    [Fact]
    public void AsAllNonSystemInterfaces_WhenOnlySystemInterfaces_ShouldRegisterImplementationAlone()
    {
        // Arrange
        var service = Service.From<Address>();

        // Act
        var result = service.AsAllNonSystemInterfaces();

        // Assert
        Assert.Equal([typeof(Address)], result.ServiceTypes);
    }

    [Fact]
    public void AsDefaultInterfaces_WhenCalled_ShouldMatchByNamingConvention()
    {
        // Arrange
        var service = Service.From<CustomerService>();

        // Act
        var result = service.AsDefaultInterfaces();

        // Assert
        Assert.Equal([typeof(CustomerService), typeof(ICustomerService)], result.ServiceTypes);
    }

    [Fact]
    public void AsDefaultInterfaces_WhenNoConventionMatch_ShouldRegisterImplementationAlone()
    {
        // Arrange
        var service = Service.From<LegacyOrderProcessor>();

        // Act
        var result = service.AsDefaultInterfaces();

        // Assert
        Assert.Equal([typeof(LegacyOrderProcessor)], result.ServiceTypes);
    }

    [Fact]
    public void AsDefaultNonSystemInterfaces_WhenCalled_ShouldCombineBothFilters()
    {
        // Arrange
        var service = Service.From<PayPalPaymentGateway>();

        // Act
        var result = service.AsDefaultNonSystemInterfaces();

        // Assert
        Assert.Equal([typeof(PayPalPaymentGateway), typeof(IPaymentGateway)], result.ServiceTypes);
    }

    [Fact]
    public void AsFirstInterface_WhenCalled_ShouldAddFirstInterface()
    {
        // Arrange
        var service = Service.From<CustomerService>();

        // Act
        var result = service.AsFirstInterface();

        // Assert
        Assert.Equal(2, result.ServiceTypes.Count);
        Assert.Equal(typeof(CustomerService), result.ServiceTypes[0]);
        Assert.Contains(result.ServiceTypes[1], typeof(CustomerService).GetInterfaces());
    }

    [Fact]
    public void AsFirstInterface_WhenNoInterfaces_ShouldRegisterImplementationAlone()
    {
        // Arrange
        var service = Service.From<Customer>();

        // Act
        var result = service.AsFirstInterface();

        // Assert
        Assert.Equal([typeof(Customer)], result.ServiceTypes);
    }

    [Fact]
    public void AsAllInterfaces_WhenChainedWithAsDefaultInterfaces_ShouldAccumulateInFirstOccurrenceOrder()
    {
        // Arrange
        var service = Service.From<PayPalPaymentGateway>();

        // Act
        var result = service.AsDefaultInterfaces().AsAllInterfaces();

        // Assert — the implementation then the convention match lead; AsAllInterfaces appends the rest in
        // reflection order. Duplicates are kept on the service and collapsed when it is registered.
        Assert.Equal(typeof(PayPalPaymentGateway), result.ServiceTypes[0]);
        Assert.Equal(typeof(IPaymentGateway), result.ServiceTypes[1]);
        Assert.Equal(4, result.ServiceTypes.Count);
        Assert.Equal(2, result.ServiceTypes.Count(service => service == typeof(IPaymentGateway)));
        Assert.Contains(typeof(IDisposable), result.ServiceTypes);
    }

    [Fact]
    public void AsAllInterfaces_WhenCalled_ShouldNotChangeImplementationType()
    {
        // Arrange
        var service = Service.From<PayPalPaymentGateway>();

        // Act
        var result = service.AsAllInterfaces();

        // Assert
        Assert.Equal(typeof(PayPalPaymentGateway), result.ImplementationType);
    }
}
