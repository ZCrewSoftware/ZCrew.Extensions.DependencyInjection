using Fixtures.SmallProject.Application.Pipelines;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Infrastructure.Caching;
using Fixtures.SmallProject.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.ClassesTests;

public class ClassesGenericFilterTests
{
    [Fact]
    public void GenericTypes_WithMixedTypes_ShouldFilterToGenericTypes()
    {
        // Act
        var result = Classes
            .From(
                typeof(InMemoryRepository<>),
                typeof(InMemoryRepository<Customer>),
                typeof(CustomerService),
                typeof(SqlCustomerRepository)
            )
            .GenericTypes()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(InMemoryRepository<>), registeredTypes);
        Assert.Contains(typeof(InMemoryRepository<Customer>), registeredTypes);
        Assert.DoesNotContain(typeof(CustomerService), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void GenericTypes_WithNoGenericTypes_ShouldReturnEmpty()
    {
        // Act
        var result = Classes
            .From(typeof(CustomerService), typeof(OrderService))
            .GenericTypes()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GenericTypes_FromAssembly_ShouldExcludeNonGenericClasses()
    {
        // Act
        var result = Classes.FromAssemblyContaining<CustomerService>().GenericTypes().AsSelf().ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(InMemoryRepository<>), registeredTypes);
        Assert.Contains(typeof(SqlRepository<>), registeredTypes);
        Assert.Contains(typeof(InMemoryCacheProvider<>), registeredTypes);
        Assert.DoesNotContain(typeof(CustomerService), registeredTypes);
        Assert.DoesNotContain(typeof(OrderValidationStep), registeredTypes);
    }

    [Fact]
    public void GenericTypeDefinitions_WithMixedGenerics_ShouldFilterToOpenGenerics()
    {
        // Act
        var result = Classes
            .From(typeof(InMemoryRepository<>), typeof(InMemoryRepository<Customer>), typeof(CustomerService))
            .GenericTypeDefinitions()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(InMemoryRepository<>), descriptor.ImplementationType);
    }

    [Fact]
    public void GenericTypeDefinitions_FromAssembly_ShouldFilterToOpenGenerics()
    {
        // Act
        var result = Classes
            .FromAssemblyContaining<CustomerService>()
            .GenericTypeDefinitions()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(InMemoryRepository<>), registeredTypes);
        Assert.Contains(typeof(LoggingStep<>), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
    }

    [Fact]
    public void ConstructedGenericTypes_WithMixedGenerics_ShouldFilterToClosedGenerics()
    {
        // Act
        var result = Classes
            .From(typeof(InMemoryRepository<>), typeof(InMemoryRepository<Customer>), typeof(CustomerService))
            .ConstructedGenericTypes()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(InMemoryRepository<Customer>), descriptor.ImplementationType);
    }

    [Fact]
    public void ConstructedGenericTypes_WithClosedGeneric_ShouldRegisterClosedType()
    {
        // Act
        var result = Classes
            .From(typeof(InMemoryRepository<Customer>))
            .ConstructedGenericTypes()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(InMemoryRepository<Customer>), descriptor.ServiceType);
        Assert.Equal(typeof(InMemoryRepository<Customer>), descriptor.ImplementationType);
    }

    [Fact]
    public void ConstructedGenericTypes_FromAssembly_ShouldReturnEmpty()
    {
        // Act
        var result = Classes
            .FromAssemblyContaining<CustomerService>()
            .ConstructedGenericTypes()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GenericTypeDefinitions_WhenChainedWithConstructedGenericTypes_ShouldReturnEmpty()
    {
        // Act
        var result = Classes
            .From(typeof(InMemoryRepository<>), typeof(InMemoryRepository<Customer>))
            .GenericTypeDefinitions()
            .ConstructedGenericTypes()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GenericTypes_WhenChainedWithGenericTypeDefinitions_ShouldFilterToOpenGenerics()
    {
        // Act
        var result = Classes
            .From(typeof(InMemoryRepository<>), typeof(InMemoryRepository<Customer>), typeof(CustomerService))
            .GenericTypes()
            .GenericTypeDefinitions()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(InMemoryRepository<>), descriptor.ImplementationType);
    }

    [Fact]
    public void GenericTypeDefinitions_WhenCombinedWithBasedOn_ShouldApplyBoth()
    {
        // Act
        var result = Classes
            .FromAssemblyContaining<CustomerService>()
            .BasedOn(typeof(IRepository<>))
            .GenericTypeDefinitions()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(InMemoryRepository<>), registeredTypes);
        Assert.Contains(typeof(SqlRepository<>), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
        Assert.DoesNotContain(typeof(LoggingStep<>), registeredTypes);
    }

    [Fact]
    public void GenericTypeDefinitions_WhenCombinedWithNameEndsWith_ShouldApplyBoth()
    {
        // Act
        var result = Classes
            .FromAssemblyContaining<CustomerService>()
            .NameEndsWith("Repository")
            .GenericTypeDefinitions()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(InMemoryRepository<>), registeredTypes);
        Assert.Contains(typeof(SqlRepository<>), registeredTypes);
        Assert.DoesNotContain(typeof(SqlCustomerRepository), registeredTypes);
        Assert.DoesNotContain(typeof(InMemoryCacheProvider<>), registeredTypes);
    }

    [Fact]
    public void GenericTypes_WhenEnumeratedWithoutTerminalMethod_ShouldDefaultToSelfRegistration()
    {
        // Arrange
        var filter = Classes.From(typeof(InMemoryRepository<>), typeof(CustomerService)).GenericTypes();

        // Act
        var result = filter.ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(InMemoryRepository<>), descriptor.ServiceType);
        Assert.Equal(typeof(InMemoryRepository<>), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }
}
