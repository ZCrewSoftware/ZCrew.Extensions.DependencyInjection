using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection;

/// <summary>
///     Describes a service registration for a decorator including the decorator type, factory, and lifetime.
/// </summary>
/// <seealso cref="ServiceDescriptor" />
[DebuggerDisplay("{DebuggerToString(),nq}")]
internal class DecoratorServiceDescriptor
{
    /// <summary>
    ///     Initializes a new instance of <see cref="DecoratorServiceDescriptor" /> with the specified
    ///     <paramref name="decoratorType" />.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type" /> of the service.</param>
    /// <param name="decoratorType">The decorator <see cref="Type" /> implementing the service.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime" /> of the service.</param>
    public DecoratorServiceDescriptor(Type serviceType, Type decoratorType, ServiceLifetime? lifetime)
        : this(serviceType, null, decoratorType, lifetime) { }

    /// <summary>
    ///     Initializes a new instance of <see cref="ServiceDescriptor" /> with the specified
    ///     <paramref name="decoratorType" />.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type" /> of the service.</param>
    /// <param name="serviceKey">The <see cref="DecoratorServiceDescriptor.ServiceKey" /> of the service.</param>
    /// <param name="decoratorType">The decorator <see cref="Type" /> implementing the service.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime" /> of the service.</param>
    public DecoratorServiceDescriptor(
        Type serviceType,
        object? serviceKey,
        Type decoratorType,
        ServiceLifetime? lifetime
    )
        : this(serviceType, serviceKey, lifetime)
    {
        if (serviceKey == null)
        {
            DecoratorType = decoratorType;
        }
        else
        {
            KeyedDecoratorType = decoratorType;
        }
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="ServiceDescriptor" /> with the specified <paramref name="factory" />.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type" /> of the service.</param>
    /// <param name="factory">A factory used for creating service decorator instances.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime" /> of the service.</param>
    public DecoratorServiceDescriptor(
        Type serviceType,
        Func<IServiceProvider, object, object> factory,
        ServiceLifetime? lifetime
    )
        : this(serviceType, serviceKey: null, lifetime)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(factory);

        DecoratorFactory = factory;
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="ServiceDescriptor" /> with the specified <paramref name="factory" />.
    /// </summary>
    /// <param name="serviceType">The <see cref="Type" /> of the service.</param>
    /// <param name="serviceKey">The <see cref="DecoratorServiceDescriptor.ServiceKey" /> of the service.</param>
    /// <param name="factory">A factory used for creating service decorator instances.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime" /> of the service.</param>
    public DecoratorServiceDescriptor(
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object, object?, object> factory,
        ServiceLifetime? lifetime
    )
        : this(serviceType, serviceKey, lifetime)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(factory);

        if (serviceKey is null)
        {
            // If the key is null, use the same factory signature as non-keyed descriptor
            DecoratorFactory = (sp, service) => factory(sp, service, null);
        }
        else
        {
            KeyedDecoratorFactory = factory;
        }
    }

    private DecoratorServiceDescriptor(Type serviceType, object? serviceKey, ServiceLifetime? lifetime)
    {
        Lifetime = lifetime;
        ServiceType = serviceType;
        ServiceKey = serviceKey;
    }

    /// <summary>
    ///     The <see cref="ServiceLifetime" /> of the decorator. If this is <see langword="null" /> then the decorator
    ///     will inherit the lifetime of the delegate service.
    /// </summary>
    public ServiceLifetime? Lifetime { get; }

    /// <summary>
    ///     The service key for the decorator, if applicable.
    /// </summary>
    public object? ServiceKey { get; }

    /// <summary>
    ///     The service type.
    /// </summary>
    public Type ServiceType { get; }

    /// <summary>
    ///     Gets the <see cref="Type" /> that implements the service, or returns <see langword="null" /> if
    ///     <see cref="IsKeyedService" /> is <see langword="true" />.
    /// </summary>
    /// <remarks>
    ///     If <see cref="IsKeyedService" /> is <see langword="true" />, <see cref="KeyedDecoratorType" />
    ///     should be called instead.
    /// </remarks>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type? DecoratorType { get; }

    /// <summary>
    ///     Gets the <see cref="Type" /> that implements the service, or returns <see langword="null" /> if
    ///     <see cref="IsKeyedService" /> is <see langword="false" />.
    /// </summary>
    /// <remarks>
    ///     If <see cref="IsKeyedService" /> is <see langword="false" />, <see cref="DecoratorType" />
    ///     should be called instead.
    /// </remarks>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type? KeyedDecoratorType { get; }

    /// <summary>
    ///     Gets the factory that is used for creating decorator instances, or returns <see langword="null" /> if
    ///     <see cref="IsKeyedService" /> is <see langword="true" />.
    /// </summary>
    /// <remarks>
    ///     If <see cref="IsKeyedService" /> is <see langword="true" />, <see cref="KeyedDecoratorFactory" />
    ///     should be called instead.
    /// </remarks>
    public Func<IServiceProvider, object, object>? DecoratorFactory { get; }

    /// <summary>
    ///     Gets the factory that is used for creating decorator instances, or returns <see langword="null" /> if
    ///     <see cref="IsKeyedService" /> is <see langword="false" />.
    /// </summary>
    /// <remarks>
    ///     If <see cref="IsKeyedService" /> is <see langword="false" />, <see cref="KeyedDecoratorFactory" />
    ///     should be called instead.
    /// </remarks>
    public Func<IServiceProvider, object, object?, object>? KeyedDecoratorFactory { get; }

    /// <summary>
    ///     Indicates whether the decorator is a keyed service.
    /// </summary>
    public bool IsKeyedService => ServiceKey != null;

    /// <summary>
    ///     Creates a new <see cref="ServiceDescriptor" /> based on the delegate's service descriptor. This depends on
    ///     the delegate <see cref="ServiceDescriptor" /> to be modified by providing it with a unique
    ///     <see cref="ServiceDescriptor.ServiceKey" /> which should be passed as <paramref name="delegateServiceKey" />.
    /// </summary>
    /// <param name="delegateServiceKey">The unique key that the delegate service is registered with.</param>
    /// <param name="delegateLifetime">
    ///     When the <see cref="ServiceLifetime" /> is <see langword="null" /> then this lifetime will be used instead.
    /// </param>
    /// <returns>A new service descriptor representing the decorator service.</returns>
    public ServiceDescriptor ToServiceDescriptor(object? delegateServiceKey, ServiceLifetime delegateLifetime)
    {
        var lifetime = Lifetime ?? delegateLifetime;
        var implementationFactory = ToImplementationFactory(delegateServiceKey);
        return new ServiceDescriptor(ServiceType, ServiceKey, implementationFactory, lifetime);
    }

    private Func<IServiceProvider, object?, object> ToImplementationFactory(object? delegateServiceKey)
    {
        if (IsKeyedService)
        {
            return KeyedDecoratorFactory != null
                ? FromKeyedDecoratorFactory(delegateServiceKey)
                : FromKeyedDecoratorType(delegateServiceKey);
        }
        return DecoratorFactory != null
            ? FromDecoratorFactory(delegateServiceKey)
            : FromDecoratorType(delegateServiceKey);
    }

    private Func<IServiceProvider, object?, object> FromKeyedDecoratorFactory(object? delegateServiceKey)
    {
        return (sp, serviceKey) =>
        {
            var service = sp.GetRequiredKeyedService(ServiceType, delegateServiceKey);
            return KeyedDecoratorFactory!(sp, service, serviceKey);
        };
    }

    private Func<IServiceProvider, object?, object> FromKeyedDecoratorType(object? delegateServiceKey)
    {
        return (sp, _) =>
        {
            var service = sp.GetRequiredKeyedService(ServiceType, delegateServiceKey);
            return ActivatorUtilities.CreateInstance(sp, KeyedDecoratorType!, service);
        };
    }

    private Func<IServiceProvider, object?, object> FromDecoratorFactory(object? delegateServiceKey)
    {
        return (sp, _) =>
        {
            var service = sp.GetRequiredKeyedService(ServiceType, delegateServiceKey);
            return DecoratorFactory!(sp, service);
        };
    }

    private Func<IServiceProvider, object?, object> FromDecoratorType(object? delegateServiceKey)
    {
        return (sp, _) =>
        {
            var service = sp.GetRequiredKeyedService(ServiceType, delegateServiceKey);
            return ActivatorUtilities.CreateInstance(sp, DecoratorType!, service);
        };
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage(Justification = "This is copied from Microsoft.Extensions.DependencyInjection")]
    public override string ToString()
    {
        var toString = $"{nameof(ServiceType)}: {ServiceType} ";

        if (Lifetime != null)
        {
            toString += $"{nameof(Lifetime)}: {Lifetime} ";
        }

        if (IsKeyedService)
        {
            toString += $"{nameof(ServiceKey)}: {ServiceKey} ";

            if (KeyedDecoratorFactory != null)
            {
                return toString + $"{nameof(KeyedDecoratorFactory)}: {KeyedDecoratorFactory.Method}";
            }

            return toString + $"{nameof(KeyedDecoratorType)}: {KeyedDecoratorType}";
        }
        if (DecoratorFactory != null)
        {
            return toString + $"{nameof(DecoratorFactory)}: {DecoratorFactory.Method}";
        }

        return toString + $"{nameof(DecoratorType)}: {DecoratorType}";
    }

    [ExcludeFromCodeCoverage(Justification = "This is copied from Microsoft.Extensions.DependencyInjection")]
    internal string ToServiceString()
    {
        var toString = $"{nameof(ServiceType)}: {ServiceType}";

        if (Lifetime != null)
        {
            toString += $" {nameof(Lifetime)}: {Lifetime}";
        }

        if (IsKeyedService)
        {
            toString += $" {nameof(ServiceKey)}: {ServiceKey}";
        }

        return toString;
    }

    [ExcludeFromCodeCoverage(Justification = "This is copied from Microsoft.Extensions.DependencyInjection")]
    private string DebuggerToString()
    {
        var debugText = $@"{nameof(ServiceType)} = ""{ServiceType.FullName}""";

        if (Lifetime != null)
        {
            debugText += $", {nameof(Lifetime)} = {Lifetime}";
        }

        if (IsKeyedService)
        {
            debugText += $@", ServiceKey = ""{ServiceKey}""";

            if (KeyedDecoratorFactory != null)
            {
                debugText += $", {nameof(KeyedDecoratorFactory)} = {KeyedDecoratorFactory.Method}";
            }

            debugText += $@", {nameof(KeyedDecoratorType)} = ""{KeyedDecoratorType!.FullName}""";
        }
        else
        {
            if (DecoratorFactory != null)
            {
                debugText += $", {nameof(DecoratorFactory)} = {DecoratorFactory.Method}";
            }

            debugText += $@", {nameof(DecoratorType)} = ""{DecoratorType!.FullName}""";
        }

        return debugText;
    }
}
