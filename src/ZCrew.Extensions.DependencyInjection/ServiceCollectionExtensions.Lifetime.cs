using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ZCrew.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
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
    }
}
