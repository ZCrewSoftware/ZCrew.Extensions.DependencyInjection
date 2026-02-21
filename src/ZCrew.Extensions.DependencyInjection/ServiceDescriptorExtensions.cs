using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection;

public static class ServiceDescriptorExtensions
{
    extension(ServiceDescriptor serviceDescriptor)
    {
        public ServiceDescriptor WithServiceKey(object? serviceKey)
        {
            return serviceDescriptor.IsKeyedService
                ? RecreateKeyedServiceDescriptor(serviceDescriptor, serviceKey)
                : RecreateAsKeyedServiceDescriptor(serviceDescriptor, serviceKey);
        }

        public ServiceDescriptor WithLifetime(ServiceLifetime lifetime)
        {
            return serviceDescriptor.WithLifetime(lifetime, ignoreSingletonImplementations: false);
        }

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
            return new ServiceDescriptor(
                serviceDescriptor.ServiceType,
                serviceDescriptor.ImplementationType,
                lifetime
            );
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
        throw new InvalidOperationException($"Failed to create keyed service descriptor for: {serviceDescriptor}");
    }

    private static InvalidOperationException ServiceDescriptorLifetimeException(ServiceDescriptor serviceDescriptor, ServiceLifetime lifetime)
    {
        throw new InvalidOperationException($"Failed to change service descriptor lifetime to {lifetime} for: {serviceDescriptor}");
    }
}
