using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.GeneratorTests;

public interface IGeneratedFoo;

public interface IGeneratedBar;

public interface IGeneratedEmail;

public interface IGeneratedHealthCheck;

public interface IGeneratedDatabaseHealthCheck;

[Service]
public class GeneratedSelfService;

[Service, As<IGeneratedFoo>]
public class GeneratedFooService : IGeneratedFoo;

[Service, Scoped, As<IGeneratedBar>]
public class GeneratedScopedBar : IGeneratedBar;

[Service, As<IGeneratedEmail>("smtp")]
public class GeneratedSmtpEmail : IGeneratedEmail;

[Service, As<IGeneratedEmail>("ses")]
public class GeneratedSesEmail : IGeneratedEmail;

[Service]
[As<IGeneratedHealthCheck>("Database"), As<IGeneratedDatabaseHealthCheck>]
public class GeneratedDatabaseHealthCheck : IGeneratedHealthCheck, IGeneratedDatabaseHealthCheck;

/// <summary>
///     Drives the generator end-to-end: the <c>[Service]</c> types above are collected by the source generator into
///     the assembly-local <c>Services.FromThisAssembly()</c> entry point, which these tests filter and add to a real
///     container.
/// </summary>
public class ServiceGeneratorTests
{
    [Fact]
    public void FromThisAssembly_ForSelfService_ShouldRegisterAndResolveImplementation()
    {
        // Arrange
        var services = new ServiceCollection();
        Services
            .FromThisAssembly()
            .Where(service => service.ImplementationType == typeof(GeneratedSelfService))
            .ToServiceCollection(services);

        // Act
        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetRequiredService<GeneratedSelfService>());
    }

    [Fact]
    public void FromThisAssembly_ForForwardedService_ShouldResolveServiceAndImplementationToSameInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        Services
            .FromThisAssembly()
            .Where(service => service.ImplementationType == typeof(GeneratedFooService))
            .ToServiceCollection(services);

        // Act
        var provider = services.BuildServiceProvider();

        // Assert
        var foo = provider.GetRequiredService<IGeneratedFoo>();
        var implementation = provider.GetRequiredService<GeneratedFooService>();
        Assert.Same(implementation, foo);
    }

    [Fact]
    public void FromThisAssembly_ForScopedService_ShouldRegisterWithScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        Services
            .FromThisAssembly()
            .Where(service => service.ImplementationType == typeof(GeneratedScopedBar))
            .ToServiceCollection(services);

        // Assert
        Assert.Contains(
            services,
            descriptor =>
                descriptor.ServiceType == typeof(IGeneratedBar) && descriptor.Lifetime == ServiceLifetime.Scoped
        );
    }

    [Fact]
    public void FromThisAssembly_ForKeyedServices_ShouldResolveEachKeyToItsImplementation()
    {
        // Arrange
        var services = new ServiceCollection();
        Services
            .FromThisAssembly()
            .Where(service =>
                service.ImplementationType == typeof(GeneratedSmtpEmail)
                || service.ImplementationType == typeof(GeneratedSesEmail)
            )
            .ToServiceCollection(services);

        // Act
        var provider = services.BuildServiceProvider();

        // Assert
        Assert.IsType<GeneratedSmtpEmail>(provider.GetRequiredKeyedService<IGeneratedEmail>("smtp"));
        Assert.IsType<GeneratedSesEmail>(provider.GetRequiredKeyedService<IGeneratedEmail>("ses"));
    }

    [Fact]
    public void FromThisAssembly_WithNoFilter_ShouldRegisterEveryDeclaredService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        Services.FromThisAssembly().ToServiceCollection(services);

        // Assert
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(GeneratedSelfService));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IGeneratedFoo));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IGeneratedBar));
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IGeneratedEmail) && Equals(descriptor.ServiceKey, "smtp")
        );
        Assert.Contains(
            services,
            descriptor => descriptor.ServiceType == typeof(IGeneratedEmail) && Equals(descriptor.ServiceKey, "ses")
        );
    }

    [Fact]
    public void Add_ForFilteredServices_ShouldRegisterOnlyTheMatchingServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.Add(
            Services.FromThisAssembly().Where(service => service.ImplementationType == typeof(GeneratedFooService))
        );

        // Assert
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IGeneratedFoo));
        Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(IGeneratedBar));
    }

    [Fact]
    public void FromThisAssembly_WithBasedOnFilter_ShouldRegisterOnlyImplementationsOfThatType()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        Services.FromThisAssembly().BasedOn<IGeneratedFoo>().ToServiceCollection(services);

        // Assert
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IGeneratedFoo));
        Assert.DoesNotContain(services, descriptor => descriptor.ServiceType == typeof(IGeneratedEmail));
    }

    [Fact]
    public void FromThisAssembly_ForPerServiceTypeKeys_ShouldResolveKeyedAndUnkeyedToSameInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        Services
            .FromThisAssembly()
            .Where(service => service.ImplementationType == typeof(GeneratedDatabaseHealthCheck))
            .ToServiceCollection(services);

        // Act
        var provider = services.BuildServiceProvider();

        // Assert — the "Database"-keyed health check and the unkeyed database health check are one shared instance.
        var keyed = provider.GetRequiredKeyedService<IGeneratedHealthCheck>("Database");
        var unkeyed = provider.GetRequiredService<IGeneratedDatabaseHealthCheck>();
        Assert.Same(keyed, unkeyed);
        Assert.IsType<GeneratedDatabaseHealthCheck>(keyed);
    }
}
