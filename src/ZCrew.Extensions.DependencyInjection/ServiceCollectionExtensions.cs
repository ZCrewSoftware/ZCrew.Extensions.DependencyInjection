using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ZCrew.Extensions.DependencyInjection;

/// <summary>
///     Extension methods on <see cref="IServiceCollection"/> for changing service lifetimes and bulk-adding, trying,
///     or replacing service descriptors.
/// </summary>
public static partial class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddSingleton(IEnumerable<ServiceDescriptor> descriptors)
        {
            ArgumentNullException.ThrowIfNull(descriptors);
            return services.Add(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Singleton)));
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddScoped(IEnumerable<ServiceDescriptor> descriptors)
        {
            ArgumentNullException.ThrowIfNull(descriptors);
            return services.Add(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Scoped)));
        }

        /// <summary>
        ///     Adds the descriptors as <see cref="ServiceLifetime.Transient"/> registrations.
        /// </summary>
        /// <param name="descriptors">The service descriptors to add.</param>
        public IServiceCollection AddTransient(IEnumerable<ServiceDescriptor> descriptors)
        {
            ArgumentNullException.ThrowIfNull(descriptors);
            return services.Add(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Transient)));
        }

        /// <summary>
        ///     Tries to add the descriptors as <see cref="ServiceLifetime.Singleton"/> registrations. Descriptors whose
        ///     service type is already registered are skipped.
        /// </summary>
        /// <param name="descriptors">The service descriptors to try adding.</param>
        public IServiceCollection TryAddSingleton(IEnumerable<ServiceDescriptor> descriptors)
        {
            ArgumentNullException.ThrowIfNull(descriptors);
            services.TryAdd(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Singleton)));
            return services;
        }

        /// <summary>
        ///     Tries to add the descriptors as <see cref="ServiceLifetime.Scoped"/> registrations. Descriptors whose
        ///     service type is already registered are skipped.
        /// </summary>
        /// <param name="descriptors">The service descriptors to try adding.</param>
        public IServiceCollection TryAddScoped(IEnumerable<ServiceDescriptor> descriptors)
        {
            ArgumentNullException.ThrowIfNull(descriptors);
            services.TryAdd(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Scoped)));
            return services;
        }

        /// <summary>
        ///     Tries to add the descriptors as <see cref="ServiceLifetime.Transient"/> registrations. Descriptors whose
        ///     service type is already registered are skipped.
        /// </summary>
        /// <param name="descriptors">The service descriptors to try adding.</param>
        public IServiceCollection TryAddTransient(IEnumerable<ServiceDescriptor> descriptors)
        {
            ArgumentNullException.ThrowIfNull(descriptors);
            services.TryAdd(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Transient)));
            return services;
        }

        /// <summary>
        ///     Replaces existing registrations for each descriptor's service type.
        /// </summary>
        /// <param name="descriptors">The service descriptors to replace with.</param>
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

        /// <summary>
        ///     Replaces existing registrations with <see cref="ServiceLifetime.Singleton"/> descriptors.
        /// </summary>
        /// <param name="descriptors">The service descriptors to replace with.</param>
        public IServiceCollection ReplaceSingleton(IEnumerable<ServiceDescriptor> descriptors)
        {
            ArgumentNullException.ThrowIfNull(descriptors);
            return services.Replace(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Singleton)));
        }

        /// <summary>
        ///     Replaces existing registrations with <see cref="ServiceLifetime.Scoped"/> descriptors.
        /// </summary>
        /// <param name="descriptors">The service descriptors to replace with.</param>
        public IServiceCollection ReplaceScoped(IEnumerable<ServiceDescriptor> descriptors)
        {
            ArgumentNullException.ThrowIfNull(descriptors);
            return services.Replace(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Scoped)));
        }

        /// <summary>
        ///     Replaces existing registrations with <see cref="ServiceLifetime.Transient"/> descriptors.
        /// </summary>
        /// <param name="descriptors">The service descriptors to replace with.</param>
        public IServiceCollection ReplaceTransient(IEnumerable<ServiceDescriptor> descriptors)
        {
            ArgumentNullException.ThrowIfNull(descriptors);
            return services.Replace(descriptors.Select(descriptor => descriptor.WithLifetime(ServiceLifetime.Transient)));
        }
    }
}
