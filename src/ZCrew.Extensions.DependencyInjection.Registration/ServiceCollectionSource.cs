using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     A mutable <see cref="IServiceSource"/> backed by a concrete <see cref="ServiceCollection"/>. Used as the
///     terminal node produced by <see cref="ServiceSelector"/> methods.
/// </summary>
public class ServiceCollectionSource : ServiceCollection, IServiceSource
{
    internal ServiceCollectionSource(IEnumerable<ServiceDescriptor> descriptors)
    {
        ICollection<ServiceDescriptor> collection = this;
        foreach (var descriptor in descriptors)
        {
            collection.Add(descriptor);
        }
        MakeReadOnly();
    }
}
