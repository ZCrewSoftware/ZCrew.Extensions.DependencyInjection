using System.Collections;
using Fixtures.SmallProject.Application.Services;
using Fixtures.SmallProject.Domain.Repositories;
using Fixtures.SmallProject.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.IntegrationTests.ApiTests;

public class MultiEnumerationTests
{
    private static readonly Type[] SourceTypes = [typeof(CustomerService), typeof(SqlCustomerRepository)];

    [Fact]
    public void ServiceSource_WhenConstructed_ShouldNotEnumerateServices()
    {
        // Arrange
        var source = new CountingSource<Service>(Services());

        // Act
        _ = new ServiceSource(source);

        // Assert
        Assert.Equal(0, source.EnumerationCount);
    }

    [Fact]
    public void ServiceSource_WhenTerminated_ShouldEnumerateServicesOnce()
    {
        // Arrange — a ServiceSource holds finalized services, so give them a lifetime (the lifetime stage would
        // normally do this before a ServiceSource is produced).
        var source = new CountingSource<Service>(
            Services().Select(service => service.AsLifetime(ServiceLifetime.Singleton))
        );
        var serviceSource = new ServiceSource(source);

        // Act
        _ = serviceSource.ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void ServiceLifetimeSelector_WhenConstructed_ShouldNotEnumerateServices()
    {
        // Arrange
        var source = new CountingSource<Service>(Services());

        // Act
        _ = new ServiceLifetimeSelector(source);

        // Assert
        Assert.Equal(0, source.EnumerationCount);
    }

    [Fact]
    public void ServiceLifetimeSelector_WhenTerminatedWithDefaultLifetime_ShouldEnumerateServicesOnce()
    {
        // Arrange
        var source = new CountingSource<Service>(Services());
        var selector = new ServiceLifetimeSelector(source);

        // Act
        _ = selector.ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void ServiceLifetimeSelector_WhenTerminatedAfterAsLifetime_ShouldEnumerateServicesOnce()
    {
        // Arrange
        var source = new CountingSource<Service>(Services());
        var selector = new ServiceLifetimeSelector(source);

        // Act
        _ = selector.AsSingleton().ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void ServiceKeySelector_WhenConstructed_ShouldNotEnumerateServices()
    {
        // Arrange
        var source = new CountingSource<Service>(Services());

        // Act
        _ = new ServiceKeySelector(source);

        // Assert
        Assert.Equal(0, source.EnumerationCount);
    }

    [Fact]
    public void ServiceKeySelector_WhenTerminatedUnkeyed_ShouldEnumerateServicesOnce()
    {
        // Arrange
        var source = new CountingSource<Service>(Services());
        var selector = new ServiceKeySelector(source);

        // Act
        _ = selector.ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void ServiceKeySelector_WhenTerminatedAfterKeyed_ShouldEnumerateServicesOnce()
    {
        // Arrange
        var source = new CountingSource<Service>(Services());
        var selector = new ServiceKeySelector(source);

        // Act
        _ = selector.Keyed("key").ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void ServiceSelector_WhenConstructed_ShouldNotEnumerateTypes()
    {
        // Arrange
        var source = new CountingSource<Type>(SourceTypes);

        // Act
        _ = new ServiceSelector(source, [typeof(object)]);

        // Assert
        Assert.Equal(0, source.EnumerationCount);
    }

    [Fact]
    public void ServiceSelector_WhenTerminatedWithSelfDefault_ShouldEnumerateTypesOnce()
    {
        // Arrange
        var source = new CountingSource<Type>(SourceTypes);
        var selector = new ServiceSelector(source, [typeof(object)]);

        // Act
        _ = selector.ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void ServiceSelector_WhenTerminatedAfterAs_ShouldEnumerateTypesOnce()
    {
        // Arrange
        var source = new CountingSource<Type>(SourceTypes);
        var selector = new ServiceSelector(source, [typeof(object)]);

        // Act
        _ = selector.As(type => [type]).ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void TypeFilter_WhenConstructedWithBasedOnFilter_ShouldNotEnumerateTypes()
    {
        // Arrange
        var source = new CountingSource<Type>(SourceTypes);

        // Act
        _ = new TypeFilter(source, [typeof(IRepository<>)]);

        // Assert
        Assert.Equal(0, source.EnumerationCount);
    }

    [Fact]
    public void TypeFilter_WhenTerminatedWithDefaultBases_ShouldEnumerateTypesOnce()
    {
        // Arrange
        var source = new CountingSource<Type>(SourceTypes);
        var filter = new TypeFilter(source);

        // Act
        _ = filter.ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void TypeFilter_WhenTerminatedWithBasedOnFilter_ShouldEnumerateTypesOnce()
    {
        // Arrange
        var source = new CountingSource<Type>(SourceTypes);
        var filter = new TypeFilter(source, [typeof(IRepository<>)]);

        // Act
        _ = filter.ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void TypeFilter_WhenTerminatedAfterWhere_ShouldEnumerateTypesOnce()
    {
        // Arrange
        var source = new CountingSource<Type>(SourceTypes);
        var filter = new TypeFilter(source);

        // Act
        _ = filter.Where(_ => true).ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void ServiceSelector_WhenConstructedFromServices_ShouldNotEnumerateServices()
    {
        // Arrange
        var source = new CountingSource<Service>(Services());

        // Act
        _ = new ServiceSelector(source, [typeof(object)]);

        // Assert
        Assert.Equal(0, source.EnumerationCount);
    }

    [Fact]
    public void ServiceSelector_WhenTerminatedFromServices_ShouldEnumerateServicesOnce()
    {
        // Arrange
        var source = new CountingSource<Service>(Services());
        var selector = new ServiceSelector(source, [typeof(object)]);

        // Act
        _ = selector.ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    [Fact]
    public void ServiceSelector_WhenTerminatedAfterChainedAs_ShouldEnumerateTypesOnce()
    {
        // Arrange
        var source = new CountingSource<Type>(SourceTypes);
        var selector = new ServiceSelector(source, [typeof(object)]);

        // Act
        _ = selector.As(type => [type]).As(type => type.GetInterfaces()).ToServiceCollection();

        // Assert
        Assert.Equal(1, source.EnumerationCount);
    }

    private static IEnumerable<Service> Services()
    {
        return
        [
            new Service(typeof(CustomerService), [typeof(ICustomerService)]),
            new Service(typeof(OrderService), [typeof(IOrderService)]),
        ];
    }

    /// <summary>
    ///     An <see cref="IEnumerable{T}"/> that records how many times enumeration is started, so a test can assert
    ///     how often a stage walks its underlying source.
    /// </summary>
    private sealed class CountingSource<T>(IEnumerable<T> items) : IEnumerable<T>
    {
        public int EnumerationCount { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            EnumerationCount++;
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
