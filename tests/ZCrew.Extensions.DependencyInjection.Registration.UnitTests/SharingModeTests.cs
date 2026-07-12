using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

public class SharingModeTests
{
    [Fact]
    public void AsSingleton_WhenSingleService_ShouldShortCircuitToDirectRegistration()
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
    public void AsSingleton_WhenMultipleServices_ShouldRegisterHiddenSharedComponentDescriptor()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces();

        // Act
        var result = source.AsSingleton().ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d =>
                d.ServiceType == typeof(PayPalPaymentGateway)
                && d.KeyedImplementationType == typeof(PayPalPaymentGateway)
                && IsSharedComponentKey(d.ServiceKey)
        );
    }

    [Fact]
    public void AsSingleton_WhenMultipleServices_ShouldRegisterFactoryForwardsForEachService()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces();

        // Act
        var result = source.AsSingleton().ToServiceCollection();

        // Assert
        var unkeyed = result.Where(d => d.ServiceKey is null).ToArray();
        Assert.NotEmpty(unkeyed);
        Assert.All(unkeyed, d => Assert.NotNull(d.ImplementationFactory));
        Assert.All(unkeyed, d => Assert.Null(d.ImplementationType));
        Assert.Contains(unkeyed, d => d.ServiceType == typeof(IPaymentGateway));
        Assert.Contains(unkeyed, d => d.ServiceType == typeof(IDisposable));
    }

    [Fact]
    public void AsSingletonDependent_WhenMultipleServices_ShouldRegisterFactoriesWithoutHiddenKey()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces();

        // Act
        var result = source.AsSingletonDependent().ToServiceCollection();

        // Assert
        Assert.DoesNotContain(result, d => IsSharedComponentKey(d.ServiceKey));
        Assert.All(result, d => Assert.NotNull(d.ImplementationFactory));
        Assert.Contains(result, d => d.ServiceType == typeof(IPaymentGateway));
        Assert.Contains(result, d => d.ServiceType == typeof(IDisposable));
    }

    [Fact]
    public void AsSingletonIndependent_WhenMultipleServices_ShouldRegisterEachServiceDirectly()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces();

        // Act
        var result = source.AsSingletonIndependent().ToServiceCollection();

        // Assert
        Assert.DoesNotContain(result, d => IsSharedComponentKey(d.ServiceKey));
        Assert.All(result, d => Assert.Null(d.ImplementationFactory));
        Assert.All(result, d => Assert.Equal(typeof(PayPalPaymentGateway), d.ImplementationType));
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void AsScoped_WhenMultipleServices_ShouldUseSharedComponentDefault()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces();

        // Act
        var result = source.AsScoped().ToServiceCollection();

        // Assert
        Assert.Contains(result, d => IsSharedComponentKey(d.ServiceKey));
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
    }

    [Fact]
    public void AsScopedDependent_WhenMultipleServices_ShouldRegisterFactoriesWithoutHiddenKey()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces();

        // Act
        var result = source.AsScopedDependent().ToServiceCollection();

        // Assert
        Assert.DoesNotContain(result, d => IsSharedComponentKey(d.ServiceKey));
        Assert.All(result, d => Assert.NotNull(d.ImplementationFactory));
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
    }

    [Fact]
    public void AsScopedIndependent_WhenMultipleServices_ShouldRegisterEachServiceDirectly()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces();

        // Act
        var result = source.AsScopedIndependent().ToServiceCollection();

        // Assert
        Assert.DoesNotContain(result, d => IsSharedComponentKey(d.ServiceKey));
        Assert.All(result, d => Assert.Null(d.ImplementationFactory));
        Assert.All(result, d => Assert.Equal(typeof(PayPalPaymentGateway), d.ImplementationType));
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Scoped, d.Lifetime));
    }

    [Fact]
    public void AsTransient_WhenMultipleServices_ShouldRegisterEachServiceDirectly()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces();

        // Act
        var result = source.AsTransient().ToServiceCollection();

        // Assert
        Assert.DoesNotContain(result, d => IsSharedComponentKey(d.ServiceKey));
        Assert.All(result, d => Assert.Null(d.ImplementationFactory));
        Assert.All(result, d => Assert.Equal(typeof(PayPalPaymentGateway), d.ImplementationType));
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    [Fact]
    public void AsLifetime_WhenTransientWithSharedComponent_ShouldThrow()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces();

        // Act
        var act = () => source.AsLifetime(ServiceLifetime.Transient, SharingMode.SharedComponent);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void AsLifetime_WhenTransientWithDependent_ShouldThrow()
    {
        // Arrange
        var source = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces();

        // Act
        var act = () => source.AsLifetime(ServiceLifetime.Transient, SharingMode.Dependent);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void AsSingleton_WhenImplIsOpenGeneric_ShouldThrow()
    {
        // Arrange
        var source = Classes.From(typeof(InMemoryRepository<>)).AsAllInterfaces();

        // Act
        var act = () => source.AsSingleton().ToServiceCollection();

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(act);
        Assert.Contains("Open generic services can not be forwarded", exception.Message);
    }

    [Fact]
    public void AsSingletonDependent_WhenImplIsOpenGeneric_ShouldThrow()
    {
        // Arrange
        var source = Classes.From(typeof(InMemoryRepository<>)).AsAllInterfaces();

        // Act
        var act = () => source.AsSingletonDependent().ToServiceCollection();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AsSingletonIndependent_WhenImplIsOpenGeneric_ShouldRegisterOpenGenericServices()
    {
        // Arrange
        var source = Classes.From(typeof(InMemoryRepository<>)).AsAllInterfaces();

        // Act
        var result = source.AsSingletonIndependent().ToServiceCollection();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, d => Assert.Equal(typeof(InMemoryRepository<>), d.ImplementationType));
        Assert.Contains(
            result,
            d =>
                d.ServiceType.IsGenericType
                && d.ServiceType.GetGenericTypeDefinition() == typeof(IRepository<>)
        );
    }

    private static bool IsSharedComponentKey(object? serviceKey)
    {
        return serviceKey?.ToString()?.StartsWith("ZCrew:SharedComponent:", StringComparison.Ordinal) == true;
    }
}
