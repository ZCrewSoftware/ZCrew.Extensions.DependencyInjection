using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public class LazyServiceCollection : IServiceCollection
{
    private readonly Lazy<List<ServiceDescriptor>> descriptors;

    public LazyServiceCollection(Func<IEnumerable<ServiceDescriptor>> serviceDescriptorsProvider)
    {
        this.descriptors = new Lazy<List<ServiceDescriptor>>(() =>
        {
            var services = serviceDescriptorsProvider();
            return services.ToList();
        });
    }

    public ServiceDescriptor this[int index]
    {
        get => this.descriptors.Value[index];
        set => throw ReadonlyException();
    }

    public int Count => this.descriptors.Value.Count;

    public bool IsReadOnly => true;

    public int IndexOf(ServiceDescriptor item)
    {
        return this.descriptors.Value.IndexOf(item);
    }

    public bool Contains(ServiceDescriptor item)
    {
        return this.descriptors.Value.Contains(item);
    }

    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        this.descriptors.Value.CopyTo(array, arrayIndex);
    }

    public void Add(ServiceDescriptor item)
    {
        throw ReadonlyException();
    }

    public void Clear()
    {
        throw ReadonlyException();
    }

    public bool Remove(ServiceDescriptor item)
    {
        throw ReadonlyException();
    }

    public void Insert(int index, ServiceDescriptor item)
    {
        throw ReadonlyException();
    }

    public void RemoveAt(int index)
    {
        throw ReadonlyException();
    }

    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        return this.descriptors.Value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static InvalidOperationException ReadonlyException()
    {
        return new InvalidOperationException("The service collection cannot be modified because it is read-only.");
    }
}
