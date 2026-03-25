using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration.UnitTests;

[SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
public class ServiceSourceTests
{
    [Fact]
    public void Count_WhenSelectServicesReturnsDescriptors_ShouldReturnCorrectCount()
    {
        // Arrange
        var descriptor1 = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var descriptor2 = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var source = new TestServiceSource([descriptor1, descriptor2]);

        // Act
        var count = source.Count;

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public void Count_WhenSelectServicesReturnsEmpty_ShouldReturnZero()
    {
        // Arrange
        var source = new TestServiceSource([]);

        // Act
        var count = source.Count;

        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public void IsReadOnly_WhenAccessed_ShouldReturnTrue()
    {
        // Arrange
        var source = new TestServiceSource([]);

        // Act
        var isReadOnly = source.IsReadOnly;

        // Assert
        Assert.True(isReadOnly);
    }

    [Fact]
    public void Indexer_WhenIndexIsValid_ShouldReturnDescriptor()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var source = new TestServiceSource([descriptor]);

        // Act
        var result = source[0];

        // Assert
        Assert.Same(descriptor, result);
    }

    [Fact]
    public void IndexerSet_WhenCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var source = new TestServiceSource([descriptor]);

        // Act
        var act = () => source[0] = descriptor;

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void IndexOf_WhenDescriptorExists_ShouldReturnIndex()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var source = new TestServiceSource([descriptor]);

        // Act
        var index = source.IndexOf(descriptor);

        // Assert
        Assert.Equal(0, index);
    }

    [Fact]
    public void IndexOf_WhenDescriptorDoesNotExist_ShouldReturnNegativeOne()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var other = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var source = new TestServiceSource([descriptor]);

        // Act
        var index = source.IndexOf(other);

        // Assert
        Assert.Equal(-1, index);
    }

    [Fact]
    public void Contains_WhenDescriptorExists_ShouldReturnTrue()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var source = new TestServiceSource([descriptor]);

        // Act
        var contains = source.Contains(descriptor);

        // Assert
        Assert.True(contains);
    }

    [Fact]
    public void Contains_WhenDescriptorDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var other = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var source = new TestServiceSource([descriptor]);

        // Act
        var contains = source.Contains(other);

        // Assert
        Assert.False(contains);
    }

    [Fact]
    public void CopyTo_WhenCalled_ShouldCopyDescriptorsToArray()
    {
        // Arrange
        var descriptor1 = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var descriptor2 = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var source = new TestServiceSource([descriptor1, descriptor2]);
        var array = new ServiceDescriptor[2];

        // Act
        source.CopyTo(array, 0);

        // Assert
        Assert.Same(descriptor1, array[0]);
        Assert.Same(descriptor2, array[1]);
    }

    [Fact]
    public void Add_WhenCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var source = new TestServiceSource([]);
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();

        // Act
        var act = () => source.Add(descriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Clear_WhenCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var source = new TestServiceSource([]);

        // Act
        var act = () => source.Clear();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Remove_WhenCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var source = new TestServiceSource([descriptor]);

        // Act
        var act = () =>
        {
            source.Remove(descriptor);
        };

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Insert_WhenCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var source = new TestServiceSource([]);

        // Act
        var act = () => source.Insert(0, descriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void RemoveAt_WhenCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var source = new TestServiceSource([descriptor]);

        // Act
        var act = () => source.RemoveAt(0);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetEnumerator_WhenCalled_ShouldEnumerateDescriptors()
    {
        // Arrange
        var descriptor1 = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var descriptor2 = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var source = new TestServiceSource([descriptor1, descriptor2]);

        // Act
        var result = source.ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Same(descriptor1, result[0]);
        Assert.Same(descriptor2, result[1]);
    }

    [Fact]
    public void SelectServices_WhenNotAccessed_ShouldNotBeEvaluated()
    {
        // Arrange & Act
        var source = new TestServiceSource([]);

        // Assert
        Assert.Equal(0, source.SelectServicesCallCount);
    }

    [Fact]
    public void SelectServices_WhenAccessedMultipleTimes_ShouldBeEvaluatedOnlyOnce()
    {
        // Arrange
        var source = new TestServiceSource([ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>()]);

        // Act
        _ = source.Count;
        _ = source.Count;

        // Assert
        Assert.Equal(1, source.SelectServicesCallCount);
    }

    private sealed class TestServiceSource : ServiceSource
    {
        private readonly IEnumerable<ServiceDescriptor> serviceDescriptors;

        public int SelectServicesCallCount { get; private set; }

        public TestServiceSource(IEnumerable<ServiceDescriptor> serviceDescriptors)
        {
            this.serviceDescriptors = serviceDescriptors;
        }

        protected override IEnumerable<ServiceDescriptor> SelectServices()
        {
            SelectServicesCallCount++;
            return this.serviceDescriptors;
        }
    }
}
