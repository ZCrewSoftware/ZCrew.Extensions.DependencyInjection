using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Abstract base class for registration chain nodes that lazily produce service descriptors. Implements
///     <see cref="IServiceCollection"/> by deferring to a <see cref="LazyServiceCollection"/> that evaluates
///     <see cref="SelectServices"/> on first access. The resulting collection is read-only.
/// </summary>
public abstract class ServiceSource : IServiceSource
{
    private readonly LazyServiceCollection lazyServiceCollection;

    /// <summary>
    ///     Initialize a new service source with a <see cref="lazyServiceCollection"/> backing it.
    /// </summary>
    protected ServiceSource()
    {
        this.lazyServiceCollection = new LazyServiceCollection(SelectServices);
    }

    /// <summary>
    ///     When overridden, produces the service descriptors for this node in the registration chain.
    /// </summary>
    protected abstract IEnumerable<ServiceDescriptor> SelectServices();

    /// <inheritdoc />
    public ServiceDescriptor this[int index]
    {
        get => this.lazyServiceCollection[index];
        set => this.lazyServiceCollection[index] = value;
    }

    /// <inheritdoc />
    public int Count => this.lazyServiceCollection.Count;

    /// <inheritdoc />
    public bool IsReadOnly => this.lazyServiceCollection.IsReadOnly;

    /// <inheritdoc />
    public void Add(ServiceDescriptor item)
    {
        this.lazyServiceCollection.Add(item);
    }

    /// <inheritdoc />
    public void Clear()
    {
        this.lazyServiceCollection.Clear();
    }

    /// <inheritdoc />
    public bool Contains(ServiceDescriptor item)
    {
        return this.lazyServiceCollection.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        this.lazyServiceCollection.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public bool Remove(ServiceDescriptor item)
    {
        return this.lazyServiceCollection.Remove(item);
    }

    /// <inheritdoc />
    public int IndexOf(ServiceDescriptor item)
    {
        return this.lazyServiceCollection.IndexOf(item);
    }

    /// <inheritdoc />
    public void Insert(int index, ServiceDescriptor item)
    {
        this.lazyServiceCollection.Insert(index, item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        this.lazyServiceCollection.RemoveAt(index);
    }

    /// <inheritdoc />
    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        return this.lazyServiceCollection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
