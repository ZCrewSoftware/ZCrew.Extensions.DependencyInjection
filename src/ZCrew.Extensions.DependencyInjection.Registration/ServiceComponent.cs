using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Intermediate between the <see cref="ServiceSelector"/> and the raw <see cref="ServiceDescriptor"/>.
/// </summary>
public readonly record struct ServiceComponent
{
    private readonly Type implementation;
    private readonly IReadOnlyList<Type> services;
    private readonly ServiceLifetime? lifetime;
    private readonly Func<Type, ServiceLifetime>? lifetimeProvider;
    private readonly object? serviceKey;

    private readonly Func<Type, Type, object?>? serviceKeyProvider;

    /// <summary>
    ///     Create a new service component registered against the <paramref name="implementation"/> itself. Any further
    ///     services are added on top of it, so they resolve to a single shared instance.
    /// </summary>
    /// <param name="implementation">The implementation type.</param>
    internal ServiceComponent(Type implementation)
    {
        this.implementation = implementation;
        this.services = [implementation];
    }

    /// <summary>
    ///     Create a new service component.
    /// </summary>
    /// <param name="implementation">The implementation type.</param>
    /// <param name="services">The services to register for the <paramref name="implementation"/>.</param>
    internal ServiceComponent(Type implementation, IReadOnlyList<Type> services)
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
    ///     The implementation type.
    /// </summary>
    public Type ImplementationType => this.implementation;

    /// <summary>
    ///     The non-distinct service types. Services will be registered once, in-order.
    /// </summary>
    public IReadOnlyList<Type> ServiceTypes => this.services;

    /// <summary>
    ///     Adds the <paramref name="service"/> to this component. This verifies that the
    ///     <see cref="ImplementationType"/> is assignable to the <paramref name="service"/>. A duplicate service can be
    ///     added; but, it is excluded when registering the component.
    /// </summary>
    /// <param name="service">The service to add.</param>
    /// <returns>The modified component.</returns>
    /// <exception cref="ArgumentException">
    ///     If the <paramref name="service"/> isn't a base type of the implementation.
    /// </exception>
    public ServiceComponent As(Type service)
    {
        ArgumentNullException.ThrowIfNull(service);

        if (!this.implementation.IsBasedOn(service))
        {
            throw new ArgumentException(
                $"The implementation {this.implementation} is not based on the service type {service}"
            );
        }
        return new ServiceComponent(
            this.implementation,
            this.services.Append(service).ToArray(),
            this.lifetime,
            this.lifetimeProvider,
            this.serviceKey,
            this.serviceKeyProvider
        );
    }

    /// <summary>
    ///     Adds the <paramref name="services"/> to this component. This verifies that the
    ///     <see cref="ImplementationType"/> is assignable to each service. Duplicate services can be added; but, they
    ///     are excluded when registering the component.
    /// </summary>
    /// <param name="services">The services to add.</param>
    /// <returns>The modified component or the same component if no services were added.</returns>
    /// <exception cref="ArgumentException">If any service isn't a base type of the implementation.</exception>
    public ServiceComponent As(IEnumerable<Type> services)
    {
        return As(_ => services);
    }

    /// <summary>
    ///     Adds the result of calling <paramref name="serviceSelector"/> to this component. This verifies that the
    ///     <see cref="ImplementationType"/> is assignable to each service. Duplicate services can be added; but, they
    ///     are excluded when registering the component.
    /// </summary>
    /// <param name="serviceSelector">The delegate that provides services.</param>
    /// <returns>The modified component or the same component if no services were added.</returns>
    /// <exception cref="ArgumentException">If any service isn't a base type of the implementation.</exception>
    public ServiceComponent As(Func<Type, IEnumerable<Type>> serviceSelector)
    {
        ArgumentNullException.ThrowIfNull(serviceSelector);
        var services =  serviceSelector(this.implementation);
        ArgumentNullException.ThrowIfNull(services);
        var serviceArray = services.ToArray();
        if (serviceArray.Length == 0)
        {
            return this;
        }

        foreach (var service in serviceArray)
        {
            if (!this.implementation.IsBasedOn(service))
            {
                throw new ArgumentException(
                    $"The implementation {this.implementation} is not based on the service type {service}"
                );
            }
        }
        return AsUnchecked(serviceArray);
    }

    /// <summary>
    ///     Like <see cref="As(IEnumerable{Type})"/> but unchecked, only for the registration fluent API where the
    ///     services are base types.
    /// </summary>
    /// <param name="services">The services to add.</param>
    /// <returns>The modified component or the same component if no services were added.</returns>
    internal ServiceComponent AsUnchecked(IEnumerable<Type> services)
    {
        return new ServiceComponent(
            this.implementation,
            this.services.Concat(services).ToArray(),
            this.lifetime,
            this.lifetimeProvider,
            this.serviceKey,
            this.serviceKeyProvider
        );
    }

    /// <summary>
    ///     Set the <see cref="ServiceDescriptor.Lifetime"/> of the future <see cref="ServiceDescriptor"/> instances.
    /// </summary>
    /// <param name="lifetime">The lifetime.</param>
    /// <returns>The modified component or the same component if the <see cref="lifetime"/> was the same.</returns>
    public ServiceComponent AsLifetime(ServiceLifetime lifetime)
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
    /// <returns>
    ///         The modified component or the same component if the <see cref="lifetimeProvider"/> was the same reference.
    /// </returns>
    public ServiceComponent AsLifetime(Func<Type, ServiceLifetime> lifetimeProvider)
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
    ///     Remove any service keys from the component.
    /// </summary>
    /// <returns>The modified <see cref="ServiceComponent"/> or the same component if it was already unkeyed.</returns>
    public ServiceComponent Unkeyed()
    {
        if (this.serviceKeyProvider == null && this.serviceKey == null)
        {
            return this;
        }

        return new ServiceComponent(
            this.implementation,
            this.services,
            this.lifetime,
            this.lifetimeProvider,
            null,
            null
        );
    }

    /// <summary>
    ///     Set the <see cref="ServiceDescriptor.ServiceKey"/> of the component.
    /// </summary>
    /// <param name="serviceKey">The shared service key.</param>
    /// <returns>The modified <see cref="ServiceComponent"/> or the same component if it was already keyed.</returns>
    public ServiceComponent Keyed(object? serviceKey)
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
    ///     Set the <see cref="ServiceDescriptor.ServiceKey"/> of the component through the
    /// <paramref name="serviceKeyProvider"/> delegate, evaluated when adding the component to a
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceKeyProvider">The service key provider.</param>
    public ServiceComponent Keyed(Func<Type, Type, object?> serviceKeyProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceKeyProvider);
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
    ///     Register the <see cref="ServiceDescriptor"/> instances represented by this component. Each service type is
    ///     registered directly against the implementation when the lifetime is
    ///     <see cref="ServiceLifetime.Transient"/>, when there is only a single service type, or when the
    ///     implementation is not itself one of the selected service types. Otherwise (a
    ///     <see cref="ServiceLifetime.Scoped"/> or <see cref="ServiceLifetime.Singleton"/> component with multiple
    ///     service types that include the implementation) the implementation is registered once and the remaining
    ///     service types are forwarded to it so they resolve to a single shared instance.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add the <see cref="ServiceDescriptor"/>(s) to.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the shared-component path is taken for an open generic implementation. Microsoft's container
    ///     does not support factory-based resolution of open generics
    ///     (<see href="https://github.com/dotnet/runtime/issues/41050"/>) which is required to forward multiple
    ///     service types to a single instance.
    /// </exception>
    internal void AddServiceDescriptors(IServiceCollection serviceCollection)
    {
        // No services to register
        if (this.services.Count == 0)
        {
            return;
        }

        var lifetime = this.lifetimeProvider?.Invoke(this.implementation) ?? this.lifetime ?? ServiceLifetime.Singleton;

        // Shortcut for when there is only 1 - this also skips the open generic check and distinct processing
        if (this.services.Count == 1)
        {
            serviceCollection.Add(Registration(lifetime, this.services[0]));
            return;
        }

        // Preserve ordering in-case it matters but still expose the hashset instead of just using Distinct() which
        // just returns the underlying sequential-iterator
        var seenServices = new HashSet<Type>(this.services.Count);
        var distinctServices = new List<Type>(this.services.Count);
        foreach (var service in this.services)
        {
            if (seenServices.Add(service))
            {
                distinctServices.Add(service);
            }
        }

        // This also doesn't register a shared component if the implementation isn't in the service list
        if (lifetime == ServiceLifetime.Transient || !seenServices.Contains(this.implementation))
        {
            AddIndependentServiceDescriptors(distinctServices, lifetime, serviceCollection);
            return;
        }

        // Sharing can't be done with generic types due to a limitation in Microsoft DI:
        // services.AddSingleton(typeof(IRepository<>), sp => sp.GetRequiredService(typeof(Repository<>)));
        // See: https://github.com/dotnet/runtime/issues/41050
        if (this.implementation.IsGenericTypeDefinition)
        {
            throw new InvalidOperationException(
                "Open generic services can not be forwarded. "
                    + "This is due to a limitation of the Microsoft Dependency Injection container. "
                    + "For more information, see: https://github.com/dotnet/runtime/issues/41050"
            );
        }

        AddComponentServiceDescriptors(distinctServices, lifetime, serviceCollection);
    }

    private void AddIndependentServiceDescriptors(
        IEnumerable<Type> services,
        ServiceLifetime lifetime,
        IServiceCollection serviceCollection
    )
    {
        foreach (var service in services)
        {
            serviceCollection.Add(Registration(lifetime, service));
        }
    }

    private void AddComponentServiceDescriptors(
        IEnumerable<Type> services,
        ServiceLifetime lifetime,
        IServiceCollection serviceCollection
    )
    {
        var impl = this.implementation;
        var implementationKey = this.serviceKeyProvider?.Invoke(impl, impl) ?? this.serviceKey;
        Func<IServiceProvider, object?, object> factory =
            implementationKey == null
                ? (serviceProvider, _) => serviceProvider.GetRequiredService(impl)
                : (serviceProvider, _) => serviceProvider.GetRequiredKeyedService(impl, implementationKey);
        foreach (var service in services)
        {
            // Skip forwarding the service to itself
            if (service == impl)
            {
                serviceCollection.Add(Registration(lifetime, impl));
                continue;
            }

            serviceCollection.Add(ForwardRegistration(lifetime, service, factory));
        }
    }

    private ServiceDescriptor Registration(ServiceLifetime lifetime, Type service)
    {
        var serviceKey = this.serviceKeyProvider?.Invoke(this.implementation, service) ?? this.serviceKey;
        return new ServiceDescriptor(service, serviceKey, this.implementation, lifetime);
    }

    private ServiceDescriptor ForwardRegistration(
        ServiceLifetime lifetime,
        Type service,
        Func<IServiceProvider, object?, object> factory
    )
    {
        var specificServiceKey = this.serviceKeyProvider?.Invoke(this.implementation, service) ?? this.serviceKey;
        return new ServiceDescriptor(service, specificServiceKey, factory, lifetime);
    }
}
