using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Infrastructure.External;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.SharingTests;

public class SharingBehaviorTests
{
    [Fact]
    public void AsSingleton_WhenImplIsSelectedAmongMultipleServices_ShouldShareSingleInstance()
    {
        // Arrange
        var services = Classes
            .From(typeof(PayPalPaymentGateway))
            .As(type => type.GetInterfaces().Prepend(type))
            .AsSingleton()
            .ToServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var impl = provider.GetRequiredService<PayPalPaymentGateway>();
        var gateway = provider.GetRequiredService<IPaymentGateway>();

        // Assert
        Assert.Same(impl, gateway);
    }

    [Fact]
    public void AsScoped_WhenImplIsSelectedAmongMultipleServices_ShouldShareInstanceWithinScope()
    {
        // Arrange
        var services = Classes
            .From(typeof(PayPalPaymentGateway))
            .As(type => type.GetInterfaces().Prepend(type))
            .AsScoped()
            .ToServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var impl = scope.ServiceProvider.GetRequiredService<PayPalPaymentGateway>();
        var gateway = scope.ServiceProvider.GetRequiredService<IPaymentGateway>();

        // Assert
        Assert.Same(impl, gateway);
    }

    [Fact]
    public void AsScoped_WhenImplIsSelectedAmongMultipleServices_ShouldGiveDifferentInstancesAcrossScopes()
    {
        // Arrange
        var services = Classes
            .From(typeof(PayPalPaymentGateway))
            .As(type => type.GetInterfaces().Prepend(type))
            .AsScoped()
            .ToServiceCollection();
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
    public void AsSingleton_WhenImplIsNotSelected_ShouldGiveEachServiceItsOwnInstance()
    {
        // Arrange — the implementation is not one of the selected services (only its interfaces are), so each
        // service type is registered independently and no instance is shared.
        var services = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces().AsSingleton().ToServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var gateway = provider.GetRequiredService<IPaymentGateway>();
        var disposable = provider.GetRequiredService<IDisposable>();

        // Assert
        Assert.NotSame(gateway, disposable);
    }

    [Fact]
    public void AsTransient_WhenMultipleServices_ShouldGiveEachResolutionNewInstance()
    {
        // Arrange
        var services = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces().AsTransient().ToServiceCollection();
        var provider = services.BuildServiceProvider();

        // Act
        var first = provider.GetRequiredService<IPaymentGateway>();
        var second = provider.GetRequiredService<IPaymentGateway>();

        // Assert
        Assert.NotSame(first, second);
    }
}
