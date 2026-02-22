using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Services;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.ClassesTests;

public class ClassesAssemblyVisibilityTests
{
    private const string DomainServicesNamespace = "Fixtures.SmallProject.Domain.Services";

    [Fact]
    public void FromAssembly_WithDefaultVisibility_ShouldExcludeInternalTypes()
    {
        // Arrange
        var assembly = typeof(CustomerService).Assembly;

        // Act
        var result = Classes.FromAssembly(assembly).InNamespace(DomainServicesNamespace).AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.DoesNotContain(typeof(InternalOrderValidator), registeredTypes);
    }

    [Fact]
    public void IncludePublicTypes_WhenCalled_ShouldExcludeInternalTypes()
    {
        // Arrange
        var assembly = typeof(CustomerService).Assembly;

        // Act
        var result = Classes.FromAssembly(assembly).IncludePublicTypes().InNamespace(DomainServicesNamespace).AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.DoesNotContain(typeof(InternalOrderValidator), registeredTypes);
    }

    [Fact]
    public void IncludeInternalTypes_WhenCalled_ShouldIncludeInternalTypes()
    {
        // Arrange
        var assembly = typeof(CustomerService).Assembly;

        // Act
        var result = Classes
            .FromAssembly(assembly)
            .IncludeInternalTypes()
            .InNamespace(DomainServicesNamespace)
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(InternalOrderValidator), registeredTypes);
    }

    [Fact]
    public void IncludeInternalTypes_WhenCalled_ShouldStillIncludePublicTypes()
    {
        // Arrange
        var assembly = typeof(CustomerService).Assembly;

        // Act
        var result = Classes
            .FromAssembly(assembly)
            .IncludeInternalTypes()
            .InNamespace(DomainServicesNamespace)
            .AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(OrderValidator), registeredTypes);
        Assert.Contains(typeof(CustomerValidator), registeredTypes);
    }

    [Fact]
    public void IncludeAllTypes_WhenCalled_ShouldIncludeAllTypes()
    {
        // Arrange
        var assembly = typeof(CustomerService).Assembly;

        // Act
        var result = Classes.FromAssembly(assembly).IncludeAllTypes().InNamespace(DomainServicesNamespace).AsSelf();

        // Assert
        var registeredTypes = result.Select(d => d.ImplementationType).ToArray();
        Assert.Contains(typeof(OrderValidator), registeredTypes);
        Assert.Contains(typeof(CustomerValidator), registeredTypes);
        Assert.Contains(typeof(InternalOrderValidator), registeredTypes);
    }
}
