using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.UnitTests;

public class ServiceTimelineExtensionsTests
{
    [Theory]
    [InlineData(ServiceLifetime.Singleton, ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Singleton, ServiceLifetime.Transient)]
    [InlineData(ServiceLifetime.Scoped, ServiceLifetime.Transient)]
    public void Exceeds_WhenLifetimeIsLonger_ShouldReturnTrue(ServiceLifetime lifetime, ServiceLifetime other)
    {
        // Act
        var result = lifetime.Exceeds(other);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(ServiceLifetime.Singleton, ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped, ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Scoped, ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient, ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Transient, ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient, ServiceLifetime.Transient)]
    public void Exceeds_WhenLifetimeIsSameOrShorter_ShouldReturnFalse(ServiceLifetime lifetime, ServiceLifetime other)
    {
        // Act
        var result = lifetime.Exceeds(other);

        // Assert
        Assert.False(result);
    }
}
