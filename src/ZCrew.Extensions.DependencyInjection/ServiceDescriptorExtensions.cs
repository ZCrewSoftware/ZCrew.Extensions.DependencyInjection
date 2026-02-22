using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection;

/// <summary>
///     Extension methods on <see cref="ServiceDescriptor"/> for recreating descriptors with a different service key
///     or lifetime.
/// </summary>
public static class ServiceDescriptorExtensions
{
    extension(ServiceDescriptor serviceDescriptor)
    {
        /// <summary>
        ///     Returns a new <see cref="ServiceDescriptor"/> registered under the specified
        ///     <paramref name="serviceKey"/>. If the descriptor is already keyed, the key is replaced; otherwise the
        ///     descriptor is converted to a keyed registration.
        /// </summary>
        /// <param name="serviceKey">
        ///     The service key to assign, or <see langword="null"/> for the default key.
        /// </param>
        public ServiceDescriptor WithServiceKey(object? serviceKey)
        {
            return serviceDescriptor.IsKeyedService
                ? RecreateKeyedServiceDescriptor(serviceDescriptor, serviceKey)
                : RecreateAsKeyedServiceDescriptor(serviceDescriptor, serviceKey);
        }

        /// <summary>
        ///     Returns a new <see cref="ServiceDescriptor"/> with the specified <paramref name="lifetime"/>.
        ///     Instance-based descriptors that cannot change lifetime will throw.
        /// </summary>
        /// <param name="lifetime">The target service lifetime.</param>
        public ServiceDescriptor WithLifetime(ServiceLifetime lifetime)
        {
            return serviceDescriptor.WithLifetime(lifetime, ignoreSingletonImplementations: false);
        }

        /// <summary>
        ///     Returns a new <see cref="ServiceDescriptor"/> with the specified <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="lifetime">The target service lifetime.</param>
        /// <param name="ignoreSingletonImplementations">
        ///     When <see langword="true"/>, instance-based descriptors that can only be singletons are returned
        ///     unchanged instead of throwing.
        /// </param>
        public ServiceDescriptor WithLifetime(ServiceLifetime lifetime, bool ignoreSingletonImplementations)
        {
            return serviceDescriptor.IsKeyedService
                ? RecreateKeyedServiceDescriptor(serviceDescriptor, lifetime, ignoreSingletonImplementations)
                : RecreateServiceDescriptor(serviceDescriptor, lifetime, ignoreSingletonImplementations);
        }
    }

    private static ServiceDescriptor RecreateServiceDescriptor(
        ServiceDescriptor serviceDescriptor,
        ServiceLifetime lifetime,
        bool ignoreSingletonImplementations
    )
    {
        if (serviceDescriptor.ImplementationType != null)
        {
            return new ServiceDescriptor(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, lifetime);
        }

        if (serviceDescriptor.ImplementationFactory != null)
        {
            return new ServiceDescriptor(
                serviceDescriptor.ServiceType,
                serviceDescriptor.ImplementationFactory,
                lifetime
            );
        }

        // Only singleton is acceptable for implementation service descriptors
        if (serviceDescriptor.ImplementationInstance != null)
        {
            if (lifetime == ServiceLifetime.Singleton || ignoreSingletonImplementations)
            {
                return serviceDescriptor;
            }
        }

        throw ServiceDescriptorLifetimeException(serviceDescriptor, lifetime);
    }

    private static ServiceDescriptor RecreateKeyedServiceDescriptor(
        ServiceDescriptor serviceDescriptor,
        ServiceLifetime lifetime,
        bool ignoreSingletonImplementations
    )
    {
        if (serviceDescriptor.KeyedImplementationType != null)
        {
            return new ServiceDescriptor(
                serviceDescriptor.ServiceType,
                serviceDescriptor.ServiceKey,
                serviceDescriptor.KeyedImplementationType,
                lifetime
            );
        }

        if (serviceDescriptor.KeyedImplementationFactory != null)
        {
            return new ServiceDescriptor(
                serviceDescriptor.ServiceType,
                serviceDescriptor.ServiceKey,
                serviceDescriptor.KeyedImplementationFactory,
                lifetime
            );
        }

        // Only singleton is acceptable for implementation service descriptors - it would be unchanged
        if (serviceDescriptor.KeyedImplementationInstance != null)
        {
            if (lifetime == ServiceLifetime.Singleton || ignoreSingletonImplementations)
            {
                return serviceDescriptor;
            }
        }

        throw ServiceDescriptorLifetimeException(serviceDescriptor, lifetime);
    }

    private static ServiceDescriptor RecreateAsKeyedServiceDescriptor(
        ServiceDescriptor serviceDescriptor,
        object? serviceKey
    )
    {
        if (serviceDescriptor.ImplementationType != null)
        {
            return new ServiceDescriptor(
                serviceDescriptor.ServiceType,
                serviceKey,
                serviceDescriptor.ImplementationType,
                serviceDescriptor.Lifetime
            );
        }

        if (serviceDescriptor.ImplementationFactory != null)
        {
            return new ServiceDescriptor(
                serviceDescriptor.ServiceType,
                serviceKey,
                (serviceProvider, _) => serviceDescriptor.ImplementationFactory.Invoke(serviceProvider),
                serviceDescriptor.Lifetime
            );
        }

        if (serviceDescriptor.ImplementationInstance != null)
        {
            return new ServiceDescriptor(
                serviceDescriptor.ServiceType,
                serviceKey,
                serviceDescriptor.ImplementationInstance
            );
        }
        throw ToKeyedServiceException(serviceDescriptor);
    }

    private static ServiceDescriptor RecreateKeyedServiceDescriptor(
        ServiceDescriptor serviceDescriptor,
        object? serviceKey
    )
    {
        if (serviceDescriptor.KeyedImplementationType != null)
        {
            return new ServiceDescriptor(
                serviceDescriptor.ServiceType,
                serviceKey,
                serviceDescriptor.KeyedImplementationType,
                serviceDescriptor.Lifetime
            );
        }

        if (serviceDescriptor.KeyedImplementationFactory != null)
        {
            return new ServiceDescriptor(
                serviceDescriptor.ServiceType,
                serviceKey,
                (serviceProvider, _) =>
                {
                    // This allows the keyed factory to receive the original service key
                    var originalServiceKey = serviceDescriptor.ServiceKey;
                    return serviceDescriptor.KeyedImplementationFactory.Invoke(serviceProvider, originalServiceKey);
                },
                serviceDescriptor.Lifetime
            );
        }

        if (serviceDescriptor.KeyedImplementationInstance != null)
        {
            return new ServiceDescriptor(
                serviceDescriptor.ServiceType,
                serviceKey,
                serviceDescriptor.KeyedImplementationInstance
            );
        }
        throw ToKeyedServiceException(serviceDescriptor);
    }

    private static InvalidOperationException ToKeyedServiceException(ServiceDescriptor serviceDescriptor)
    {
        return new InvalidOperationException($"Failed to create keyed service descriptor for: {serviceDescriptor}");
    }

    private static InvalidOperationException ServiceDescriptorLifetimeException(
        ServiceDescriptor serviceDescriptor,
        ServiceLifetime lifetime
    )
    {
        return new InvalidOperationException(
            $"Failed to change service descriptor lifetime to {lifetime} for: {serviceDescriptor}"
        );
    }
}
