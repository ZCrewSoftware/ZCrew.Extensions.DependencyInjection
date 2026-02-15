using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection;

public static class ServiceDescriptorExtensions
{
    public static ServiceDescriptor WithServiceKey(this ServiceDescriptor serviceDescriptor, object? serviceKey)
    {
        return serviceDescriptor.IsKeyedService
            ? RecreateKeyedServiceDescriptor(serviceDescriptor, serviceKey)
            : RecreateAsKeyedServiceDescriptor(serviceDescriptor, serviceKey);
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
        throw ServiceRecreationException(serviceDescriptor);
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
        throw ServiceRecreationException(serviceDescriptor);
    }

    private static InvalidOperationException ServiceRecreationException(ServiceDescriptor serviceDescriptor)
    {
        throw new InvalidOperationException($"Failed to create keyed service descriptor for: {serviceDescriptor}");
    }
}
