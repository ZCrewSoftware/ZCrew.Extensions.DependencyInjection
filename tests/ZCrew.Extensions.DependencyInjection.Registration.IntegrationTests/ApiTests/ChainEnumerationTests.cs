using System.Collections;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.ApiTests;

public class ChainEnumerationTests
{
    private static readonly Type[] SourceTypes =
    [
        typeof(CustomerService),
        typeof(OrderService),
        typeof(ProductService),
        typeof(SqlCustomerRepository),
        typeof(SqlOrderRepository),
    ];

    [Fact]
    public void BuildingDeepChain_ShouldNotEnumerateSource()
    {
        // Arrange
        var source = new CountingTypeSource(SourceTypes);

        // Act
        _ = Classes
            .From(source)
            .BasedOn(typeof(IRepository<>))
            .Where(type => !type.IsGenericTypeDefinition)
            .InNamespace("Fixtures.SmallProject.Infrastructure.Persistence", includeSubnamespaces: true)
            .AsInterface()
            .Keyed();

        // Assert
        Assert.Equal(0, source.EnumerationCount);
    }

    [Fact]
    public void FullSkipTerminal_ShouldEnumerateSourceExactlyOnce()
    {
        // Arrange
        var source = new CountingTypeSource(SourceTypes);

        // Act
        _ = Classes.From(source).ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void BasedOnInterfaceSelectionTerminal_ShouldEnumerateSourceExactlyOnce()
    {
        // Arrange
        var source = new CountingTypeSource(SourceTypes);

        // Act
        _ = Classes.From(source).BasedOn(typeof(IRepository<>)).AsInterface().ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void AllInterfacesTerminal_ShouldEnumerateSourceExactlyOnce()
    {
        // Arrange
        var source = new CountingTypeSource(SourceTypes);

        // Act
        _ = Classes.From(source).AsAllInterfaces().ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void AddSingletonTerminal_ShouldEnumerateSourceExactlyOnce()
    {
        // Arrange
        var source = new CountingTypeSource(SourceTypes);
        var services = new ServiceCollection();

        // Act
        services.AddSingleton(Classes.From(source).BasedOn(typeof(IRepository<>)).AsInterface());

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void BranchingMidChainIntoTwoTerminals_ShouldEnumerateSourceOncePerBranch()
    {
        // Arrange
        var source = new CountingTypeSource(SourceTypes);
        var midChain = Classes.From(source);

        // Act
        _ = midChain.AsSelf().ToServiceCollection();
        _ = midChain.AsAllInterfaces().ToServiceCollection();

        // Assert
        Assert.Equal(2, source.EnumerationCount);
    }

    /// <summary>
    ///     An <see cref="IEnumerable{T}"/> that records how many times enumeration is started, so tests can assert
    ///     how often the chain touches the underlying type sequence.
    /// </summary>
    private sealed class CountingTypeSource(IEnumerable<Type> types) : IEnumerable<Type>
    {
        public int EnumerationCount { get; private set; }

        public IEnumerator<Type> GetEnumerator()
        {
            EnumerationCount++;
            return types.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
