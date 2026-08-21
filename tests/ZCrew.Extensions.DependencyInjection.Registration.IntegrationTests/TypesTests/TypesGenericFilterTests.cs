using Fixtures.SmallProject.Application.Pipelines;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Domain.Services;
using Fixtures.SmallProject.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.TypesTests;

public class TypesGenericFilterTests
{
    [Fact]
    public void GenericTypes_WithInterfaces_ShouldIncludeGenericInterfaces()
    {
        // Act
        var result = Types.FromAssemblyContaining<CustomerService>().GenericTypes().AsSelf().ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(IRepository<>), registeredTypes);
        Assert.Contains(typeof(IValidator<>), registeredTypes);
        Assert.Contains(typeof(IPipelineBehavior<,>), registeredTypes);
        Assert.Contains(typeof(InMemoryRepository<>), registeredTypes);
        Assert.DoesNotContain(typeof(ICustomerService), registeredTypes);
        Assert.DoesNotContain(typeof(CustomerService), registeredTypes);
    }

    [Fact]
    public void GenericTypes_WithStructsAndEnums_ShouldExcludeNonGenerics()
    {
        // Act
        var result = Types
            .From(typeof(Currency), typeof(OrderStatus), typeof(IRepository<>))
            .GenericTypes()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(IRepository<>), descriptor.ImplementationType);
    }

    [Fact]
    public void GenericTypeDefinitions_WithTypeNestedInGenericType_ShouldMatch()
    {
        // Act
        var result = Types
            .FromAssemblyContaining<CustomerService>()
            .GenericTypeDefinitions()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(Pipeline<>.IStep), registeredTypes);
        Assert.DoesNotContain(typeof(OrderValidationStep), registeredTypes);
    }

    [Fact]
    public void ConstructedGenericTypes_WithClosedInterface_ShouldFilterToClosedGenerics()
    {
        // Act
        var result = Types
            .From(typeof(IRepository<>), typeof(IRepository<Customer>), typeof(ICustomerRepository))
            .ConstructedGenericTypes()
            .AsSelf()
            .ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(IRepository<Customer>), descriptor.ImplementationType);
    }

    [Fact]
    public void GenericTypes_WhenEnumeratedWithoutTerminalMethod_ShouldDefaultToSelfRegistration()
    {
        // Arrange
        var filter = Types.From(typeof(IRepository<>), typeof(ICustomerRepository)).GenericTypes();

        // Act
        var result = filter.ToServiceCollection();

        // Assert
        var descriptor = Assert.Single(result);
        Assert.Equal(typeof(IRepository<>), descriptor.ServiceType);
        Assert.Equal(typeof(IRepository<>), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }
}
