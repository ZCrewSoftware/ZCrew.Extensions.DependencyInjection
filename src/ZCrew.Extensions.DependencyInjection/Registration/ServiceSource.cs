using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public abstract class ServiceSource : IServiceSource
{
    private readonly LazyServiceCollection lazyServiceCollection;

    protected ServiceSource()
    {
        this.lazyServiceCollection = new LazyServiceCollection(SelectServices);
    }

    protected abstract IEnumerable<ServiceDescriptor> SelectServices();

    public ServiceDescriptor this[int index]
    {
        get => this.lazyServiceCollection[index];
        set => this.lazyServiceCollection[index] = value;
    }

    public int Count => this.lazyServiceCollection.Count;

    public bool IsReadOnly => this.lazyServiceCollection.IsReadOnly;

    public void Add(ServiceDescriptor item)
    {
        this.lazyServiceCollection.Add(item);
    }

    public void Clear()
    {
        this.lazyServiceCollection.Clear();
    }

    public bool Contains(ServiceDescriptor item)
    {
        return this.lazyServiceCollection.Contains(item);
    }

    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        this.lazyServiceCollection.CopyTo(array, arrayIndex);
    }

    public bool Remove(ServiceDescriptor item)
    {
        return this.lazyServiceCollection.Remove(item);
    }

    public int IndexOf(ServiceDescriptor item)
    {
        return this.lazyServiceCollection.IndexOf(item);
    }

    public void Insert(int index, ServiceDescriptor item)
    {
        this.lazyServiceCollection.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        this.lazyServiceCollection.RemoveAt(index);
    }

    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        return this.lazyServiceCollection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
