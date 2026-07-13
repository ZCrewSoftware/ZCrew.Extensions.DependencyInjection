using System.Collections;
using Fixtures.SmallProject.Application.Services;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

/// <summary>
///     Regression tests protecting the immutability, branching, and laziness guarantees of the class-based chain: a
///     held mid-chain instance can be terminated multiple ways without state bleeding between branches, and no type
///     source is enumerated (nor any assembly scanned) until the chain is terminated.
/// </summary>
public class ChainLazinessTests
{
    [Fact]
    public void MidChainInstance_WhenTerminatedTwoWays_ShouldProduceIndependentResults()
    {
        // Arrange
        var midChain = Classes.From(typeof(CustomerService), typeof(OrderService));

        // Act
        var asSelf = midChain.AsSelf().ToServiceCollection();
        var asAllInterfaces = midChain.AsAllInterfaces().ToServiceCollection();

        // Assert
        Assert.Contains(asSelf, d => d.ServiceType == typeof(CustomerService));
        Assert.DoesNotContain(asSelf, d => d.ServiceType == typeof(ICustomerService));
        Assert.Contains(asAllInterfaces, d => d.ServiceType == typeof(ICustomerService));
        Assert.DoesNotContain(asAllInterfaces, d => d.ServiceType == typeof(CustomerService));
    }

    [Fact]
    public void MidChainKeySelector_WhenBranchedKeyedAndUnkeyed_ShouldNotShareState()
    {
        // Arrange
        var keySelector = Classes.From(typeof(CustomerService)).AsInterface<ICustomerService>();

        // Act
        var keyed = keySelector.Keyed("customer").ToServiceCollection();
        var unkeyed = keySelector.Unkeyed().ToServiceCollection();
        var skippedDefault = keySelector.ToServiceCollection();

        // Assert
        Assert.All(keyed, d => Assert.Equal("customer", d.ServiceKey));
        Assert.All(unkeyed, d => Assert.False(d.IsKeyedService));
        Assert.All(skippedDefault, d => Assert.False(d.IsKeyedService));
    }

    [Fact]
    public void EnumerableSource_WhenChainBuilt_ShouldNotEnumerateUntilTerminated()
    {
        // Arrange
        var chain = Types.From(new ThrowingTypeSequence()).Where(_ => true).AsSelf();

        // Act
        var terminate = () => chain.ToServiceCollection();

        // Assert
        Assert.Throws<InvalidOperationException>(terminate);
    }

    [Fact]
    public void HasAttribute_WhenChainBuilt_ShouldNotEnumerateUntilTerminated()
    {
        // Arrange
        var chain = Types.From(new ThrowingTypeSequence()).HasAttribute<ObsoleteAttribute>().AsSelf();

        // Act
        var terminate = () => chain.ToServiceCollection();

        // Assert
        Assert.Throws<InvalidOperationException>(terminate);
    }

    [Fact]
    public void AssemblyScan_WhenChainBuilt_ShouldNotInvokeFilterUntilTerminated()
    {
        // Arrange
        var filterInvocations = 0;
        var chain = Classes.FromAssemblyContaining<CustomerService>().Where(_ => Interlocked.Increment(ref filterInvocations) > 0);

        // Act
        var invocationsBeforeTerminal = filterInvocations;
        chain.ToServiceCollection();

        // Assert
        Assert.Equal(0, invocationsBeforeTerminal);
        Assert.True(filterInvocations > 0);
    }

    private sealed class ThrowingTypeSequence : IEnumerable<Type>
    {
        public IEnumerator<Type> GetEnumerator()
        {
            throw new InvalidOperationException("Enumeration should be deferred until the chain is terminated.");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
