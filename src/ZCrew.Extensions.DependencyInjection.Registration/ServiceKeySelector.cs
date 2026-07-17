namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Assigns service keys to registrations produced by the service selection stage. This is an optional stage
///     between <see cref="ServiceSelector"/> and <see cref="ServiceLifetimeSelector"/> in the registration fluent API.
///     When the stage is skipped, the registrations pass through unkeyed.
/// </summary>
public class ServiceKeySelector : ServiceLifetimeSelector
{
    private readonly IEnumerable<ServiceComponent> components;

    // Single walk per terminal is verified by MultiEnumerationTests.
    // ReSharper disable PossibleMultipleEnumeration
    internal ServiceKeySelector(IEnumerable<ServiceComponent> components)
        : base(components)
    {
        this.components = components;
    }
    // ReSharper restore PossibleMultipleEnumeration

    /// <summary>
    ///     Explicitly avoid assigning a service key to each registration.
    /// </summary>
    public ServiceLifetimeSelector Unkeyed()
    {
        return new ServiceLifetimeSelector(this.components);
    }

    /// <summary>
    ///     Assigns a service key to each registration by convention: the implementation type name with the service
    ///     type's interface name stripped. For example, <c>PayPalPaymentGateway</c> registered as
    ///     <c>IPaymentGateway</c> yields key <c>"PayPal"</c>. Registrations where the auto-detected key would be
    ///     empty are left unkeyed.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Classes.From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
    ///         .AsInterface&lt;IPaymentGateway&gt;()
    ///         .Keyed()
    ///     // PayPalPaymentGateway keyed as "PayPal"
    ///     // StripePaymentGateway keyed as "Stripe"
    ///     </code>
    /// </example>
    public ServiceLifetimeSelector Keyed()
    {
        return Keyed(
            (implementationType, serviceType) =>
            {
                var implementationName = StripGenericArity(implementationType.Name);
                var serviceName = StripGenericArity(serviceType.GetInterfaceName());

                // The implementation and service may be the same type, so ensure there is a prefix differentiating them
                if (implementationName.EndsWith(serviceName) && implementationName.Length > serviceName.Length)
                {
                    var serviceKeyString = new string(implementationName[..^serviceName.Length]);
                    return serviceKeyString;
                }

                // Implementation name did not end with service name, no service key can be extracted automatically
                return null;
            }
        );
    }

    /// <summary>
    ///     Assigns the specified <paramref name="serviceKey"/> to all registrations. When
    ///     <paramref name="serviceKey"/> is <see langword="null"/>, the registrations are returned unchanged.
    /// </summary>
    /// <param name="serviceKey">
    ///     The service key to assign, or <see langword="null"/> to leave registrations unkeyed.
    /// </param>
    /// <example>
    ///     <code>
    ///     Classes.From(typeof(PayPalPaymentGateway))
    ///         .AsInterface&lt;IPaymentGateway&gt;()
    ///         .Keyed("myKey")
    ///     </code>
    /// </example>
    public ServiceLifetimeSelector Keyed(object? serviceKey)
    {
        // Just skip the scan entirely
        if (serviceKey == null)
        {
            return Unkeyed();
        }

        return new ServiceLifetimeSelector(this.components.Select(component => component.Keyed(serviceKey)));
    }

    /// <summary>
    ///     Assigns a service key to each registration using a function that receives the implementation type. When
    ///     the function returns <see langword="null"/> for a descriptor, that descriptor is left unkeyed.
    /// </summary>
    /// <param name="serviceKeySelector">
    ///     A function that receives the implementation type and returns the service key.
    /// </param>
    /// <example>
    ///     <code>
    ///     Classes.From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
    ///         .AsInterface&lt;IPaymentGateway&gt;()
    ///         .Keyed(type => type.Name)
    ///     </code>
    /// </example>
    public ServiceLifetimeSelector Keyed(Func<Type, object?> serviceKeySelector)
    {
        return Keyed((implementationType, _) => serviceKeySelector(implementationType));
    }

    /// <summary>
    ///     Assigns a service key to each registration using a function that receives both the implementation type
    ///     and the service type. When the function returns <see langword="null"/> for a descriptor, that descriptor
    ///     is left unkeyed.
    /// </summary>
    /// <param name="serviceKeySelector">
    ///     A function that receives the implementation type and service type and returns the service key.
    /// </param>
    /// <example>
    ///     <code>
    ///     Classes.From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
    ///         .AsInterface&lt;IPaymentGateway&gt;()
    ///         .Keyed((impl, svc) => $"{impl.Name}:{svc.Name}")
    ///     </code>
    /// </example>
    public ServiceLifetimeSelector Keyed(Func<Type, Type, object?> serviceKeySelector)
    {
        ArgumentNullException.ThrowIfNull(serviceKeySelector);
        return new ServiceLifetimeSelector(this.components.Select(component => component.Keyed(serviceKeySelector)));
    }

    private static ReadOnlySpan<char> StripGenericArity(ReadOnlySpan<char> name)
    {
        var backtick = name.IndexOf('`');
        return backtick >= 0 ? name[..backtick] : name;
    }
}
