using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ZCrew.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AsSingleton()
        {
            return services.AsLifetime(ServiceLifetime.Singleton);
        }

        public IServiceCollection AsScoped()
        {
            return services.AsLifetime(ServiceLifetime.Scoped);
        }

        public IServiceCollection AsScoped(bool ignoreSingletonImplementations)
        {
            return services.AsLifetime(ServiceLifetime.Scoped, ignoreSingletonImplementations);
        }

        public IServiceCollection AsTransient()
        {
            return services.AsLifetime(ServiceLifetime.Transient);
        }

        public IServiceCollection AsTransient(bool ignoreSingletonImplementations)
        {
            return services.AsLifetime(ServiceLifetime.Transient, ignoreSingletonImplementations);
        }

        public IServiceCollection AsLifetime(ServiceLifetime lifetime)
        {
            return services.AsLifetime(lifetime, ignoreSingletonImplementations: true);
        }

        public IServiceCollection AsLifetime(ServiceLifetime lifetime, bool ignoreSingletonImplementations)
        {
            var copy = new ServiceCollection();
            foreach (var descriptor in services)
            {
                copy.Add(descriptor.WithLifetime(lifetime, ignoreSingletonImplementations));
            }
            return copy;
        }

        public IServiceCollection AddSingleton(IEnumerable<ServiceDescriptor> descriptors)
        {
            return services.Add(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Singleton)));
        }

        public IServiceCollection AddScoped(IEnumerable<ServiceDescriptor> descriptors)
        {
            return services.Add(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Scoped)));
        }

        public IServiceCollection AddTransient(IEnumerable<ServiceDescriptor> descriptors)
        {
            return services.Add(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Scoped)));
        }

        public IServiceCollection TryAddSingleton(IEnumerable<ServiceDescriptor> descriptors)
        {
            services.TryAdd(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Singleton)));
            return services;
        }

        public IServiceCollection TryAddScoped(IEnumerable<ServiceDescriptor> descriptors)
        {
            services.TryAdd(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Scoped)));
            return services;
        }

        public IServiceCollection TryAddTransient(IEnumerable<ServiceDescriptor> descriptors)
        {
            services.TryAdd(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Scoped)));
            return services;
        }

        public IServiceCollection Replace(IEnumerable<ServiceDescriptor> descriptors)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(descriptors);

            foreach (var descriptor in descriptors)
            {
                services.Replace(descriptor);
            }

            return services;
        }

        public IServiceCollection ReplaceSingleton(IEnumerable<ServiceDescriptor> descriptors)
        {
            return services.Replace(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Singleton)));
        }

        public IServiceCollection ReplaceScoped(IEnumerable<ServiceDescriptor> descriptors)
        {
            return services.Replace(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Scoped)));
        }

        public IServiceCollection ReplaceTransient(IEnumerable<ServiceDescriptor> descriptors)
        {
            return services.Replace(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Scoped)));
        }
    }
}
