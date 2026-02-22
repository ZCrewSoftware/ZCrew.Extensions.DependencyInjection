using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using ZCrew.Extensions.DependencyInjection.Registration;

namespace ZCrew.Extensions.DependencyInjection.UnitTests.Registration;

[SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
public class LazyServiceCollectionTests
{
    [Fact]
    public void Count_WhenProviderReturnsDescriptors_ShouldReturnCorrectCount()
    {
        // Arrange
        var collection = new LazyServiceCollection(() =>
            [
                ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>(),
                ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>(),
            ]
        );

        // Act
        var count = collection.Count;

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public void Count_WhenProviderReturnsEmpty_ShouldReturnZero()
    {
        // Arrange
        var collection = new LazyServiceCollection(() => []);

        // Act
        var count = collection.Count;

        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public void IsReadOnly_WhenAccessed_ShouldReturnTrue()
    {
        // Arrange
        var collection = new LazyServiceCollection(() => []);

        // Act
        var isReadOnly = collection.IsReadOnly;

        // Assert
        Assert.True(isReadOnly);
    }

    [Fact]
    public void Indexer_WhenIndexIsValid_ShouldReturnDescriptor()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var collection = new LazyServiceCollection(() => [descriptor]);

        // Act
        var result = collection[0];

        // Assert
        Assert.Same(descriptor, result);
    }

    [Fact]
    public void IndexerSet_WhenCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var collection = new LazyServiceCollection(() => [descriptor]);

        // Act
        var act = () => collection[0] = descriptor;

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void IndexOf_WhenDescriptorExists_ShouldReturnIndex()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var collection = new LazyServiceCollection(() => [descriptor]);

        // Act
        var index = collection.IndexOf(descriptor);

        // Assert
        Assert.Equal(0, index);
    }

    [Fact]
    public void IndexOf_WhenDescriptorDoesNotExist_ShouldReturnNegativeOne()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var other = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var collection = new LazyServiceCollection(() => [descriptor]);

        // Act
        var index = collection.IndexOf(other);

        // Assert
        Assert.Equal(-1, index);
    }

    [Fact]
    public void Contains_WhenDescriptorExists_ShouldReturnTrue()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var collection = new LazyServiceCollection(() => [descriptor]);

        // Act
        var contains = collection.Contains(descriptor);

        // Assert
        Assert.True(contains);
    }

    [Fact]
    public void Contains_WhenDescriptorDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var other = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var collection = new LazyServiceCollection(() => [descriptor]);

        // Act
        var contains = collection.Contains(other);

        // Assert
        Assert.False(contains);
    }

    [Fact]
    public void CopyTo_WhenCalled_ShouldCopyDescriptorsToArray()
    {
        // Arrange
        var descriptor1 = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var descriptor2 = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var collection = new LazyServiceCollection(() => [descriptor1, descriptor2]);
        var array = new ServiceDescriptor[2];

        // Act
        collection.CopyTo(array, 0);

        // Assert
        Assert.Same(descriptor1, array[0]);
        Assert.Same(descriptor2, array[1]);
    }

    [Fact]
    public void Add_WhenCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var collection = new LazyServiceCollection(() => []);
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();

        // Act
        var act = () => collection.Add(descriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Clear_WhenCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var collection = new LazyServiceCollection(() => []);

        // Act
        var act = () => collection.Clear();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Remove_WhenCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var collection = new LazyServiceCollection(() => [descriptor]);

        // Act
        var act = () =>
        {
            collection.Remove(descriptor);
        };

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Insert_WhenCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var collection = new LazyServiceCollection(() => []);

        // Act
        var act = () => collection.Insert(0, descriptor);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void RemoveAt_WhenCalled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var descriptor = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var collection = new LazyServiceCollection(() => [descriptor]);

        // Act
        var act = () => collection.RemoveAt(0);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void GetEnumerator_WhenCalled_ShouldEnumerateDescriptors()
    {
        // Arrange
        var descriptor1 = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var descriptor2 = ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>();
        var collection = new LazyServiceCollection(() => [descriptor1, descriptor2]);

        // Act
        var result = collection.ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Same(descriptor1, result[0]);
        Assert.Same(descriptor2, result[1]);
    }

    [Fact]
    public void Constructor_WhenProviderNotAccessed_ShouldNotEvaluateProvider()
    {
        // Arrange
        var evaluated = false;

        // Act
        _ = new LazyServiceCollection(() =>
        {
            evaluated = true;
            return [];
        });

        // Assert
        Assert.False(evaluated);
    }

    [Fact]
    public void Count_WhenAccessedMultipleTimes_ShouldEvaluateProviderOnlyOnce()
    {
        // Arrange
        var evaluationCount = 0;
        var collection = new LazyServiceCollection(() =>
        {
            evaluationCount++;
            return [ServiceDescriptor.Transient<IServiceProvider, ServiceProvider>()];
        });

        // Act
        _ = collection.Count;
        _ = collection.Count;

        // Assert
        Assert.Equal(1, evaluationCount);
    }
}
