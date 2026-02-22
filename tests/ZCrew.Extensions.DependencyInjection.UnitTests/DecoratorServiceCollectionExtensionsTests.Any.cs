using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.UnitTests;

public partial class DecoratorServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDecorator_Type_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new
            ServiceCollection();

        // Act
        var act = () => services.AddDecorator(null!, typeof(AuditServiceDecorator));

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddDecorator_Type_WhenDecoratorTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddDecorator(typeof(IAuditService), (Type)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddDecorator_GenericFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddDecorator<IAuditService>(
                null!
            );

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddDecorator_TypeFactory_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddDecorator(null!, (_, s) => s);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddDecorator_TypeFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddDecorator(typeof(IAuditService), (Func<IServiceProvider, object, object>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedDecorator_Type_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKeyedDecorator(null!, typeof(AuditServiceDecorator), "key");

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedDecorator_Type_WhenDecoratorTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKeyedDecorator(typeof(IAuditService), null!, "key");

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedDecorator_GenericFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddKeyedDecorator<IAuditService>(
                "key",
                null!
            );

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedDecorator_TypeFactory_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddKeyedDecorator(null!, "key", (_, s, _) => s);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedDecorator_TypeFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddKeyedDecorator(
                typeof(IAuditService),
                "key",
                null!
            );

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddDecorator_Generic_WhenCalled_ShouldAddDescriptorWithInheritedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IAuditService, AuditService>();

        // Act
        var result = services.AddDecorator<IAuditService, AuditServiceDecorator>();

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Transient, decorator.Lifetime);
    }

    [Fact]
    public void AddDecorator_Type_WhenCalled_ShouldAddDescriptorWithInheritedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IAuditService, AuditService>();

        // Act
        var result = services.AddDecorator(typeof(IAuditService), typeof(AuditServiceDecorator));

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, decorator.Lifetime);
    }

    [Fact]
    public void AddDecorator_GenericFactory_WhenCalled_ShouldAddDescriptorWithInheritedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IAuditService, AuditService>();

        // Act
        var result = services.AddDecorator<IAuditService>((_, s) => new AuditServiceDecorator(s));

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, decorator.Lifetime);
    }

    [Fact]
    public void AddDecorator_TypeFactory_WhenCalled_ShouldAddDescriptorWithInheritedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IAuditService, AuditService>();

        // Act
        var result = services.AddDecorator(
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
    public void AddKeyedDecorator_Generic_WhenCalled_ShouldAddDescriptorWithInheritedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedScoped<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedDecorator<IAuditService, AuditServiceDecorator>("key");

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedDecorator_Type_WhenCalled_ShouldAddDescriptorWithInheritedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedDecorator(typeof(IAuditService), typeof(AuditServiceDecorator), "key");

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedDecorator_GenericFactory_WhenCalled_ShouldAddDescriptorWithInheritedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedTransient<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedDecorator<IAuditService>(
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
    public void AddKeyedDecorator_TypeFactory_WhenCalled_ShouldAddDescriptorWithInheritedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedScoped<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedDecorator(
            typeof(IAuditService),
            "key",
            (_, s, _) => new AuditServiceDecorator((IAuditService)s)
        );

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, decorator.Lifetime);
    }
}
