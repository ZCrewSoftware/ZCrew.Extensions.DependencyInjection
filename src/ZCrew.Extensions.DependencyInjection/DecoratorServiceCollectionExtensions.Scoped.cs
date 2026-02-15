using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection;

public static partial class DecoratorServiceCollectionExtensions
{
    /// <param name="services">The <see cref="IServiceCollection" /> to add the service to.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds a scoped decorator of the type specified in <typeparamref name="TService" /> with an implementation
        ///     type specified in <typeparamref name="TDecorator" /> to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TDecorator">The type of the decorator to use.</typeparam>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped" />
        /// <exception cref="InvalidOperationException">
        ///     If no services were registered for the type <typeparamref name="TService" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If any service registered for type <typeparamref name="TService" /> is registered as a
        ///     <see cref="ServiceLifetime.Transient" />.
        /// </exception>
        public IServiceCollection AddScopedDecorator<
            TService,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TDecorator
        >()
            where TService : class
            where TDecorator : class
        {
            ArgumentNullException.ThrowIfNull(services);
            services.AddDecorator(
                new DecoratorServiceDescriptor(typeof(TService), typeof(TDecorator), ServiceLifetime.Scoped)
            );
            return services;
        }

        /// <summary>
        ///     Adds a scoped decorator of the type specified in <paramref name="serviceType" /> with an implementation
        ///     type specified in <paramref name="decoratorType" /> to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to add.</param>
        /// <param name="decoratorType">The type of the decorator to use.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped" />
        /// <exception cref="InvalidOperationException">
        ///     If no services were registered for the type <paramref name="serviceType" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If any service registered for type <paramref name="serviceType" /> is registered as a
        ///     <see cref="ServiceLifetime.Transient" />.
        /// </exception>
        public IServiceCollection AddScopedDecorator(
            Type serviceType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type decoratorType
        )
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(decoratorType);
            services.AddDecorator(new DecoratorServiceDescriptor(serviceType, decoratorType, ServiceLifetime.Scoped));
            return services;
        }

        /// <summary>
        ///     Adds a scoped decorator of the type specified in <typeparamref name="TService" /> with an implementation
        ///     type specified in <paramref name="factory" /> to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <typeparamref name="TService">The type of the service to add.</typeparamref>
        /// <param name="factory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped" />
        /// <exception cref="InvalidOperationException">
        ///     If no services were registered for the type <typeparamref name="TService" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If any service registered for type <typeparamref name="TService" /> is registered as a
        ///     <see cref="ServiceLifetime.Transient" />.
        /// </exception>
        /// <example>
        ///     The delegate service is provided to the factory type and must be passed to the constructor of the
        ///     implementation type:
        ///     <code>
        ///     services.AddScopedDecorator&lt;IService&gt;((IServiceProvider _, IService service) =>
        ///     {
        ///         return new DecoratorService(service);
        ///     });
        ///     </code>
        /// </example>
        public IServiceCollection AddScopedDecorator<TService>(Func<IServiceProvider, TService, TService> factory)
            where TService : class
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(factory);
            Func<IServiceProvider, object, object> untypedFactory = (sp, s) => factory(sp, (TService)s);
            services.AddDecorator(
                new DecoratorServiceDescriptor(typeof(TService), untypedFactory, ServiceLifetime.Scoped)
            );
            return services;
        }

        /// <summary>
        ///     Adds a scoped decorator of the type specified in <paramref name="serviceType" /> with an implementation
        ///     type specified in <paramref name="factory" /> to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to add.</param>
        /// <param name="factory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped" />
        /// <exception cref="InvalidOperationException">
        ///     If no services were registered for the type <paramref name="serviceType" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If any service registered for type <paramref name="serviceType" /> is registered as a
        ///     <see cref="ServiceLifetime.Transient" />.
        /// </exception>
        /// <example>
        ///     The delegate service is provided to the factory type and must be passed to the constructor of the
        ///     implementation type:
        ///     <code>
        ///     services.AddScopedDecorator(typeof(IService), (IServiceProvider _, object service) =>
        ///     {
        ///         return new DecoratorService((IService)service);
        ///     });
        ///     </code>
        /// </example>
        public IServiceCollection AddScopedDecorator(Type serviceType, Func<IServiceProvider, object, object> factory)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(factory);
            services.AddDecorator(new DecoratorServiceDescriptor(serviceType, factory, ServiceLifetime.Scoped));
            return services;
        }

        /// <summary>
        ///     Adds a scoped decorator of the type specified in <typeparamref name="TService" /> with an implementation
        ///     type specified in <typeparamref name="TDecorator" /> to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TDecorator">The type of the decorator to use.</typeparam>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey" /> of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped" />
        /// <exception cref="InvalidOperationException">
        ///     If no services were registered for the type <typeparamref name="TService" /> with the same
        ///     <see name="ServiceDescriptor.ServiceKey" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If any service registered for type <typeparamref name="TService" /> with the same
        ///     <see name="ServiceDescriptor.ServiceKey" /> is registered as a <see cref="ServiceLifetime.Transient" />.
        /// </exception>
        public IServiceCollection AddKeyedScopedDecorator<
            TService,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TDecorator
        >(object? serviceKey)
            where TService : class
            where TDecorator : class
        {
            ArgumentNullException.ThrowIfNull(services);
            services.AddDecorator(
                new DecoratorServiceDescriptor(typeof(TService), serviceKey, typeof(TDecorator), ServiceLifetime.Scoped)
            );
            return services;
        }

        /// <summary>
        ///     Adds a scoped decorator of the type specified in <paramref name="serviceType" /> with an implementation
        ///     type specified in <paramref name="decoratorType" /> to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to add.</param>
        /// <param name="decoratorType">The type of the decorator to use.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey" /> of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped" />
        /// <exception cref="InvalidOperationException">
        ///     If no services were registered for the type <paramref name="serviceType" /> with the same
        ///     <see name="ServiceDescriptor.ServiceKey" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If any service registered for type <paramref name="serviceType" /> with the same
        ///     <see name="ServiceDescriptor.ServiceKey" /> is registered as a <see cref="ServiceLifetime.Transient" />.
        /// </exception>
        public IServiceCollection AddKeyedScopedDecorator(
            Type serviceType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type decoratorType,
            object? serviceKey
        )
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(decoratorType);
            services.AddDecorator(
                new DecoratorServiceDescriptor(serviceType, serviceKey, decoratorType, ServiceLifetime.Scoped)
            );
            return services;
        }

        /// <summary>
        ///     Adds a scoped decorator of the type specified in <typeparamref name="TService" /> with an implementation
        ///     type specified in <paramref name="factory" /> to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <typeparamref name="TService">The type of the service to add.</typeparamref>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey" /> of the service.</param>
        /// <param name="factory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped" />
        /// <exception cref="InvalidOperationException">
        ///     If no services were registered for the type <typeparamref name="TService" /> with the same
        ///     <see name="ServiceDescriptor.ServiceKey" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If any service registered for type <typeparamref name="TService" /> with the same
        ///     <see name="ServiceDescriptor.ServiceKey" /> is registered as a <see cref="ServiceLifetime.Transient" />.
        /// </exception>
        /// <example>
        ///     The delegate service and service key is provided to the factory type and must be passed to the
        ///     constructor of the implementation type:
        ///     <code>
        ///     services.AddKeyedScopedDecorator&lt;IService&gt;(
        ///         "service-key",
        ///         (IServiceProvider _, IService service, object? serviceKey) =>
        ///         {
        ///             return new DecoratorService(service);
        ///         });
        ///     </code>
        /// </example>
        public IServiceCollection AddKeyedScopedDecorator<TService>(
            object? serviceKey,
            Func<IServiceProvider, TService, object?, TService> factory
        )
            where TService : class
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(factory);
            Func<IServiceProvider, object, object?, object> untypedFactory = (sp, s, sk) =>
                factory(sp, (TService)s, sk);
            services.AddDecorator(
                new DecoratorServiceDescriptor(typeof(TService), serviceKey, untypedFactory, ServiceLifetime.Scoped)
            );
            return services;
        }

        /// <summary>
        ///     Adds a scoped decorator of the type specified in <paramref name="serviceType" /> with an implementation
        ///     type specified in <paramref name="factory" /> to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="serviceType">The type of the service to add.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey" /> of the service.</param>
        /// <param name="factory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped" />
        /// <exception cref="InvalidOperationException">
        ///     If no services were registered for the type <paramref name="serviceType" /> with the same
        ///     <see name="ServiceDescriptor.ServiceKey" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If any service registered for type <paramref name="serviceType" /> with the same
        ///     <see name="ServiceDescriptor.ServiceKey" /> is registered as a <see cref="ServiceLifetime.Transient" />.
        /// </exception>
        /// <example>
        ///     The delegate service and service key is provided to the factory type and must be passed to the
        ///     constructor of the implementation type:
        ///     <code>
        ///     services.AddKeyedScopedDecorator(
        ///         typeof(IService),
        ///         "service-key",
        ///         (IServiceProvider _, object service, object? serviceKey) =>
        ///         {
        ///             return new DecoratorService((IService)service);
        ///         });
        ///     </code>
        /// </example>
        public IServiceCollection AddKeyedScopedDecorator(
            Type serviceType,
            object? serviceKey,
            Func<IServiceProvider, object, object?, object> factory
        )
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(factory);
            services.AddDecorator(
                new DecoratorServiceDescriptor(serviceType, serviceKey, factory, ServiceLifetime.Scoped)
            );
            return services;
        }
    }
}
