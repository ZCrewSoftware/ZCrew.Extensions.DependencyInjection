using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Infrastructure.External;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.SharingTests;

public class SharingBehaviorTests
{
    [Fact]
    public void AsSingleton_WhenMultipleServices_ShouldShareSingleInstance()
    {
        // Arrange
        var services = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces().AsSingleton();
        var provider = services.BuildServiceProvider();

        // Act
        var gateway = provider.GetRequiredService<IPaymentGateway>();
        var disposable = provider.GetRequiredService<IDisposable>();

        // Assert
        Assert.Same(gateway, disposable);
    }

    [Fact]
    public void AsScoped_WhenMultipleServices_ShouldShareInstanceWithinScope()
    {
        // Arrange
        var services = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces().AsScoped();
        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var gateway = scope.ServiceProvider.GetRequiredService<IPaymentGateway>();
        var disposable = scope.ServiceProvider.GetRequiredService<IDisposable>();

        // Assert
        Assert.Same(gateway, disposable);
    }

    [Fact]
    public void AsScoped_WhenMultipleServices_ShouldGiveDifferentInstancesAcrossScopes()
    {
        // Arrange
        var services = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces().AsScoped();
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
    public void AsSingletonDependent_WhenImplIsRegisteredViaAsSelf_ShouldShareWithImplRegistration()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        foreach (var d in Classes.From(typeof(PayPalPaymentGateway)).AsSelf().AsSingleton())
        {
            services.Add(d);
        }
        foreach (var d in Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces().AsSingletonDependent())
        {
            services.Add(d);
        }
        var provider = services.BuildServiceProvider();

        // Act
        var direct = provider.GetRequiredService<PayPalPaymentGateway>();
        var viaInterface = provider.GetRequiredService<IPaymentGateway>();

        // Assert
        Assert.Same(direct, viaInterface);
    }

    [Fact]
    public void AsSingletonDependent_WhenImplIsNotRegistered_ShouldThrowAtResolution()
    {
        // Arrange — multiple services force the forwarding path (single-service mappings short-circuit to direct
        // registration regardless of sharing mode, so Dependent's "must have impl registered" contract only kicks in
        // when more than one service is mapped).
        var services = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces().AsSingletonDependent();
        var provider = services.BuildServiceProvider();

        // Act
        var act = () => provider.GetRequiredService<IPaymentGateway>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AsSingletonIndependent_WhenMultipleServices_ShouldGiveEachServiceItsOwnInstance()
    {
        // Arrange
        var services = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces().AsSingletonIndependent();
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
        var services = Classes.From(typeof(PayPalPaymentGateway)).AsAllInterfaces().AsTransient();
        var provider = services.BuildServiceProvider();

        // Act
        var first = provider.GetRequiredService<IPaymentGateway>();
        var second = provider.GetRequiredService<IPaymentGateway>();

        // Assert
        Assert.NotSame(first, second);
    }
}
