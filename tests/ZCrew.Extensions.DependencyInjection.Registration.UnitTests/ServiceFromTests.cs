using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Infrastructure.External;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class ServiceFromTests
{
    [Fact]
    public void From_WhenNoServiceTypes_ShouldSeedImplementationAsOnlyService()
    {
        // Act
        var service = Service.From(typeof(CustomerService), ServiceLifetime.Singleton, null);

        // Assert
        Assert.Equal(typeof(CustomerService), service.ImplementationType);
        Assert.Equal([typeof(CustomerService)], service.ServiceTypes);
    }

    [Fact]
    public void From_WhenNoLifetimeMarker_ShouldRegisterWithGivenLifetime()
    {
        // Arrange
        var service = Service.From(
            typeof(CustomerService),
            ServiceLifetime.Singleton,
            null,
            (typeof(ICustomerService), null)
        );
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        Assert.NotEmpty(services);
        Assert.All(services, descriptor => Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime));
    }

    [Fact]
    public void From_WhenLifetimeIsScoped_ShouldRegisterWithThatLifetime()
    {
        // Arrange
        var service = Service.From(
            typeof(CustomerService),
            ServiceLifetime.Scoped,
            null,
            (typeof(ICustomerService), null)
        );
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        Assert.All(services, descriptor => Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime));
    }

    [Fact]
    public void From_WhenImplementationKeyed_ShouldKeyTheImplementationRegistration()
    {
        // Arrange
        var service = Service.From(typeof(CustomerService), ServiceLifetime.Singleton, "primary");
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        var descriptor = Assert.Single(services);
        Assert.Equal("primary", descriptor.ServiceKey);
    }

    [Fact]
    public void From_WhenNonStringKey_ShouldPreserveKeyValueAndType()
    {
        // Arrange — the key is lifted verbatim; the boxed runtime type must survive unchanged.
        var service = Service.From(typeof(CustomerService), ServiceLifetime.Singleton, 5L);
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        var descriptor = Assert.Single(services);
        Assert.Equal(5L, descriptor.ServiceKey);
    }

    [Fact]
    public void From_WhenServiceTypeKeyed_ShouldKeyOnlyThatServiceType()
    {
        // Arrange — the implementation is unkeyed; only IPaymentGateway carries the key.
        var service = Service.From(
            typeof(PayPalPaymentGateway),
            ServiceLifetime.Singleton,
            null,
            (typeof(IPaymentGateway), "external")
        );
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        var implementation = Assert.Single(services, d => d.ServiceType == typeof(PayPalPaymentGateway));
        Assert.Null(implementation.ServiceKey);
        var gateway = Assert.Single(services, d => d.ServiceType == typeof(IPaymentGateway));
        Assert.Equal("external", gateway.ServiceKey);
    }

    [Fact]
    public void From_WhenSameServiceTypeAddedWithDistinctKeys_ShouldRegisterEachKey()
    {
        // Arrange
        var service = Service.From(
            typeof(PayPalPaymentGateway),
            ServiceLifetime.Singleton,
            null,
            (typeof(IPaymentGateway), "primary"),
            (typeof(IPaymentGateway), "secondary")
        );
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        var gateways = services.Where(d => d.ServiceType == typeof(IPaymentGateway)).ToList();
        Assert.Equal(2, gateways.Count);
        Assert.Contains(gateways, d => Equals(d.ServiceKey, "primary"));
        Assert.Contains(gateways, d => Equals(d.ServiceKey, "secondary"));
    }

    [Fact]
    public void From_WhenSameServiceTypeAndKeyRepeated_ShouldDeduplicate()
    {
        // Arrange
        var service = Service.From(
            typeof(PayPalPaymentGateway),
            ServiceLifetime.Singleton,
            null,
            (typeof(IPaymentGateway), "primary"),
            (typeof(IPaymentGateway), "primary")
        );
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        Assert.Single(services, d => d.ServiceType == typeof(IPaymentGateway));
    }

    [Fact]
    public void From_WhenSingletonWithServiceType_ShouldForwardServiceToSharedImplementation()
    {
        // Arrange
        var service = Service.From(
            typeof(PayPalPaymentGateway),
            ServiceLifetime.Singleton,
            null,
            (typeof(IPaymentGateway), null)
        );
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert — the implementation registers directly and the service type forwards to it.
        Assert.Equal(2, services.Count);
        var implementation = Assert.Single(services, d => d.ServiceType == typeof(PayPalPaymentGateway));
        Assert.Equal(typeof(PayPalPaymentGateway), implementation.ImplementationType);
        Assert.Null(implementation.ImplementationFactory);
        var forwarded = Assert.Single(services, d => d.ServiceType == typeof(IPaymentGateway));
        Assert.NotNull(forwarded.ImplementationFactory);
        Assert.Null(forwarded.ImplementationType);
    }

    [Fact]
    public void From_WhenTransientWithServiceType_ShouldRegisterEachServiceDirectly()
    {
        // Arrange
        var service = Service.From(
            typeof(PayPalPaymentGateway),
            ServiceLifetime.Transient,
            null,
            (typeof(IPaymentGateway), null)
        );
        var services = new ServiceCollection();

        // Act
        services.Add(service);

        // Assert
        Assert.All(services, d => Assert.Equal(typeof(PayPalPaymentGateway), d.ImplementationType));
        Assert.All(services, d => Assert.Null(d.ImplementationFactory));
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void From_WhenImplementationKeyedWithServiceType_ShouldShareOneInstanceAcrossKeys()
    {
        // Arrange — the implementation is keyed "impl"; IPaymentGateway is keyed "external" and forwards to it.
        var service = Service.From(
            typeof(PayPalPaymentGateway),
            ServiceLifetime.Singleton,
            "impl",
            (typeof(IPaymentGateway), "external")
        );
        var services = new ServiceCollection();
        services.Add(service);

        // Act
        var provider = services.BuildServiceProvider();

        // Assert
        var implementation = provider.GetRequiredKeyedService<PayPalPaymentGateway>("impl");
        var gateway = provider.GetRequiredKeyedService<IPaymentGateway>("external");
        Assert.Same(implementation, gateway);
    }
}
