using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ZCrew.Extensions.DependencyInjection;

/// <summary>
///     Extension methods on <see cref="IServiceCollection"/> for changing service lifetimes and bulk-adding, trying,
///     or replacing service descriptors.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Returns a new <see cref="IServiceCollection"/> with all descriptors set to
        ///     <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public IServiceCollection AsSingleton()
        {
            return services.AsLifetime(ServiceLifetime.Singleton);
        }

        /// <summary>
        ///     Returns a new <see cref="IServiceCollection"/> with all descriptors set to
        ///     <see cref="ServiceLifetime.Scoped"/>.
        /// </summary>
        public IServiceCollection AsScoped()
        {
            return services.AsLifetime(ServiceLifetime.Scoped);
        }

        /// <summary>
        ///     Returns a new <see cref="IServiceCollection"/> with all descriptors set to
        ///     <see cref="ServiceLifetime.Scoped"/>.
        /// </summary>
        /// <param name="ignoreSingletonImplementations">
        ///     When <see langword="true"/>, instance-based descriptors that can only be singletons are kept unchanged
        ///     instead of throwing.
        /// </param>
        public IServiceCollection AsScoped(bool ignoreSingletonImplementations)
        {
            return services.AsLifetime(ServiceLifetime.Scoped, ignoreSingletonImplementations);
        }

        /// <summary>
        ///     Returns a new <see cref="IServiceCollection"/> with all descriptors set to
        ///     <see cref="ServiceLifetime.Transient"/>.
        /// </summary>
        public IServiceCollection AsTransient()
        {
            return services.AsLifetime(ServiceLifetime.Transient);
        }

        /// <summary>
        ///     Returns a new <see cref="IServiceCollection"/> with all descriptors set to
        ///     <see cref="ServiceLifetime.Transient"/>.
        /// </summary>
        /// <param name="ignoreSingletonImplementations">
        ///     When <see langword="true"/>, instance-based descriptors that can only be singletons are kept unchanged
        ///     instead of throwing.
        /// </param>
        public IServiceCollection AsTransient(bool ignoreSingletonImplementations)
        {
            return services.AsLifetime(ServiceLifetime.Transient, ignoreSingletonImplementations);
        }

        /// <summary>
        ///     Returns a new <see cref="IServiceCollection"/> with all descriptors set to the specified
        ///     <paramref name="lifetime"/>. Instance-based descriptors that cannot change lifetime are kept unchanged.
        /// </summary>
        /// <param name="lifetime">The target service lifetime.</param>
        public IServiceCollection AsLifetime(ServiceLifetime lifetime)
        {
            return services.AsLifetime(lifetime, ignoreSingletonImplementations: true);
        }

        /// <summary>
        ///     Returns a new <see cref="IServiceCollection"/> with all descriptors set to the specified
        ///     <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="lifetime">The target service lifetime.</param>
        /// <param name="ignoreSingletonImplementations">
        ///     When <see langword="true"/>, instance-based descriptors that can only be singletons are kept unchanged
        ///     instead of throwing.
        /// </param>
        public IServiceCollection AsLifetime(ServiceLifetime lifetime, bool ignoreSingletonImplementations)
        {
            var copy = new ServiceCollection();
            foreach (var descriptor in services)
            {
                copy.Add(descriptor.WithLifetime(lifetime, ignoreSingletonImplementations));
            }
            return copy;
        }

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
