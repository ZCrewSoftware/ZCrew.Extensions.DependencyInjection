using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Intermediate between the <see cref="ServiceSelector"/> and the raw <see cref="ServiceDescriptor"/>.
/// </summary>
internal readonly record struct ServiceComponent
{
    private readonly Type implementation;
    private readonly IReadOnlyList<Type> services;
    private readonly ServiceLifetime? lifetime;
    private readonly Func<Type, ServiceLifetime>? lifetimeProvider;
    private readonly object? serviceKey;
    private readonly Func<Type, Type, object?>? serviceKeyProvider;

    /// <summary>
    ///     The effective lifetime for this component: the value produced by the <see cref="lifetimeProvider"/> when
    ///     one is set (evaluated against the implementation type), otherwise the fixed <see cref="lifetime"/>.
    /// </summary>
    /// <remarks>
    ///     Either <see cref="implementation"/> or <see cref="lifetime"/> is set so the fallback is never hit.
    /// </remarks>
    private ServiceLifetime EffectiveLifetime =>
        this.lifetimeProvider?.Invoke(this.implementation) ?? this.lifetime ?? ServiceLifetime.Singleton;

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
    /// <param name="lifetimeProvider">The dynamic service lifetime provider.</param>
    /// <param name="serviceKey">The shared service key.</param>
    /// <param name="serviceKeyProvider">The dynamic service key provider.</param>
    private ServiceComponent(
        Type implementation,
        IReadOnlyList<Type> services,
        ServiceLifetime? lifetime,
        Func<Type, ServiceLifetime>? lifetimeProvider,
        object? serviceKey,
        Func<Type, Type, object?>? serviceKeyProvider
    )
    {
        this.implementation = implementation;
        this.services = services;
        this.lifetime = lifetime;
        this.lifetimeProvider = lifetimeProvider;
        this.serviceKey = serviceKey;
        this.serviceKeyProvider = serviceKeyProvider;
    }

    /// <summary>
    ///     Set the <see cref="ServiceDescriptor.Lifetime"/> of the future <see cref="ServiceDescriptor"/> instances.
    /// </summary>
    /// <param name="lifetime">The lifetime.</param>
    public ServiceComponent WithLifetime(ServiceLifetime lifetime)
    {
        if (lifetime == this.lifetime && this.lifetimeProvider == null)
        {
            return this;
        }

        return new ServiceComponent(
            this.implementation,
            this.services,
            lifetime,
            null,
            this.serviceKey,
            this.serviceKeyProvider
        );
    }

    /// <summary>
    ///     Indirectly set the <see cref="ServiceDescriptor.Lifetime"/> of the future <see cref="ServiceDescriptor"/>
    ///     instances by evaluating the lifetime against the implementation type when resolving the descriptors.
    /// </summary>
    /// <param name="lifetimeProvider">The dynamic service lifetime provider.</param>
    public ServiceComponent WithLifetime(Func<Type, ServiceLifetime> lifetimeProvider)
    {
        if (lifetimeProvider == this.lifetimeProvider)
        {
            return this;
        }

        return new ServiceComponent(
            this.implementation,
            this.services,
            this.lifetime,
            lifetimeProvider,
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

        return new ServiceComponent(
            this.implementation,
            this.services,
            this.lifetime,
            this.lifetimeProvider,
            serviceKey,
            null
        );
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

        return new ServiceComponent(
            this.implementation,
            this.services,
            this.lifetime,
            this.lifetimeProvider,
            null,
            serviceKeyProvider
        );
    }

    /// <summary>
    ///     Register the <see cref="ServiceDescriptor"/> instances represented by this component, using the supplied
    ///     <paramref name="sharingMode"/> to determine how a single implementation registered against multiple
    ///     service types shares its instance.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add the <see cref="ServiceDescriptor"/>(s) to.</param>
    /// <param name="sharingMode">
    ///     The sharing mode to apply to this component or <see langword="null"/> if the lifetime is dynamic and the
    ///     default sharing mode should be used instead.
    /// </param>
    /// <returns>The resulting service descriptors.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when sharing is requested for an open generic implementation. Microsoft's container does not
    ///     support factory-based resolution of open generics
    ///     (<see href="https://github.com/dotnet/runtime/issues/41050"/>) which is required to share an instance
    ///     across multiple service types.
    /// </exception>
    public void AddServiceDescriptors(IServiceCollection serviceCollection, SharingMode? sharingMode)
    {
        // No services to register
        if (this.services.Count == 0)
        {
            return;
        }

        // When dealing with a dynamic lifetime the sharing mode can't be set
        var componentSharingMode = sharingMode ?? EffectiveLifetime.DefaultSharingMode();

        // Don't need to bother sharing with only one registration
        if (componentSharingMode == SharingMode.Independent || this.services.Count == 1
        )
        {
            AddIndependentServiceDescriptors(serviceCollection);
            return;
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

        if (componentSharingMode == SharingMode.Dependent)
        {
            AddDependentServiceDescriptors(serviceCollection);
            return;
        }

        AddSharedComponentServiceDescriptors(serviceCollection);
    }

    private void AddIndependentServiceDescriptors(IServiceCollection serviceCollection)
    {
        foreach (var service in this.services)
        {
            serviceCollection.Add(Registration(service));
        }
    }

    private void AddSharedComponentServiceDescriptors(IServiceCollection serviceCollection)
    {
        var impl = this.implementation;
        Func<IServiceProvider, object?, object> factory;

        if (this.services.Contains(impl))
        {
            // If registering the implementation, then no shared component is necessary. Register the service as-is and
            // forward without keys
            serviceCollection.Add(Registration(impl));
            factory = (serviceProvider, _) => serviceProvider.GetRequiredService(impl);
        }
        else
        {
            // Otherwise, create a shared component with a unique key to reference from each service
            var sharedKey = new SharedComponentKey();
            serviceCollection.Add(SharedComponentRegistration(impl, sharedKey));
            factory = (serviceProvider, _) => serviceProvider.GetRequiredKeyedService(impl, sharedKey);
        }

        foreach (var service in this.services)
        {
            // Skip forwarding the service to itself. It was registered above
            if (service == this.implementation)
            {
                continue;
            }

            serviceCollection.Add(FactoryRegistration(service, factory));
        }
    }

    private void AddDependentServiceDescriptors(IServiceCollection serviceCollection)
    {
        var impl = this.implementation;
        Func<IServiceProvider, object?, object> factory = (serviceProvider, _) => serviceProvider.GetRequiredService(impl);
        foreach (var service in this.services)
        {
            // In Dependent mode the implementation must already be registered elsewhere. If the user happens to have
            // selected it as one of the service types, register it directly instead of pointing it at its own factory.
            if (service == this.implementation)
            {
                serviceCollection.Add(Registration(service));
                continue;
            }

            serviceCollection.Add(FactoryRegistration(service, factory));
        }
    }

    private ServiceDescriptor Registration(Type service)
    {
        var lifetime = EffectiveLifetime;
        if (this.serviceKeyProvider != null)
        {
            var specificServiceKey = this.serviceKeyProvider(this.implementation, service);
            return new ServiceDescriptor(service, specificServiceKey, this.implementation, lifetime);
        }
        // If the service key is null then it's a non-keyed service anyway
        return new ServiceDescriptor(service, this.serviceKey, this.implementation, lifetime);
    }

    private ServiceDescriptor FactoryRegistration(Type service, Func<IServiceProvider, object?, object> factory)
    {
        var lifetime = EffectiveLifetime;
        if (this.serviceKeyProvider != null)
        {
            var specificServiceKey = this.serviceKeyProvider(this.implementation, service);
            return new ServiceDescriptor(service, specificServiceKey, factory, lifetime);
        }

        // If the service key is null then it's a non-keyed service anyway
        return new ServiceDescriptor(service, this.serviceKey, factory, lifetime);
    }

    private ServiceDescriptor SharedComponentRegistration(Type service, SharedComponentKey key)
    {
        return new ServiceDescriptor(service, key, service, EffectiveLifetime);
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
