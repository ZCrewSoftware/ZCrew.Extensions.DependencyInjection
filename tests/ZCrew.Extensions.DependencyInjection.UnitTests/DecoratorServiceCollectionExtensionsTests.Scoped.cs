using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.UnitTests;

public partial class DecoratorServiceCollectionExtensionsTests
{
    [Fact]
    public void AddScopedDecorator_Type_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddScopedDecorator(null!, typeof(AuditServiceDecorator));

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddScopedDecorator_Type_WhenDecoratorTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddScopedDecorator(typeof(IAuditService), (Type)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddScopedDecorator_GenericFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddScopedDecorator<IAuditService>(
                null!
            );

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddScopedDecorator_TypeFactory_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddScopedDecorator(null!, (_, s) => s);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddScopedDecorator_TypeFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddScopedDecorator(
                typeof(IAuditService),
                (Func<IServiceProvider, object, object>)null!
            );

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedScopedDecorator_Type_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKeyedScopedDecorator(null!, typeof(AuditServiceDecorator), "key");

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedScopedDecorator_Type_WhenDecoratorTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKeyedScopedDecorator(typeof(IAuditService), null!, "key");

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedScopedDecorator_GenericFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddKeyedScopedDecorator<IAuditService>(
                "key",
                null!
            );

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedScopedDecorator_TypeFactory_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddKeyedScopedDecorator(
                null!,
                "key",
                (_, s, _) => s
            );

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedScopedDecorator_TypeFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddKeyedScopedDecorator(
                typeof(IAuditService),
                "key",
                null!
            );

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddScopedDecorator_Generic_WhenCalled_ShouldAddDescriptorWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IAuditService, AuditService>();

        // Act
        var result = services.AddScopedDecorator<IAuditService, AuditServiceDecorator>();

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, decorator.Lifetime);
    }

    [Fact]
    public void AddScopedDecorator_Type_WhenCalled_ShouldAddDescriptorWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IAuditService, AuditService>();

        // Act
        var result = services.AddScopedDecorator(typeof(IAuditService), typeof(AuditServiceDecorator));

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, decorator.Lifetime);
    }

    [Fact]
    public void AddScopedDecorator_GenericFactory_WhenCalled_ShouldAddDescriptorWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IAuditService, AuditService>();

        // Act
        var result = services.AddScopedDecorator<IAuditService>((_, s) => new AuditServiceDecorator(s));

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, decorator.Lifetime);
    }

    [Fact]
    public void AddScopedDecorator_TypeFactory_WhenCalled_ShouldAddDescriptorWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IAuditService, AuditService>();

        // Act
        var result = services.AddScopedDecorator(
            typeof(IAuditService),
            (_, s) => new AuditServiceDecorator((IAuditService)s)
        );

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedScopedDecorator_Generic_WhenCalled_ShouldAddDescriptorWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedScoped<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedScopedDecorator<IAuditService, AuditServiceDecorator>("key");

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedScopedDecorator_Type_WhenCalled_ShouldAddDescriptorWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedScopedDecorator(
            typeof(IAuditService),
            typeof(AuditServiceDecorator),
            "key"
        );

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedScopedDecorator_GenericFactory_WhenCalled_ShouldAddDescriptorWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedScopedDecorator<IAuditService>(
            "key",
            (_, s, _) => new AuditServiceDecorator(s)
        );

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Scoped, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedScopedDecorator_TypeFactory_WhenCalled_ShouldAddDescriptorWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedScopedDecorator(
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
