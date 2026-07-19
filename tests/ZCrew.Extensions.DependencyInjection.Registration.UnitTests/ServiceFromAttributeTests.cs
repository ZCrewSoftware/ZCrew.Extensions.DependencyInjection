using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Infrastructure.External;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceFromAttributeTests
{
    [Fact]
    public void From_WhenAttributeHasNoServiceTypes_ShouldSeedImplementationAsOnlyService()
    {
        // Arrange
        var attribute = new ServiceAttribute();

        // Act
        var service = Service.From(typeof(CustomerService), attribute);

        // Assert
        Assert.Equal(typeof(CustomerService), service.ImplementationType);
        Assert.Equal([typeof(CustomerService)], service.ServiceTypes);
    }

    [Fact]
    public void From_WhenAttributeHasServiceTypes_ShouldSeedImplementationBeforeServices()
    {
        // Arrange
        var attribute = new ServiceAttribute(typeof(ICustomerService));

        // Act
        var service = Service.From(typeof(CustomerService), attribute);

        // Assert
        Assert.Equal([typeof(CustomerService), typeof(ICustomerService)], service.ServiceTypes);
    }

    [Fact]
    public void From_WhenAttributeHasMultipleServiceTypes_ShouldSeedAllInOrder()
    {
        // Arrange
        var attribute = new ServiceAttribute(typeof(IPaymentGateway), typeof(IDisposable));

        // Act
        var service = Service.From(typeof(PayPalPaymentGateway), attribute);

        // Assert
        Assert.Equal(
            [typeof(PayPalPaymentGateway), typeof(IPaymentGateway), typeof(IDisposable)],
            service.ServiceTypes
        );
    }

    [Fact]
    public void From_WhenAttributeHasNoLifetime_ShouldRegisterAsSingleton()
    {
        // Arrange
        var service = Service.From(typeof(CustomerService), new ServiceAttribute(typeof(ICustomerService)));
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        Assert.NotEmpty(services);
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void From_WhenAttributeHasLifetime_ShouldRegisterWithThatLifetime()
    {
        // Arrange
        var attribute = new ServiceAttribute(typeof(ICustomerService)) { Lifetime = ServiceLifetime.Scoped };
        var service = Service.From(typeof(CustomerService), attribute);
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
    }

    [Fact]
    public void From_WhenAttributeHasStringKey_ShouldRegisterKeyedDescriptors()
    {
        // Arrange
        var attribute = new ServiceAttribute(typeof(ICustomerService)) { Key = "primary" };
        var service = Service.From(typeof(CustomerService), attribute);
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        Assert.All(services, d => Assert.Equal("primary", d.ServiceKey));
    }

    [Fact]
    public void From_WhenAttributeHasNonStringKey_ShouldPreserveKeyValueAndType()
    {
        // Arrange — the bridge lifts the key verbatim; the boxed runtime type must survive unchanged.
        var attribute = new ServiceAttribute(typeof(ICustomerService)) { Key = 5L };
        var service = Service.From(typeof(CustomerService), attribute);
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        Assert.All(services, d => Assert.Equal(5L, d.ServiceKey));
    }

    [Fact]
    public void From_WhenSingletonWithMultipleServiceTypes_ShouldForwardServicesToSharedImplementation()
    {
        // Arrange
        var attribute = new ServiceAttribute(typeof(IPaymentGateway));
        var service = Service.From(typeof(PayPalPaymentGateway), attribute);
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert — the implementation registers directly and the interface forwards to it.
        Assert.Equal(2, services.Count);
        var impl = Assert.Single(services, d => d.ServiceType == typeof(PayPalPaymentGateway));
        Assert.Equal(typeof(PayPalPaymentGateway), impl.ImplementationType);
        Assert.Null(impl.ImplementationFactory);
        var forwarded = Assert.Single(services, d => d.ServiceType == typeof(IPaymentGateway));
        Assert.NotNull(forwarded.ImplementationFactory);
        Assert.Null(forwarded.ImplementationType);
    }

    [Fact]
    public void From_WhenTransientWithMultipleServiceTypes_ShouldRegisterEachServiceDirectly()
    {
        // Arrange
        var attribute = new ServiceAttribute(typeof(IPaymentGateway)) { Lifetime = ServiceLifetime.Transient };
        var service = Service.From(typeof(PayPalPaymentGateway), attribute);
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        Assert.All(services, d => Assert.Equal(typeof(PayPalPaymentGateway), d.ImplementationType));
        Assert.All(services, d => Assert.Null(d.ImplementationFactory));
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void ServiceAttribute_WhenAppliedToTypeMultipleTimes_ShouldCompileAndMapEachDeclaration()
    {
        // Arrange — `[Service]` resolves to ServiceAttribute even though a `Service` struct exists (no CS1614).
        var attributes = typeof(Decorated)
            .GetCustomAttributes(typeof(ServiceAttribute), inherit: false)
            .Cast<ServiceAttribute>();

        // Act
        var services = attributes.Select(a => Service.From(typeof(Decorated), a)).ToArray();

        // Assert
        Assert.Equal(2, services.Length);
        Assert.All(services, s => Assert.Equal(typeof(Decorated), s.ImplementationType));
    }

    [Service]
    [Service(typeof(object), Lifetime = ServiceLifetime.Scoped, Key = "decorated")]
    private sealed class Decorated;
}
