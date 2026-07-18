using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.SharingTests;

public class ServiceSharingTests
{
    [Fact]
    public void From_WhenServicesAddedWithDefaultLifetime_ShouldShareSingleInstance()
    {
        // Arrange — a service seeds itself as a service, so its services forward to it with no AsSelf equivalent.
        var services = new ServiceCollection();
        services.Add(Service.From<PayPalPaymentGateway>().As<IPaymentGateway, IDisposable>());
        var provider = services.BuildServiceProvider();

        // Act
        var impl = provider.GetRequiredService<PayPalPaymentGateway>();
        var gateway = provider.GetRequiredService<IPaymentGateway>();
        var disposable = provider.GetRequiredService<IDisposable>();

        // Assert
        Assert.Same(impl, gateway);
        Assert.Same(impl, disposable);
    }

    [Fact]
    public void From_WhenScoped_ShouldShareInstanceWithinScope()
    {
        // Arrange
        var services = new ServiceCollection();
        services.Add(
            Service
                .From<PayPalPaymentGateway>()
                .As<IPaymentGateway, IDisposable>()
                .AsLifetime(ServiceLifetime.Scoped)
        );
        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var gateway = scope.ServiceProvider.GetRequiredService<IPaymentGateway>();
        var disposable = scope.ServiceProvider.GetRequiredService<IDisposable>();

        // Assert
        Assert.Same(gateway, disposable);
    }

    [Fact]
    public void From_WhenScoped_ShouldGiveDifferentInstancesAcrossScopes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.Add(
            Service
                .From<PayPalPaymentGateway>()
                .As<IPaymentGateway>()
                .AsLifetime(ServiceLifetime.Scoped)
        );
        var provider = services.BuildServiceProvider();

        // Act
        IPaymentGateway firstScopeGateway;
        IPaymentGateway secondScopeGateway;
        using (var firstScope = provider.CreateScope())
        {
            firstScopeGateway = firstScope.ServiceProvider.GetRequiredService<IPaymentGateway>();
        }
        using (var secondScope = provider.CreateScope())
        {
            secondScopeGateway = secondScope.ServiceProvider.GetRequiredService<IPaymentGateway>();
        }

        // Assert
        Assert.NotSame(firstScopeGateway, secondScopeGateway);
    }

    [Fact]
    public void From_WhenTransient_ShouldGiveEachResolutionNewInstance()
    {
        // Arrange — a transient can never share, so each service type registers independently.
        var services = new ServiceCollection();
        services.Add(
            Service
                .From<PayPalPaymentGateway>()
                .As<IPaymentGateway>()
                .AsLifetime(ServiceLifetime.Transient)
        );
        var provider = services.BuildServiceProvider();

        // Act
        var first = provider.GetRequiredService<IPaymentGateway>();
        var second = provider.GetRequiredService<IPaymentGateway>();

        // Assert
        Assert.NotSame(first, second);
    }

    [Fact]
    public void From_WhenManyServicesAdded_ShouldShareSingleInstanceAcrossAllOfThem()
    {
        // Arrange
        var services = new ServiceCollection();
        services.Add(
            Service
                .From<SqlCustomerRepository>()
                .As<ICustomerRepository, IRepository<Customer>, IReadOnlyRepository<Customer>>()
        );
        var provider = services.BuildServiceProvider();

        // Act
        var impl = provider.GetRequiredService<SqlCustomerRepository>();
        var repository = provider.GetRequiredService<ICustomerRepository>();
        var baseRepository = provider.GetRequiredService<IRepository<Customer>>();
        var readOnlyRepository = provider.GetRequiredService<IReadOnlyRepository<Customer>>();

        // Assert
        Assert.Same(impl, repository);
        Assert.Same(impl, baseRepository);
        Assert.Same(impl, readOnlyRepository);
    }

    [Fact]
    public void From_WhenServicesSelectedByConvention_ShouldShareSingleInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.Add(Service.From<PayPalPaymentGateway>().AsAllNonSystemInterfaces());
        var provider = services.BuildServiceProvider();

        // Act
        var impl = provider.GetRequiredService<PayPalPaymentGateway>();
        var gateway = provider.GetRequiredService<IPaymentGateway>();

        // Assert
        Assert.Same(impl, gateway);
    }

    [Fact]
    public void From_WhenOpenGenericSelectsServicesByConvention_ShouldThrowOnAdd()
    {
        // Arrange — selection itself is fine; forwarding an open generic is what the container cannot do.
        var service = Service.From(typeof(InMemoryRepository<>)).AsAllInterfaces();
        var services = new ServiceCollection();

        // Act
        Action act = () => services.Add(service);

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(act);
        Assert.Contains("Open generic services can not be forwarded", exception.Message);
    }

    [Fact]
    public void From_WhenKeyed_ShouldShareSingleInstanceAcrossKeyedServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.Add(Service.From<PayPalPaymentGateway>().As<IPaymentGateway, IDisposable>().Keyed("PayPal"));
        var provider = services.BuildServiceProvider();

        // Act
        var impl = provider.GetRequiredKeyedService<PayPalPaymentGateway>("PayPal");
        var gateway = provider.GetRequiredKeyedService<IPaymentGateway>("PayPal");
        var disposable = provider.GetRequiredKeyedService<IDisposable>("PayPal");

        // Assert
        Assert.Same(impl, gateway);
        Assert.Same(impl, disposable);
    }
}
