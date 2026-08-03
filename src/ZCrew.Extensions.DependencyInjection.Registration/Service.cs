using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     A single implementation type registered against one or more service types. Intermediate between the
///     <see cref="ServiceSelector"/> and the raw <see cref="ServiceDescriptor"/>. For
///     <see cref="ServiceLifetime.Singleton"/> and <see cref="ServiceLifetime.Scoped"/> registrations that include
///     the implementation among the services, all services resolve to a single shared instance. Begin a registration
///     with <see cref="From(Type)"/> or <see cref="From{T}"/>.
/// </summary>
public readonly record struct Service
{
    private const DynamicallyAccessedMemberTypes ImplementationMembers =
        DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.Interfaces;

    [DynamicallyAccessedMembers(ImplementationMembers)]
    private readonly Type implementation;
    private readonly IReadOnlyList<Type> services;
    private readonly IReadOnlyList<KeyValuePair<Type, object?>>? keyedServices;
    private readonly ServiceLifetime? lifetime;
    private readonly Func<Type, ServiceLifetime>? lifetimeProvider;
    private readonly object? serviceKey;

    private readonly Func<Type, Type, object?>? serviceKeyProvider;

    /// <summary>
    ///     Create a new service registered against the <paramref name="implementation"/> itself. Any further services
    ///     are added on top of it, so they resolve to a single shared instance.
    /// </summary>
    /// <param name="implementation">The implementation type.</param>
    internal Service([DynamicallyAccessedMembers(ImplementationMembers)] Type implementation)
    {
        this.implementation = implementation;
        this.services = [implementation];
    }

    /// <summary>
    ///     Create a new service.
    /// </summary>
    /// <param name="implementation">The implementation type.</param>
    /// <param name="services">The services to register for the <paramref name="implementation"/>.</param>
    internal Service(
        [DynamicallyAccessedMembers(ImplementationMembers)] Type implementation,
        IReadOnlyList<Type> services
    )
    {
        this.implementation = implementation;
        this.services = services;
    }

    /// <summary>
    ///     Create a service from the primitive registration values the source generator emits for a <c>[Service]</c>
    ///     declaration: the implementation seeded as its own service, the resolved lifetime and implementation key, and
    ///     each <c>[As]</c> service type paired with its own key.
    /// </summary>
    /// <param name="implementation">The implementation type.</param>
    /// <param name="lifetime">The resolved lifetime.</param>
    /// <param name="key">The implementation's own service key, or <see langword="null"/>.</param>
    /// <param name="serviceTypes">The additional service types with their per-type keys.</param>
    /// <remarks>
    ///     The service types are trusted as-declared and are not re-validated for assignability; the compiler and
    ///     analyzers already handled them.
    /// </remarks>
    private Service(
        [DynamicallyAccessedMembers(ImplementationMembers)] Type implementation,
        ServiceLifetime lifetime,
        object? key,
        (Type ServiceType, object? Key)[] serviceTypes
    )
    {
        this.implementation = implementation;
        this.services = [implementation];
        this.lifetime = lifetime;
        this.serviceKey = key;

        if (serviceTypes.Length > 0)
        {
            var pairs = new KeyValuePair<Type, object?>[serviceTypes.Length];
            for (var index = 0; index < serviceTypes.Length; index++)
            {
                pairs[index] = new KeyValuePair<Type, object?>(serviceTypes[index].ServiceType, serviceTypes[index].Key);
            }

            this.keyedServices = pairs;
        }
    }

    /// <summary>
    ///     Create a new service with modifications. Not all properties may be set to meaningful values.
    /// </summary>
    /// <param name="implementation">The implementation type.</param>
    /// <param name="services">The services to register for the <paramref name="implementation"/>.</param>
    /// <param name="lifetime">The service lifetime.</param>
    /// <param name="lifetimeProvider">The dynamic service lifetime provider.</param>
    /// <param name="serviceKey">The shared service key.</param>
    /// <param name="serviceKeyProvider">The dynamic service key provider.</param>
    /// <param name="keyedServices">The additional service types with their per-type keys.</param>
    private Service(
        [DynamicallyAccessedMembers(ImplementationMembers)] Type implementation,
        IReadOnlyList<Type> services,
        ServiceLifetime? lifetime,
        Func<Type, ServiceLifetime>? lifetimeProvider,
        object? serviceKey,
        Func<Type, Type, object?>? serviceKeyProvider,
        IReadOnlyList<KeyValuePair<Type, object?>>? keyedServices
    )
    {
        this.implementation = implementation;
        this.services = services;
        this.lifetime = lifetime;
        this.lifetimeProvider = lifetimeProvider;
        this.serviceKey = serviceKey;
        this.serviceKeyProvider = serviceKeyProvider;
        this.keyedServices = keyedServices;
    }

    /// <summary>
    ///     Begins registration from the specified <paramref name="type"/>. The service is registered against the
    ///     <paramref name="type"/> itself; services added with <c>As</c> are forwarded to it.
    /// </summary>
    /// <param name="type">The implementation type to build a registration from.</param>
    public static Service From([DynamicallyAccessedMembers(ImplementationMembers)] Type type)
    {
        return new Service(type);
    }

    /// <summary>
    ///     Begins registration from the specified type parameter <typeparamref name="T"/>. The service is registered
    ///     against the type itself; services added with <c>As</c> are forwarded to it.
    /// </summary>
    /// <typeparam name="T">The implementation type to build a registration from.</typeparam>
    public static Service From<[DynamicallyAccessedMembers(ImplementationMembers)] T>()
    {
        return new Service(typeof(T));
    }

    /// <summary>
    ///     Maps a <c>[Service]</c> declaration to a <see cref="Service"/> from its primitive registration values. The
    ///     <paramref name="implementation"/> is registered against itself plus each of the <paramref name="serviceTypes"/>
    ///     (resolving to a single shared instance for <see cref="ServiceLifetime.Singleton"/> and
    ///     <see cref="ServiceLifetime.Scoped"/> lifetimes), with the given <paramref name="lifetime"/> and per-type keys
    ///     applied. The implementation's own registration is keyed with <paramref name="key"/>.
    /// </summary>
    /// <remarks>
    ///     This overload exists for the code the source generator emits for <c>Services.FromThisAssembly()</c>; it is
    ///     not intended to be called directly. It is public only because that generated code compiles into the
    ///     consuming assembly, where an internal member would be unreachable. The attributes carrying these values live
    ///     in the consuming assembly (embedded by the generator), so this overload takes primitives rather than an
    ///     attribute instance.
    /// </remarks>
    /// <param name="implementation">The implementation type carrying the <c>[Service]</c> declaration.</param>
    /// <param name="lifetime">The resolved lifetime.</param>
    /// <param name="key">The implementation's own service key, or <see langword="null"/>.</param>
    /// <param name="serviceTypes">The additional service types with their per-type keys.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Service From(
        [DynamicallyAccessedMembers(ImplementationMembers)] Type implementation,
        ServiceLifetime lifetime,
        object? key,
        params (Type ServiceType, object? Key)[] serviceTypes
    )
    {
        return new Service(implementation, lifetime, key, serviceTypes);
    }

    /// <summary>
    ///     The implementation type.
    /// </summary>
    [DynamicallyAccessedMembers(ImplementationMembers)]
    public Type ImplementationType => this.implementation;

    /// <summary>
    ///     The non-distinct service types. Services will be registered once, in-order.
    /// </summary>
    public IReadOnlyList<Type> ServiceTypes => this.services;

    /// <summary>
    ///     Adds the <paramref name="service"/> to this service. This verifies that the
    ///     <see cref="ImplementationType"/> is assignable to the <paramref name="service"/>. A duplicate service can be
    ///     added; but, it is excluded when registering the service.
    /// </summary>
    /// <param name="service">The service to add.</param>
    /// <returns>The modified service.</returns>
    /// <exception cref="ArgumentException">
    ///     If the <paramref name="service"/> isn't a base type of the implementation.
    /// </exception>
    public Service As(Type service)
    {
        ArgumentNullException.ThrowIfNull(service);

        if (!this.implementation.IsBasedOn(service))
        {
            throw new ArgumentException(
                $"The implementation {this.implementation} is not based on the service type {service}"
            );
        }
        return new Service(
            this.implementation,
            this.services.Append(service).ToArray(),
            this.lifetime,
            this.lifetimeProvider,
            this.serviceKey,
            this.serviceKeyProvider,
            this.keyedServices
        );
    }

    /// <summary>
    ///     Adds the <paramref name="services"/> to this service. This verifies that the
    ///     <see cref="ImplementationType"/> is assignable to each service. Duplicate services can be added; but, they
    ///     are excluded when registering the service.
    /// </summary>
    /// <param name="services">The services to add.</param>
    /// <returns>The modified service or the same service if no services were added.</returns>
    /// <exception cref="ArgumentException">If any service isn't a base type of the implementation.</exception>
    public Service As(IEnumerable<Type> services)
    {
        return As(_ => services);
    }

    /// <summary>
    ///     Adds the result of calling <paramref name="serviceSelector"/> to this service. This verifies that the
    ///     <see cref="ImplementationType"/> is assignable to each service. Duplicate services can be added; but, they
    ///     are excluded when registering the service.
    /// </summary>
    /// <param name="serviceSelector">The delegate that provides services.</param>
    /// <returns>The modified service or the same service if no services were added.</returns>
    /// <exception cref="ArgumentException">If any service isn't a base type of the implementation.</exception>
    public Service As(Func<Type, IEnumerable<Type>> serviceSelector)
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
    /// <returns>The modified service or the same service if no services were added.</returns>
    internal Service AsUnchecked(IEnumerable<Type> services)
    {
        return new Service(
            this.implementation,
            this.services.Concat(services).ToArray(),
            this.lifetime,
            this.lifetimeProvider,
            this.serviceKey,
            this.serviceKeyProvider,
            this.keyedServices
        );
    }

    /// <summary>
    ///     Set the <see cref="ServiceDescriptor.Lifetime"/> of the future <see cref="ServiceDescriptor"/> instances.
    /// </summary>
    /// <param name="lifetime">The lifetime.</param>
    /// <returns>The modified service or the same service if the <see cref="lifetime"/> was the same.</returns>
    public Service AsLifetime(ServiceLifetime lifetime)
    {
        if (lifetime == this.lifetime && this.lifetimeProvider == null)
        {
            return this;
        }

        return new Service(
            this.implementation,
            this.services,
            lifetime,
            null,
            this.serviceKey,
            this.serviceKeyProvider,
            this.keyedServices
        );
    }

    /// <summary>
    ///     Indirectly set the <see cref="ServiceDescriptor.Lifetime"/> of the future <see cref="ServiceDescriptor"/>
    ///     instances by evaluating the lifetime against the implementation type when resolving the descriptors.
    /// </summary>
    /// <param name="lifetimeProvider">The dynamic service lifetime provider.</param>
    /// <returns>
    ///         The modified service or the same service if the <see cref="lifetimeProvider"/> was the same reference.
    /// </returns>
    public Service AsLifetime(Func<Type, ServiceLifetime> lifetimeProvider)
    {
        if (lifetimeProvider == this.lifetimeProvider)
        {
            return this;
        }

        return new Service(
            this.implementation,
            this.services,
            this.lifetime,
            lifetimeProvider,
            this.serviceKey,
            this.serviceKeyProvider,
            this.keyedServices
        );
    }

    /// <summary>
    ///     Remove any service keys from the service.
    /// </summary>
    /// <returns>The modified <see cref="Service"/> or the same service if it was already unkeyed.</returns>
    public Service Unkeyed()
    {
        if (this.serviceKeyProvider == null && this.serviceKey == null)
        {
            return this;
        }

        return new Service(
            this.implementation,
            this.services,
            this.lifetime,
            this.lifetimeProvider,
            null,
            null,
            this.keyedServices
        );
    }

    /// <summary>
    ///     Set the <see cref="ServiceDescriptor.ServiceKey"/> of the service.
    /// </summary>
    /// <param name="serviceKey">The shared service key.</param>
    /// <returns>The modified <see cref="Service"/> or the same service if it was already keyed.</returns>
    public Service Keyed(object? serviceKey)
    {
        if (serviceKey == this.serviceKey)
        {
            return this;
        }

        return new Service(
            this.implementation,
            this.services,
            this.lifetime,
            this.lifetimeProvider,
            serviceKey,
            null,
            this.keyedServices
        );
    }

    /// <summary>
    ///     Set the <see cref="ServiceDescriptor.ServiceKey"/> of the service through the
    /// <paramref name="serviceKeyProvider"/> delegate, evaluated when adding the service to a
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceKeyProvider">The service key provider.</param>
    public Service Keyed(Func<Type, Type, object?> serviceKeyProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceKeyProvider);
        if (serviceKeyProvider == this.serviceKeyProvider)
        {
            return this;
        }

        return new Service(
            this.implementation,
            this.services,
            this.lifetime,
            this.lifetimeProvider,
            null,
            serviceKeyProvider,
            this.keyedServices
        );
    }

    /// <summary>
    ///     Register the <see cref="ServiceDescriptor"/> instances represented by this service. Each service type is
    ///     registered directly against the implementation when the lifetime is
    ///     <see cref="ServiceLifetime.Transient"/>, when there is only a single service type, or when the
    ///     implementation is not itself one of the selected service types. Otherwise (a
    ///     <see cref="ServiceLifetime.Scoped"/> or <see cref="ServiceLifetime.Singleton"/> service with multiple
    ///     service types that include the implementation) the implementation is registered once and the remaining
    ///     service types are forwarded to it so they resolve to a single shared instance.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add the <see cref="ServiceDescriptor"/>(s) to.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the shared-service path is taken for an open generic implementation. Microsoft's container
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

        // Build the (service, key) entries: each declared service type paired with its resolved key, then any
        // per-service-type keyed services from the source-generator path.
        var entries = new List<(Type Service, object? Key)>(this.services.Count + (this.keyedServices?.Count ?? 0));
        foreach (var service in this.services)
        {
            entries.Add((service, this.serviceKeyProvider?.Invoke(this.implementation, service) ?? this.serviceKey));
        }

        if (this.keyedServices is not null)
        {
            foreach (var keyedService in this.keyedServices)
            {
                entries.Add((keyedService.Key, keyedService.Value));
            }
        }

        // Preserve ordering in-case it matters but de-duplicate by (service, key) so the same service type can appear
        // more than once under different keys while identical registrations collapse.
        var seen = new HashSet<(Type, object?)>(entries.Count);
        var distinctServices = new List<(Type Service, object? Key)>(entries.Count);
        foreach (var entry in entries)
        {
            if (seen.Add(entry))
            {
                distinctServices.Add(entry);
            }
        }

        // Shortcut for when there is only 1 - this also skips the open generic check and shared processing
        if (distinctServices.Count == 1)
        {
            serviceCollection.Add(Registration(lifetime, distinctServices[0].Service, distinctServices[0].Key));
            return;
        }

        var implementationAmongServices = false;
        foreach (var entry in distinctServices)
        {
            if (entry.Service == this.implementation)
            {
                implementationAmongServices = true;
                break;
            }
        }

        // This also doesn't register a shared service if the implementation isn't in the service list
        if (lifetime == ServiceLifetime.Transient || !implementationAmongServices)
        {
            foreach (var entry in distinctServices)
            {
                serviceCollection.Add(Registration(lifetime, entry.Service, entry.Key));
            }
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

        AddSharedServiceDescriptors(distinctServices, lifetime, serviceCollection);
    }

    private void AddSharedServiceDescriptors(
        List<(Type Service, object? Key)> services,
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

        var implementationRegistered = false;
        foreach (var entry in services)
        {
            // The implementation registers itself directly under its own key; every other service type (including the
            // implementation under a different key) forwards to it so they share the one instance.
            if (!implementationRegistered && entry.Service == impl && Equals(entry.Key, implementationKey))
            {
                serviceCollection.Add(Registration(lifetime, impl, entry.Key));
                implementationRegistered = true;
                continue;
            }

            serviceCollection.Add(ForwardRegistration(lifetime, entry.Service, entry.Key, factory));
        }
    }

    private ServiceDescriptor Registration(ServiceLifetime lifetime, Type service, object? serviceKey)
    {
        return new ServiceDescriptor(service, serviceKey, this.implementation, lifetime);
    }

    private ServiceDescriptor ForwardRegistration(
        ServiceLifetime lifetime,
        Type service,
        object? serviceKey,
        Func<IServiceProvider, object?, object> factory
    )
    {
        return new ServiceDescriptor(service, serviceKey, factory, lifetime);
    }
}
