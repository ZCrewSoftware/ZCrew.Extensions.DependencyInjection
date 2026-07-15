using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class SharedComponentTests
{
    [Fact]
    public void AsSingleton_WhenSingleService_ShouldRegisterDirectly()
    {
        // Arrange
        var source = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();

        // Act
        var result = source.AsSingleton().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(ICustomerService), descriptor.ServiceType);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Null(descriptor.ServiceKey);
    }

    [Fact]
    public void AsSingleton_WhenImplIsSelectedAmongMultipleServices_ShouldForwardOtherServicesToImpl()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).As(type => type.GetInterfaces().Prepend(type));

        // Act
        var result = source.AsSingleton().ToServiceCollection();

        // Assert
        var impl = Assert.Single(result, d => d.ServiceType == typeof(PayPalPaymentGateway));
        Assert.Equal(typeof(PayPalPaymentGateway), impl.ImplementationType);
        Assert.Null(impl.ImplementationFactory);

        var forwarded = result.Where(d => d.ServiceType != typeof(PayPalPaymentGateway)).ToArray();
        Assert.NotEmpty(forwarded);
        Assert.All(forwarded, d => Assert.NotNull(d.ImplementationFactory));
        Assert.All(forwarded, d => Assert.Null(d.ImplementationType));
        Assert.Contains(forwarded, d => d.ServiceType == typeof(IPaymentGateway));
        Assert.Contains(forwarded, d => d.ServiceType == typeof(IDisposable));
    }

    [Fact]
    public void AsSingleton_WhenImplIsNotSelected_ShouldRegisterEachServiceDirectly()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces();

        // Act
        var result = source.AsSingleton().ToServiceCollection();

        // Assert
        Assert.All(result, d => Assert.Null(d.ImplementationFactory));
        Assert.All(result, d => Assert.Equal(typeof(PayPalPaymentGateway), d.ImplementationType));
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void AsScoped_WhenImplIsSelectedAmongMultipleServices_ShouldForwardWithScopedLifetime()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).As(type => type.GetInterfaces().Prepend(type));

        // Act
        var result = source.AsScoped().ToServiceCollection();

        // Assert
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
        Assert.Contains(result, d => d.ServiceType == typeof(IPaymentGateway) && d.ImplementationFactory != null);
    }

    [Fact]
    public void AsTransient_WhenMultipleServices_ShouldRegisterEachServiceDirectly()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).As(type => type.GetInterfaces().Prepend(type));

        // Act
        var result = source.AsTransient().ToServiceCollection();

        // Assert
        Assert.All(result, d => Assert.Null(d.ImplementationFactory));
        Assert.All(result, d => Assert.Equal(typeof(PayPalPaymentGateway), d.ImplementationType));
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void AsSingleton_WhenImplIsOpenGenericSelectedAmongMultipleServices_ShouldThrow()
    {
        // Arrange
        var source = Classes.From(typeof(InMemoryRepository<>)).As(type => type.GetInterfaces().Prepend(type));

        // Act
        var act = () => source.AsSingleton().ToServiceCollection();

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(act);
        Assert.Contains("Open generic services can not be forwarded", exception.Message);
    }

    [Fact]
    public void AsSingleton_WhenImplIsOpenGenericNotSelected_ShouldRegisterOpenGenericServicesDirectly()
    {
        // Arrange
        var source = Classes.From(typeof(InMemoryRepository<>)).AsAllInterfaces();

        // Act
        var result = source.AsSingleton().ToServiceCollection();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, d => Assert.Equal(typeof(InMemoryRepository<>), d.ImplementationType));
        Assert.Contains(
            result,
            d => d.ServiceType.IsGenericType && d.ServiceType.GetGenericTypeDefinition() == typeof(IRepository<>)
        );
    }

    [Fact]
    public void AsSingleton_WhenSelfChainedWithInterfaces_ShouldForwardInterfacesToSharedSelf()
    {
        // Arrange
        var source = Classes.From(typeof(SqlCustomerRepository)).AsSelf().AsAllInterfaces();

        // Act
        var result = source.AsSingleton().ToServiceCollection();

        // Assert
        Assert.Equal(6, result.Count);
        var impl = Assert.Single(result, d => d.ServiceType == typeof(SqlCustomerRepository));
        Assert.Equal(typeof(SqlCustomerRepository), impl.ImplementationType);
        Assert.Null(impl.ImplementationFactory);

        var forwarded = result.Where(d => d.ServiceType != typeof(SqlCustomerRepository)).ToArray();
        Assert.Equal(5, forwarded.Length);
        Assert.All(forwarded, d => Assert.NotNull(d.ImplementationFactory));
        Assert.All(forwarded, d => Assert.Null(d.ImplementationType));
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
        Assert.Contains(forwarded, d => d.ServiceType == typeof(ICustomerRepository));
        Assert.Contains(forwarded, d => d.ServiceType == typeof(IDisposable));
    }

    [Fact]
    public void AsSingleton_WhenInterfaceSelectorsChained_ShouldRegisterDistinctIndependent()
    {
        // Arrange — chaining the same interface selector twice would double every service type; the distinct
        // union collapses them back to the five interfaces, and the impl is not among them so each is independent.
        var source = Classes.From(typeof(SqlCustomerRepository)).AsAllInterfaces().AsAllInterfaces();

        // Act
        var result = source.AsSingleton().ToServiceCollection();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(5, result.Select(d => d.ServiceType).Distinct().Count());
        Assert.All(result, d => Assert.Equal(typeof(SqlCustomerRepository), d.ImplementationType));
        Assert.All(result, d => Assert.Null(d.ImplementationFactory));
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }
}
