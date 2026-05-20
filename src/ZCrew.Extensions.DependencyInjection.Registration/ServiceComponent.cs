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
    private ServiceComponent(
        Type implementation,
        IReadOnlyList<Type> services,
        ServiceLifetime lifetime,
        object? serviceKey,
        Func<Type, Type, object?>? serviceKeyProvider
    )
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

        return new ServiceComponent(
            this.implementation,
            this.services,
            lifetime,
            this.serviceKey,
            this.serviceKeyProvider
        );
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
    ///     Evaluate the <see cref="ServiceDescriptor"/> instances represented by this component, using the supplied
    ///     <paramref name="sharingMode"/> to determine how a single implementation registered against multiple
    ///     service types shares its instance.
    /// </summary>
    /// <param name="sharingMode">The sharing mode to apply to this component.</param>
    /// <returns>The resulting service descriptors.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when sharing is requested for an open generic implementation. Microsoft's container does not
    ///     support factory-based resolution of open generics
    ///     (<see href="https://github.com/dotnet/runtime/issues/41050"/>) which is required to share an instance
    ///     across multiple service types.
    /// </exception>
    public IEnumerable<ServiceDescriptor> GetServiceDescriptors(SharingMode sharingMode)
    {
        // No services to register
        if (this.services.Count == 0)
        {
            return [];
        }

        // Don't need to bother sharing with only one registration
        if (sharingMode == SharingMode.Independent || this.services.Count == 1)
        {
            return GetIndependentServiceDescriptors();
        }

        // Sharing can't be done with generic types due to a limitation in Microsoft DI:
        // services.AddSingleton(typeof(IRepository<>), sp => sp.GetRequiredService(typeof(Repository<>)));
        // See: https://github.com/dotnet/runtime/issues/41050
        if (this.implementation.IsGenericTypeDefinition)
        {
            throw new InvalidOperationException(
                "Open generic services can not be forwarded. " +
                "This is due to a limitation of the Microsoft Dependency Injection container. " +
                "For more information, see: https://github.com/dotnet/runtime/issues/41050");
        }

        if (sharingMode == SharingMode.Dependent)
        {
            return GetDependentServiceDescriptors();
        }

        return GetSharedComponentServiceDescriptors();
    }

    private IEnumerable<ServiceDescriptor> GetIndependentServiceDescriptors()
    {
        return this.services.Select(Registration);
    }

    private IEnumerable<ServiceDescriptor> GetSharedComponentServiceDescriptors()
    {
        var impl = this.implementation;
        Func<IServiceProvider, object?, object> factory;

        if (this.services.Contains(impl))
        {
            // If registering the implementation, then no shared component is necessary. Register the service as-is and
            // forward without keys
            yield return Registration(impl);
            factory = (serviceProvider, _) => serviceProvider.GetRequiredService(impl);
        }
        else
        {
            // Otherwise, create a shared component with a unique key to reference from each service
            var sharedKey = new SharedComponentKey();
            yield return SharedComponentRegistration(impl, sharedKey);
            factory = (serviceProvider, _) => serviceProvider.GetRequiredKeyedService(impl, sharedKey);
        }

        foreach (var service in this.services)
        {
            // Skip forwarding the service to itself. It was registered above
            if (service == this.implementation)
            {
                continue;
            }

            yield return FactoryRegistration(service, factory);
        }
    }

    private IEnumerable<ServiceDescriptor> GetDependentServiceDescriptors()
    {
        var impl = this.implementation;
        Func<IServiceProvider, object?, object> factory = (serviceProvider, _) => serviceProvider.GetRequiredService(impl);
        foreach (var service in this.services)
        {
            // In Dependent mode the implementation must already be registered elsewhere. If the user happens to have
            // selected it as one of the service types, register it directly instead of pointing it at its own factory.
            if (service == this.implementation)
            {
                yield return Registration(service);
                continue;
            }

            yield return FactoryRegistration(service, factory);
        }
    }

    private ServiceDescriptor Registration(Type service)
    {
        if (this.serviceKeyProvider != null)
        {
            var specificServiceKey = this.serviceKeyProvider(this.implementation, service);
            return new ServiceDescriptor(service, specificServiceKey, this.implementation, this.lifetime);
        }
        // If the service key is null then it's a non-keyed service anyway
        return new ServiceDescriptor(service, this.serviceKey, this.implementation, this.lifetime);
    }

    private ServiceDescriptor FactoryRegistration(Type service, Func<IServiceProvider, object?, object> factory)
    {
        if (this.serviceKeyProvider != null)
        {
            var specificServiceKey = this.serviceKeyProvider(this.implementation, service);
            return new ServiceDescriptor(service, specificServiceKey, factory, this.lifetime);
        }

        // If the service key is null then it's a non-keyed service anyway
        return new ServiceDescriptor(service, this.serviceKey, factory, this.lifetime);
    }

    private ServiceDescriptor SharedComponentRegistration(Type service, SharedComponentKey key)
    {
        return new ServiceDescriptor(service, key, service, this.lifetime);
    }

    private readonly record struct SharedComponentKey
    {
        private readonly string key;

        public SharedComponentKey()
        {
            this.key = $"ZCrew:SharedComponent:{Guid.NewGuid():N}";
        }

        public override string ToString()
        {
            return this.key;
        }
    }
}
