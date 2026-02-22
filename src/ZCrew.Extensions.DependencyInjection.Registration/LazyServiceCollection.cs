using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     A read-only <see cref="IServiceCollection"/> that defers evaluation of its service descriptors until first
///     access. Mutating operations throw <see cref="InvalidOperationException"/>.
/// </summary>
internal sealed class LazyServiceCollection : IServiceCollection
{
    private readonly Lazy<List<ServiceDescriptor>> descriptors;

    internal LazyServiceCollection(Func<IEnumerable<ServiceDescriptor>> serviceDescriptorsProvider)
    {
        this.descriptors = new Lazy<List<ServiceDescriptor>>(() =>
        {
            var services = serviceDescriptorsProvider();
            return services.ToList();
        });
    }

    /// <inheritdoc />
    public ServiceDescriptor this[int index]
    {
        get => this.descriptors.Value[index];
        set => throw ReadonlyException();
    }

    /// <inheritdoc />
    public int Count => this.descriptors.Value.Count;

    /// <inheritdoc />
    public bool IsReadOnly => true;

    /// <inheritdoc />
    public int IndexOf(ServiceDescriptor item)
    {
        return this.descriptors.Value.IndexOf(item);
    }

    /// <inheritdoc />
    public bool Contains(ServiceDescriptor item)
    {
        return this.descriptors.Value.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        this.descriptors.Value.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public void Add(ServiceDescriptor item)
    {
        throw ReadonlyException();
    }

    /// <inheritdoc />
    public void Clear()
    {
        throw ReadonlyException();
    }

    /// <inheritdoc />
    public bool Remove(ServiceDescriptor item)
    {
        throw ReadonlyException();
    }

    /// <inheritdoc />
    public void Insert(int index, ServiceDescriptor item)
    {
        throw ReadonlyException();
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        throw ReadonlyException();
    }

    /// <inheritdoc />
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
