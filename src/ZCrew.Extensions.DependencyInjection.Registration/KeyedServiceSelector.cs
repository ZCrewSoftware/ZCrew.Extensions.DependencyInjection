using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Applies service keys to registrations produced by the service selection stage. Extends
///     <see cref="ServiceCollectionSource"/> to provide <see cref="IKeyedServiceSelector"/> methods that create new
///     <see cref="ServiceCollectionSource"/> instances with keyed descriptors.
/// </summary>
public sealed class KeyedServiceSelector : ServiceCollectionSource, IKeyedServiceSelector
{
    internal KeyedServiceSelector(IEnumerable<ServiceDescriptor> descriptors)
        : base(descriptors) { }

    /// <inheritdoc />
    public IServiceSource Keyed()
    {
        return Keyed((implementationType, serviceType) =>
        {
            var implementationName = StripGenericArity(implementationType.Name);
            var serviceName = StripGenericArity(serviceType.GetInterfaceName());

            // The implementation and service may be the same type, so ensure there is a prefix differentiating them
            if (implementationName.EndsWith(serviceName) && implementationName.Length > serviceName.Length)
            {
                var serviceKeyString = new string(implementationName[..^serviceName.Length]);
                return serviceKeyString;
            }

            // Implementation name did not end with service name, no service key can be extracted automatically
            return null;
        });
    }

    /// <inheritdoc />
    public IServiceSource Keyed(object? serviceKey)
    {
        return Keyed(_ => serviceKey);
    }

    /// <inheritdoc />
    public IServiceSource Keyed(Func<Type, object?> serviceKeySelector)
    {
        return Keyed((implementationType, _) => serviceKeySelector(implementationType));
    }

    /// <inheritdoc />
    public IServiceSource Keyed(Func<Type, Type, object?> serviceKeySelector)
    {
        ArgumentNullException.ThrowIfNull(serviceKeySelector);

        var descriptors = new List<ServiceDescriptor>();
        foreach (var descriptor in this)
        {
            Debug.Assert(descriptor.ImplementationType != null, "Expected implementation type to always be set for this internal flow");
            var serviceKey = serviceKeySelector(descriptor.ImplementationType, descriptor.ServiceType);
            if (serviceKey != null)
            {
                descriptors.Add(descriptor.WithServiceKey(serviceKey));
                continue;
            }

            // Either there was no implementation type or the service key specified was null
            descriptors.Add(descriptor);
        }
        return new ServiceCollectionSource(descriptors);
    }

    private static ReadOnlySpan<char> StripGenericArity(ReadOnlySpan<char> name)
    {
        var backtick = name.IndexOf('`');
        return backtick >= 0 ? name[..backtick] : name;
    }
}
