using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Intermediate between the <see cref="IServiceSelector"/> and the raw <see cref="ServiceDescriptor"/>.
/// </summary>
internal readonly record struct ServiceComponent
{
    private readonly Type implementation;
    private readonly IReadOnlyList<Type> services;
    private readonly ServiceLifetime lifetime = ServiceLifetime.Singleton;
    private readonly object? serviceKey;
    private readonly Func<Type, Type, object?>? serviceKeyProvider;

    /// <summary>
    ///     Create a new service component.
    /// </summary>
    /// <param name="implementation">The implementation type.</param>
    /// <param name="services">The services to register for the <paramref name="implementation"/>.</param>
    public ServiceComponent(Type implementation, IReadOnlyList<Type> services)
    {
        this.implementation = implementation;
        this.services = services;
    }

    /// <summary>
    ///     Create a new service component with modifications. Not all properties may be set to meaningful values.
    /// </summary>
    /// <param name="implementation">The implementation type.</param>
    /// <param name="services">The services to register for the <paramref name="implementation"/>.</param>
    /// <param name="lifetime">The service lifetime.</param>
    /// <param name="serviceKey">The shared service key.</param>
    /// <param name="serviceKeyProvider">The dynamic service key provider.</param>
    private ServiceComponent(Type implementation, IReadOnlyList<Type> services, ServiceLifetime lifetime, object? serviceKey, Func<Type, Type, object?>? serviceKeyProvider)
    {
        this.implementation = implementation;
        this.services = services;
        this.lifetime = lifetime;
        this.serviceKey = serviceKey;
        this.serviceKeyProvider = serviceKeyProvider;
    }

    /// <summary>
    ///     Set the <see cref="ServiceDescriptor.Lifetime"/> of the future <see cref="ServiceDescriptor"/> instances.
    /// </summary>
    /// <param name="lifetime">The lifetime.</param>
    public ServiceComponent WithLifetime(ServiceLifetime lifetime)
    {
        if (lifetime == this.lifetime)
        {
            return this;
        }

        return new ServiceComponent(this.implementation, this.services, lifetime, this.serviceKey, this.serviceKeyProvider);
    }

    /// <summary>
    ///     Set the <see cref="ServiceDescriptor.ServiceKey"/> of the future <see cref="ServiceDescriptor"/> instances.
    /// </summary>
    /// <param name="serviceKey">The shared service key.</param>
    public ServiceComponent WithServiceKey(object? serviceKey)
    {
        if (serviceKey == this.serviceKey)
        {
            return this;
        }

        return new ServiceComponent(this.implementation, this.services, this.lifetime, serviceKey, null);
    }

    /// <summary>
    ///     Indirectly set the <see cref="ServiceDescriptor.ServiceKey"/> of the future <see cref="ServiceDescriptor"/>
    ///     instances by evaluating the key when resolving the descriptors.
    /// </summary>
    /// <param name="serviceKeyProvider">The dynamic service key.</param>
    public ServiceComponent WithServiceKey(Func<Type, Type, object?> serviceKeyProvider)
    {
        if (serviceKeyProvider == this.serviceKeyProvider)
        {
            return this;
        }

        return new ServiceComponent(this.implementation, this.services, this.lifetime, null, serviceKeyProvider);
    }

    /// <summary>
    ///         Evaluate the <see cref="ServiceDescriptor"/> represented by this component.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ServiceDescriptor> GetServiceDescriptors()
    {
        return IsAnchoringRequired() ? GetAnchoredServiceDescriptors() : GetUnanchoredServiceDescriptors();
    }

    private bool IsAnchoringRequired()
    {
        // Transient types won't benefit from anchoring
        if (this.lifetime == ServiceLifetime.Transient)
        {
            return false;
        }

        // Open generic types can't be anchored
        if (this.implementation.IsGenericTypeDefinition)
        {
            return false;
        }

        return true;
    }

    private IEnumerable<ServiceDescriptor> GetUnanchoredServiceDescriptors()
    {
        foreach (var service in this.services)
        {
            if (this.serviceKeyProvider != null)
            {
                var specificServiceKey = this.serviceKeyProvider(this.implementation, service);
                yield return new ServiceDescriptor(service, specificServiceKey, this.implementation, this.lifetime);
            }
            else
            {
                // If the service key is null then it's a non-keyed service anyway
                yield return new ServiceDescriptor(service, this.serviceKey, this.implementation, this.lifetime);
            }
        }
    }

    private IEnumerable<ServiceDescriptor> GetAnchoredServiceDescriptors()
    {
        var isAnchorBeingRegistered = this.services.Contains(this.implementation);

        var component = this;
        Func<IServiceProvider, object?, object> anchorForwarder;
        if (isAnchorBeingRegistered)
        {
            yield return new ServiceDescriptor(this.implementation, this.implementation, this.lifetime);
            anchorForwarder = (sp, _) => sp.GetRequiredService(component.implementation);
        }
        else
        {
            var anchorKey = new AnchorServiceKey();
            yield return new ServiceDescriptor(this.implementation, anchorKey, this.implementation, this.lifetime);
            anchorForwarder = (sp, _) => sp.GetRequiredKeyedService(component.implementation, anchorKey);
        }
        foreach (var service in this.services)
        {
            // Always skip the anchor if it is present; it was registered above
            if (service == this.implementation)
            {
                continue;
            }

            if (this.serviceKeyProvider != null)
            {
                var specificServiceKey = this.serviceKeyProvider(this.implementation, service);
                yield return new ServiceDescriptor(service, specificServiceKey, anchorForwarder, this.lifetime);
            }
            else
            {
                // If service key is null then the forwarder drops it's key parameter
                yield return new ServiceDescriptor(service, this.serviceKey, anchorForwarder, this.lifetime);
            }
        }
    }

    internal readonly record struct AnchorServiceKey()
    {
        private readonly string key = $"<zcrew:anchor:{Guid.NewGuid():N}>";

        public override string ToString()
        {
            return this.key;
        }
    }
}
