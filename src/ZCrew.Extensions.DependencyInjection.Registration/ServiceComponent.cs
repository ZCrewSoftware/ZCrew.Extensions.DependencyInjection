using System.Diagnostics;
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
    public void AddServiceDescriptors(IServiceCollection serviceCollection)
    {
        // No services to register
        if (this.services.Count == 0)
        {
            return;
        }

        var lifetime = this.lifetimeProvider?.Invoke(this.implementation) ?? this.lifetime;
        Debug.Assert(lifetime != null, "Lifetime always should have been set");

        // Shortcut for when there is only 1 - this also skips the open generic check
        // This also doesn't register a shared component if the implementation isn't in the service list
        if (lifetime == ServiceLifetime.Transient || this.services.Count == 1 || !this.services.Contains(this.implementation))
        {
            AddIndependentServiceDescriptors(lifetime.Value, serviceCollection);
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

        AddComponentServiceDescriptors(lifetime.Value, serviceCollection);
    }

    private void AddIndependentServiceDescriptors(ServiceLifetime lifetime, IServiceCollection serviceCollection)
    {
        foreach (var service in this.services)
        {
            serviceCollection.Add(Registration(lifetime, service));
        }
    }

    private void AddComponentServiceDescriptors(ServiceLifetime lifetime, IServiceCollection serviceCollection)
    {
        var impl = this.implementation;
        Func<IServiceProvider, object?, object> factory = (serviceProvider, _) => serviceProvider.GetRequiredService(impl);
        foreach (var service in this.services)
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
        Func<IServiceProvider, object?, object> factory)
    {
        var specificServiceKey = this.serviceKeyProvider?.Invoke(this.implementation, service) ?? this.serviceKey;
        return new ServiceDescriptor(service, specificServiceKey, factory, lifetime);
    }
}
