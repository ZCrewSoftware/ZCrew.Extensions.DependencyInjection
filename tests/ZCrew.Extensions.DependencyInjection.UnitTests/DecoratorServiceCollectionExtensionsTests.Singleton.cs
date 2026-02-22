using Fixtures.SmallProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.UnitTests;

public partial class DecoratorServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSingletonDecorator_Type_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddSingletonDecorator(null!, typeof(AuditServiceDecorator));

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddSingletonDecorator_Type_WhenDecoratorTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddSingletonDecorator(typeof(IAuditService), (Type)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddSingletonDecorator_GenericFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddSingletonDecorator<IAuditService>(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddSingletonDecorator_TypeFactory_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddSingletonDecorator(null!, (_, s) => s);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddSingletonDecorator_TypeFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () =>
            services.AddSingletonDecorator(typeof(IAuditService), (Func<IServiceProvider, object, object>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedSingletonDecorator_Type_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKeyedSingletonDecorator(null!, typeof(AuditServiceDecorator), "key");

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedSingletonDecorator_Type_WhenDecoratorTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKeyedSingletonDecorator(typeof(IAuditService), null!, "key");

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedSingletonDecorator_GenericFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKeyedSingletonDecorator<IAuditService>("key", null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedSingletonDecorator_TypeFactory_WhenServiceTypeIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKeyedSingletonDecorator(null!, "key", (_, s, _) => s);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddKeyedSingletonDecorator_TypeFactory_WhenFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKeyedSingletonDecorator(typeof(IAuditService), "key", null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddSingletonDecorator_Generic_WhenCalled_ShouldAddDescriptorWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IAuditService, AuditService>();

        // Act
        var result = services.AddSingletonDecorator<IAuditService, AuditServiceDecorator>();

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, decorator.Lifetime);
    }

    [Fact]
    public void AddSingletonDecorator_Type_WhenCalled_ShouldAddDescriptorWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IAuditService, AuditService>();

        // Act
        var result = services.AddSingletonDecorator(typeof(IAuditService), typeof(AuditServiceDecorator));

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, decorator.Lifetime);
    }

    [Fact]
    public void AddSingletonDecorator_GenericFactory_WhenCalled_ShouldAddDescriptorWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IAuditService, AuditService>();

        // Act
        var result = services.AddSingletonDecorator<IAuditService>((_, s) => new AuditServiceDecorator(s));

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, decorator.Lifetime);
    }

    [Fact]
    public void AddSingletonDecorator_TypeFactory_WhenCalled_ShouldAddDescriptorWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IAuditService, AuditService>();

        // Act
        var result = services.AddSingletonDecorator(
            typeof(IAuditService),
            (_, s) => new AuditServiceDecorator((IAuditService)s)
        );

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is null);
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedSingletonDecorator_Generic_WhenCalled_ShouldAddDescriptorWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedSingletonDecorator<IAuditService, AuditServiceDecorator>("key");

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedSingletonDecorator_Type_WhenCalled_ShouldAddDescriptorWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedSingletonDecorator(typeof(IAuditService), typeof(AuditServiceDecorator), "key");

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedSingletonDecorator_GenericFactory_WhenCalled_ShouldAddDescriptorWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedSingletonDecorator<IAuditService>(
            "key",
            (_, s, _) => new AuditServiceDecorator(s)
        );

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, decorator.Lifetime);
    }

    [Fact]
    public void AddKeyedSingletonDecorator_TypeFactory_WhenCalled_ShouldAddDescriptorWithSingletonLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddKeyedSingleton<IAuditService, AuditService>("key");

        // Act
        var result = services.AddKeyedSingletonDecorator(
            typeof(IAuditService),
            "key",
            (_, s, _) => new AuditServiceDecorator((IAuditService)s)
        );

        // Assert
        Assert.Same(services, result);
        var decorator = Assert.Single(services, s => s.ServiceKey is "key");
        Assert.Equal(typeof(IAuditService), decorator.ServiceType);
        Assert.Equal(ServiceLifetime.Singleton, decorator.Lifetime);
    }
}
