using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Services;
using Fixtures.SmallProject.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.TypesTests;

public class TypesEntryPointTests
{
    [Fact]
    public void From_WithEnumerable_ShouldIncludeAllTypes()
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
        var result = Types.From(types).AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(ICustomerService), registeredTypes);
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(RepositoryBase<Customer>), registeredTypes);
        Assert.Contains(typeof(PricingDefaults), registeredTypes);
        Assert.Contains(typeof(OrderValidator), registeredTypes);
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void From_WithParams_ShouldIncludeAllTypes()
    {
        // Act
        var result = Types
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
        Assert.Contains(typeof(ICustomerService), registeredTypes);
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(RepositoryBase<Customer>), registeredTypes);
        Assert.Contains(typeof(PricingDefaults), registeredTypes);
        Assert.Contains(typeof(OrderValidator), registeredTypes);
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void From_WithInterfaces_ShouldIncludeInterfaces()
    {
        // Act
        var result = Types.From(typeof(ICustomerService), typeof(CustomerService)).AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(ICustomerService), registeredTypes);
        Assert.Contains(typeof(CustomerService), registeredTypes);
    }

    [Fact]
    public void From_WithAbstractClasses_ShouldIncludeAbstractClasses()
    {
        // Act
        var result = Types.From(typeof(RepositoryBase<Customer>), typeof(SqlCustomerRepository)).AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(RepositoryBase<Customer>), registeredTypes);
        Assert.Contains(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void From_WithStaticClasses_ShouldIncludeStaticClasses()
    {
        // Act
        var result = Types.From(typeof(PricingDefaults), typeof(CustomerService)).AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(PricingDefaults), registeredTypes);
        Assert.Contains(typeof(CustomerService), registeredTypes);
    }

    [Fact]
    public void From_WithStructs_ShouldIncludeStructs()
    {
        // Act
        var result = Types.From(typeof(Currency), typeof(CustomerService)).AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(Currency), registeredTypes);
        Assert.Contains(typeof(CustomerService), registeredTypes);
    }

    [Fact]
    public void From_WithEnums_ShouldIncludeEnums()
    {
        // Act
        var result = Types.From(typeof(OrderStatus), typeof(CustomerService)).AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(OrderStatus), registeredTypes);
        Assert.Contains(typeof(CustomerService), registeredTypes);
    }

    [Fact]
    public void FromAssembly_WhenCalled_ShouldIncludeAllTypes()
    {
        // Arrange
        var assembly = typeof(CustomerService).Assembly;

        // Act
        var result = Types.FromAssembly(assembly).AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(ICustomerService), registeredTypes);
        Assert.Contains(typeof(RepositoryBase<>), registeredTypes);
        Assert.Contains(typeof(PricingDefaults), registeredTypes);
    }

    [Fact]
    public void FromAssemblyContaining_WithType_ShouldScanCorrectAssembly()
    {
        // Act
        var result = Types.FromAssemblyContaining(typeof(CustomerService)).AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(ICustomerService), registeredTypes);
        Assert.Contains(typeof(OrderValidator), registeredTypes);
        Assert.Contains(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void FromAssemblyContaining_WithGeneric_ShouldScanCorrectAssembly()
    {
        // Act
        var result = Types.FromAssemblyContaining<CustomerService>().AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(CustomerService), registeredTypes);
        Assert.Contains(typeof(ICustomerService), registeredTypes);
        Assert.Contains(typeof(OrderValidator), registeredTypes);
        Assert.Contains(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void From_WithNullEnumerable_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => Types.From((IEnumerable<Type>)null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void From_WithNullParams_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => Types.From(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void FromAssembly_WithNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => Types.FromAssembly(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void FromAssemblyContaining_WithNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => Types.FromAssemblyContaining(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void From_WhenCalled_ShouldDefaultToSingletonLifetime()
    {
        // Act
        var result = Types.From(typeof(CustomerService)).AsSelf();

        // Assert
        Assert.All(result, descriptor => Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime));
    }

    [Fact]
    public void From_WhenEnumeratedWithoutTerminalMethod_ShouldDefaultToSelfRegistration()
    {
        // Arrange
        var selector = Types.From(typeof(CustomerService));

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
        var result = Types.FromThisAssembly().Where(t => t == typeof(TypesEntryPointTests)).AsSelf();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(TypesEntryPointTests), descriptor.ImplementationType);
    }
}
