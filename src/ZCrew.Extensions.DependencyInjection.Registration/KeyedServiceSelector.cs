namespace ZCrew.Extensions.DependencyInjection.Registration;

internal sealed class KeyedServiceSelector : IKeyedServiceSelector
{
    private readonly IEnumerable<ServiceComponent> components;

    internal KeyedServiceSelector(IEnumerable<ServiceComponent> components)
    {
        this.components = components;
    }

    /// <inheritdoc />
    public IServiceSource Unkeyed()
    {
        return new ServiceSource(this.components);
    }

    /// <inheritdoc />
    public IServiceSource Keyed()
    {
        return Keyed(
            (implementationType, serviceType) =>
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
            }
        );
    }

    /// <inheritdoc />
    public IServiceSource Keyed(object? serviceKey)
    {
        // Just skip the scan entirely
        if (serviceKey == null)
        {
            return Unkeyed();
        }

        return new ServiceSource(this.components.Select(component => component.WithServiceKey(serviceKey)));
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
        return new ServiceSource(this.components.Select(component => component.WithServiceKey(serviceKeySelector)));
    }

    private static ReadOnlySpan<char> StripGenericArity(ReadOnlySpan<char> name)
    {
        var backtick = name.IndexOf('`');
        return backtick >= 0 ? name[..backtick] : name;
    }
}
