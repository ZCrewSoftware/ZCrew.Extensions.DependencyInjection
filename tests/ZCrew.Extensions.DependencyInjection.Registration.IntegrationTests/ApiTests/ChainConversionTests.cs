using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.ApiTests;

public class ChainConversionTests
{
    private static readonly Type[] ServiceTypes =
    [
        typeof(CustomerService),
        typeof(OrderService),
        typeof(ProductService),
    ];

    private static readonly Type[] RepositoryTypes = [typeof(SqlCustomerRepository), typeof(SqlOrderRepository)];

    [Fact]
    public void FullSkip_WhenTerminatedDirectly_ShouldMatchExplicitDefaultTail()
    {
        // Arrange
        var skipped = Classes.From(ServiceTypes).ToServiceCollection();
        var explicitTail = Classes.From(ServiceTypes).AllTypes().AsSelf().Unkeyed().ToServiceCollection();

        // Act
        var skippedSignature = Describe(skipped);
        var explicitSignature = Describe(explicitTail);

        // Assert
        Assert.Equal(explicitSignature, skippedSignature);
    }

    [Fact]
    public void SkipAfterBasedOn_WhenTerminatedDirectly_ShouldMatchExplicitDefaultTail()
    {
        // Arrange
        var skipped = Classes.From(RepositoryTypes).BasedOn(typeof(IRepository<>)).ToServiceCollection();
        var explicitTail = Classes
            .From(RepositoryTypes)
            .BasedOn(typeof(IRepository<>))
            .AllTypes()
            .AsSelf()
            .Unkeyed()
            .ToServiceCollection();

        // Act
        var skippedSignature = Describe(skipped);
        var explicitSignature = Describe(explicitTail);

        // Assert
        Assert.Equal(explicitSignature, skippedSignature);
    }

    [Fact]
    public void SkipKeyedStage_WhenSelectingAllInterfaces_ShouldMatchExplicitUnkeyed()
    {
        // Arrange
        var skipped = Classes.From(RepositoryTypes).AsAllInterfaces().ToServiceCollection();
        var explicitUnkeyed = Classes.From(RepositoryTypes).AsAllInterfaces().Unkeyed().ToServiceCollection();

        // Act
        var skippedSignature = Describe(skipped);
        var explicitSignature = Describe(explicitUnkeyed);

        // Assert
        Assert.Equal(explicitSignature, skippedSignature);
    }

    [Fact]
    public void SkipKeyedStage_WhenSelectingInterface_ShouldMatchExplicitUnkeyed()
    {
        // Arrange
        var skipped = Classes.From(RepositoryTypes).BasedOn(typeof(IRepository<>)).AsInterface().ToServiceCollection();
        var explicitUnkeyed = Classes
            .From(RepositoryTypes)
            .BasedOn(typeof(IRepository<>))
            .AsInterface()
            .Unkeyed()
            .ToServiceCollection();

        // Act
        var skippedSignature = Describe(skipped);
        var explicitSignature = Describe(explicitUnkeyed);

        // Assert
        Assert.Equal(explicitSignature, skippedSignature);
    }

    [Fact]
    public void SkippedLifetime_WhenTerminatedDirectly_ShouldDefaultToSingleton()
    {
        // Act
        var result = Classes.From(ServiceTypes).ToServiceCollection();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void SkippedKey_WhenTerminatedDirectly_ShouldDefaultToUnkeyed()
    {
        // Act
        var result = Classes.From(ServiceTypes).AsInterface().ToServiceCollection();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, d => Assert.False(d.IsKeyedService));
    }

    [Fact]
    public void SkippedSelection_WhenTerminatedDirectly_ShouldDefaultToSelf()
    {
        // Act
        var result = Classes.From(ServiceTypes).ToServiceCollection();

        // Assert
        Assert.All(
            result,
            d => Assert.Equal(d.ServiceType, d.ImplementationType)
        );
        Assert.Contains(result, d => d.ServiceType == typeof(CustomerService));
    }

    [Fact]
    public void BasedOn_WhenSelectingInterface_ShouldRegisterTopLevelInterface()
    {
        // Act
        var result = Classes.From(RepositoryTypes).BasedOn(typeof(IRepository<>)).AsInterface().ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(SqlCustomerRepository) && d.ServiceType == typeof(ICustomerRepository)
        );
        Assert.Contains(
            result,
            d => d.ImplementationType == typeof(SqlOrderRepository) && d.ServiceType == typeof(IOrderRepository)
        );
    }

    [Fact]
    public void BasedOn_WhenSelectingBase_ShouldRegisterMatchingBaseType()
    {
        // Act
        var result = Classes.From(RepositoryTypes).BasedOn(typeof(IRepository<>)).AsBase().ToServiceCollection();

        // Assert
        Assert.Contains(
            result,
            d => d.ServiceType == typeof(IRepository<Customer>) && d.ImplementationType == typeof(SqlCustomerRepository)
        );
    }

    [Fact]
    public void BasedOn_WhenSelectionStageSkipped_ShouldStillFilterByBaseType()
    {
        // Act
        var result = Classes
            .From(typeof(SqlCustomerRepository), typeof(CustomerService))
            .BasedOn(typeof(IRepository<>))
            .ToServiceCollection();

        // Assert
        Assert.Contains(result, d => d.ImplementationType == typeof(SqlCustomerRepository));
        Assert.DoesNotContain(result, d => d.ImplementationType == typeof(CustomerService));
    }

    [Fact]
    public void AddSingleton_WhenGivenAssemblyScan_ShouldBindToChainOverloadNotInstanceRegistration()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(
            Classes
                .FromAssemblyContaining<SqlCustomerRepository>()
                .BasedOn(typeof(IRepository<>))
                .InNamespace("Fixtures.SmallProject.Infrastructure.Persistence", includeSubnamespaces: true)
                .AsInterface()
        );

        // Assert
        Assert.Contains(
            services,
            d => d.ImplementationType == typeof(SqlCustomerRepository) && d.ServiceType == typeof(ICustomerRepository)
        );
        Assert.Contains(
            services,
            d => d.ImplementationType == typeof(SqlOrderRepository) && d.ServiceType == typeof(IOrderRepository)
        );
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(ServiceKeySelector));
        Assert.All(services, d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void SelfThenAllInterfaces_WhenChained_ShouldEqualPrependedSelector()
    {
        // Arrange — chaining AsSelf().AsAllInterfaces() should produce the same registrations as the established
        // single-selector idiom that prepends the implementation to its own interfaces.
        var chained = Classes.From(typeof(SqlCustomerRepository)).AsSelf().AsAllInterfaces().ToServiceCollection();
        var prepended = Classes
            .From(typeof(SqlCustomerRepository))
            .As(type => type.GetInterfaces().Prepend(type))
            .ToServiceCollection();

        // Act
        var chainedSignature = Describe(chained);
        var prependedSignature = Describe(prepended);

        // Assert
        Assert.Equal(prependedSignature, chainedSignature);
    }

    private static List<string> Describe(IServiceCollection services)
    {
        return services
            .Select(descriptor =>
                string.Join(
                    '|',
                    descriptor.ServiceType.FullName,
                    ImplementationOf(descriptor),
                    descriptor.Lifetime,
                    descriptor.IsKeyedService ? "keyed" : "unkeyed"
                )
            )
            .OrderBy(signature => signature, StringComparer.Ordinal)
            .ToList();
    }

    // Normalizes a descriptor to its implementation identity, ignoring the specific (randomly generated) shared
    // component key value so that two structurally identical batches compare equal.
    private static string ImplementationOf(ServiceDescriptor descriptor)
    {
        if (descriptor.IsKeyedService)
        {
            return descriptor.KeyedImplementationType?.FullName ?? "factory";
        }

        return descriptor.ImplementationType?.FullName ?? "factory";
    }
}
