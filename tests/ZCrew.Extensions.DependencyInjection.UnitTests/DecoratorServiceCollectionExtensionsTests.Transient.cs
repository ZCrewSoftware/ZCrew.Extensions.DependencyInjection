using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.UnitTests;

public partial class DecoratorServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTransientDecorator_Type_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddTransientDecorator(null!, typeof(AuditServiceDecorator));

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddTransientDecorator_Type_WhenDecoratorTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddTransientDecorator(typeof(IAuditService), (Type)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddTransientDecorator_GenericFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddTransientDecorator<IAuditService>(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddTransientDecorator_TypeFactory_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddTransientDecorator(null!, (_, s) => s);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddTransientDecorator_TypeFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddTransientDecorator(typeof(IAuditService), (Func<IServiceProvider, object, object>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedTransientDecorator_Type_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKeyedTransientDecorator(null!, typeof(AuditServiceDecorator), "key");

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedTransientDecorator_Type_WhenDecoratorTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKeyedTransientDecorator(typeof(IAuditService), null!, "key");

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedTransientDecorator_GenericFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKeyedTransientDecorator<IAuditService>("key", null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedTransientDecorator_TypeFactory_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddKeyedTransientDecorator(null!, "key", (_, s, _) => s);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedTransientDecorator_TypeFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddKeyedTransientDecorator(
                typeof(IAuditService),
                "key",
                null!
            );

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddTransientDecorator_Generic_WhenCalled_ShouldAddDescriptorWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IAuditService, AuditService>();

        // Act
        var result = services.AddTransientDecorator<IAuditService, AuditServiceDecorator>();

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, decorator.Lifetime);
    }

    [Fact]
    public void AddTransientDecorator_Type_WhenCalled_ShouldAddDescriptorWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IAuditService, AuditService>();

        // Act
        var result = services.AddTransientDecorator(typeof(IAuditService), typeof(AuditServiceDecorator));

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, decorator.Lifetime);
    }

    [Fact]
    public void AddTransientDecorator_GenericFactory_WhenCalled_ShouldAddDescriptorWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IAuditService, AuditService>();

        // Act
        var result = services.AddTransientDecorator<IAuditService>((_, s) => new AuditServiceDecorator(s));

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, decorator.Lifetime);
    }

    [Fact]
    public void AddTransientDecorator_TypeFactory_WhenCalled_ShouldAddDescriptorWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IAuditService, AuditService>();

        // Act
        var result = services.AddTransientDecorator(
            typeof(IAuditService),
            (_, s) => new AuditServiceDecorator((IAuditService)s)
        );

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedTransientDecorator_Generic_WhenCalled_ShouldAddDescriptorWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedTransient<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedTransientDecorator<IAuditService, AuditServiceDecorator>("key");

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedTransientDecorator_Type_WhenCalled_ShouldAddDescriptorWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedTransientDecorator(typeof(IAuditService), typeof(AuditServiceDecorator), "key");

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedTransientDecorator_GenericFactory_WhenCalled_ShouldAddDescriptorWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedTransientDecorator<IAuditService>(
            "key",
            (_, s, _) => new AuditServiceDecorator(s)
        );

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedTransientDecorator_TypeFactory_WhenCalled_ShouldAddDescriptorWithTransientLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedTransientDecorator(
            typeof(IAuditService),
            "key",
            (_, s, _) => new AuditServiceDecorator((IAuditService)s)
        );

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, decorator.Lifetime);
    }
}
