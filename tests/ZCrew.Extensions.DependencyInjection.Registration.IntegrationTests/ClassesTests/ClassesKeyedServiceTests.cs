using Fixtures.SmallProject.Application.Ports;
using Fixtures.SmallProject.Infrastructure.External;
using Fixtures.SmallProject.Infrastructure.Notifications;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.ClassesTests;

public class ClassesKeyedServiceTests
{
    [Fact]
    public void Keyed_WhenAutoDetected_ShouldKeyByNameConvention()
    {
        // Act
        var result = Classes
            .From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
            .AsInterface<IPaymentGateway>()
            .Keyed()
            .ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d =>
                d.KeyedImplementationType == typeof(PayPalPaymentGateway)
                && d.IsKeyedService
                && Equals(d.ServiceKey, "PayPal")
        );
        Assert.Contains(
            result,
            d =>
                d.KeyedImplementationType == typeof(StripePaymentGateway)
                && d.IsKeyedService
                && Equals(d.ServiceKey, "Stripe")
        );
    }

    [Fact]
    public void Keyed_WhenExplicitKey_ShouldApplySameKeyToAll()
    {
        // Act
        var result = Classes
            .From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
            .AsInterface<IPaymentGateway>()
            .Keyed("myKey")
            .ToServiceCollection();

        // Assert
        Assert.All(result, d => Assert.Equal("myKey", d.ServiceKey));
        Assert.All(result, d => Assert.True(d.IsKeyedService));
    }

    [Fact]
    public void Keyed_WhenNullKey_ShouldNotModifyDescriptors()
    {
        // Act
        var result = Classes
            .From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
            .AsInterface<IPaymentGateway>()
            .Keyed((object?)null)
            .ToServiceCollection();

        // Assert
        Assert.All(result, d => Assert.False(d.IsKeyedService));
    }

    [Fact]
    public void Keyed_WhenFuncOfImplType_ShouldApplyPerTypeKey()
    {
        // Act
        var result = Classes
            .From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
            .AsInterface<IPaymentGateway>()
            .Keyed(type => type.Name)
            .ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d =>
                d.IsKeyedService
                && Equals(d.ServiceKey, "PayPalPaymentGateway")
                && d.KeyedImplementationType == typeof(PayPalPaymentGateway)
        );
        Assert.Contains(
            result,
            d =>
                d.IsKeyedService
                && Equals(d.ServiceKey, "StripePaymentGateway")
                && d.KeyedImplementationType == typeof(StripePaymentGateway)
        );
    }

    [Fact]
    public void Keyed_WhenFuncOfImplAndServiceType_ShouldApplyComputedKey()
    {
        // Act
        var result = Classes
            .From(typeof(EmailNotificationSender), typeof(SmsNotificationSender))
            .AsInterface<INotificationSender>()
            .Keyed((impl, svc) => $"{impl.Name}:{svc.Name}")
            .ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d => d.IsKeyedService && Equals(d.ServiceKey, "EmailNotificationSender:INotificationSender")
        );
        Assert.Contains(
            result,
            d => d.IsKeyedService && Equals(d.ServiceKey, "SmsNotificationSender:INotificationSender")
        );
    }

    [Fact]
    public void Keyed_WhenFuncReturnsNull_ShouldSkipKeying()
    {
        // Act
        var result = Classes
            .From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
            .AsInterface<IPaymentGateway>()
            .Keyed(type => type == typeof(PayPalPaymentGateway) ? "PayPal" : null)
            .ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d =>
                d.IsKeyedService
                && Equals(d.ServiceKey, "PayPal")
                && d.KeyedImplementationType == typeof(PayPalPaymentGateway)
        );
        Assert.Contains(result, d => !d.IsKeyedService && d.ImplementationType == typeof(StripePaymentGateway));
    }

    [Fact]
    public void Keyed_WhenAutoDetectYieldsEmpty_ShouldSkipKeying()
    {
        // Arrange — register PayPalPaymentGateway as itself, so impl name == service name
        // and stripping would yield empty string

        // Act
        var result = Classes.From(typeof(PayPalPaymentGateway)).AsSelf().Keyed().ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.False(descriptor.IsKeyedService);
    }
}
