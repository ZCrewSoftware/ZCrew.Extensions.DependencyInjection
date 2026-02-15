using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection;

public static partial class DecoratorServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        internal void AddDecorator(DecoratorServiceDescriptor decoratorServiceDescriptor
        )
        {
            if (!services.TryAddDecorator(decoratorServiceDescriptor))
            {
                throw NoServicesFound(decoratorServiceDescriptor);
            }
        }

        internal bool TryAddDecorator(DecoratorServiceDescriptor decoratorServiceDescriptor
        )
        {
            var serviceKey = decoratorServiceDescriptor.ServiceKey;
            var serviceType = decoratorServiceDescriptor.ServiceType;
            var lifetime = decoratorServiceDescriptor.Lifetime;
            var decorators = default(List<ServiceDescriptor>);
            for (var i = 0; i < services.Count; i++)
            {
                var service = services[i];
                if (service.ServiceType != serviceType || !Equals(service.ServiceKey, serviceKey))
                {
                    continue;
                }

                // Ensure that the decorator doesn't cause lifetime issues. For example, a singleton decorator which
                // depends on a transient delegate would never be able to acquire a new instance of the delegate.
                if (lifetime != null && lifetime.Value.Exceeds(service.Lifetime))
                {
                    throw LifetimeDependencyException(lifetime.Value, service);
                }

                // Replace the service with a new descriptor that has a unique service key
                service = service.WithServiceKey(Guid.NewGuid());
                services[i] = service;

                var decorator = decoratorServiceDescriptor.ToServiceDescriptor(service.ServiceKey, service.Lifetime);
                decorators ??= [];
                decorators.Add(decorator);
            }

            if (decorators == null)
            {
                return false;
            }

            foreach (var decorator in decorators)
            {
                services.Add(decorator);
            }
            return true;
        }
    }

    private static InvalidOperationException NoServicesFound(DecoratorServiceDescriptor decoratorServiceDescriptor)
    {
        throw new InvalidOperationException(
            $"Failed to locate delegate service. Expected service: {decoratorServiceDescriptor.ToServiceString()}"
        );
    }

    private static InvalidOperationException LifetimeDependencyException(
        ServiceLifetime decoratorLifetime,
        ServiceDescriptor serviceDescriptor
    )
    {
        return new InvalidOperationException(
            $"Decorator lifetime of {decoratorLifetime} is longer than lifetime for: {serviceDescriptor}"
        );
    }
}
