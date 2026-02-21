using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Services;
using Fixtures.SmallProject.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using ZCrew.Extensions.DependencyInjection.Registration;

namespace ZCrew.Extensions.DependencyInjection.IntegrationTests.Registration.ClassesTests;

public class ClassesEntryPointTests
{
    [Fact]
    public void From_WithEnumerable_ShouldRegisterOnlyConcreteClasses()
    {
        // Arrange
        IEnumerable<Type> types =
        [
            typeof(ICustomerService),
            typeof(CustomerService),
            typeof(RepositoryBase<Customer>),
            typeof(PricingDefaults),
            typeof(OrderValidator),
        ];

        // Act
        var result = Classes.From(types).AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderValidator), registeredTypes);
        Assert.DoesNotContain(typeof(ICustomerService), registeredTypes);
        Assert.DoesNotContain(typeof(RepositoryBase<Customer>), registeredTypes);
        Assert.DoesNotContain(typeof(PricingDefaults), registeredTypes);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void From_WithParams_ShouldRegisterOnlyConcreteClasses()
    {
        // Act
        var result = Classes
            .From(
                typeof(ICustomerService),
                typeof(CustomerService),
                typeof(RepositoryBase<Customer>),
                typeof(PricingDefaults),
                typeof(OrderValidator)
            )
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderValidator), registeredTypes);
        Assert.DoesNotContain(typeof(ICustomerService), registeredTypes);
        Assert.DoesNotContain(typeof(RepositoryBase<Customer>), registeredTypes);
        Assert.DoesNotContain(typeof(PricingDefaults), registeredTypes);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void FromAssembly_WhenCalled_ShouldRegisterOnlyConcreteClasses()
    {
        // Arrange
        var assembly = typeof(CustomerService).Assembly;

        // Act
        var result = Classes
            .FromAssembly(assembly)
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.DoesNotContain(typeof(ICustomerService), registeredTypes);
        Assert.DoesNotContain(typeof(RepositoryBase<>), registeredTypes);
        Assert.DoesNotContain(typeof(PricingDefaults), registeredTypes);
    }

    [Fact]
    public void FromAssemblyContaining_WithType_ShouldScanCorrectAssembly()
    {
        // Act
        var result = Classes
            .FromAssemblyContaining(typeof(CustomerService))
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderValidator), registeredTypes);
        Assert.Contains(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void FromAssemblyContaining_WithGeneric_ShouldScanCorrectAssembly()
    {
        // Act
        var result = Classes
            .FromAssemblyContaining<CustomerService>()
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(OrderValidator), registeredTypes);
        Assert.Contains(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void From_WithNullEnumerable_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () =>
            Classes.From((IEnumerable<Type>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void From_WithNullParams_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () =>
            Classes.From(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void FromAssembly_WithNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () =>
            Classes.FromAssembly(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void FromAssemblyContaining_WithNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () =>
            Classes.FromAssemblyContaining(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void From_WhenCalled_ShouldDefaultToSingletonLifetime()
    {
        // Act
        var result = Classes
            .From(typeof(CustomerService))
            .AsSelf();

        // Assert
        Assert.All(result, descriptor => Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime));
    }

    [Fact]
    public void From_WhenEnumeratedWithoutTerminalMethod_ShouldDefaultToSelfRegistration()
    {
        // Arrange
        var selector = Classes.From(typeof(CustomerService));

        // Act
        var result = selector.AsServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(CustomerService), descriptor.ServiceType);
        Assert.Equal(typeof(CustomerService), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void FromThisAssembly_WhenCalled_ShouldScanCallingAssembly()
    {
        // Act
        var result = Classes
            .FromThisAssembly()
            .Where(t => t == typeof(ClassesEntryPointTests))
            .AsSelf();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(ClassesEntryPointTests), descriptor.ImplementationType);
    }
}
