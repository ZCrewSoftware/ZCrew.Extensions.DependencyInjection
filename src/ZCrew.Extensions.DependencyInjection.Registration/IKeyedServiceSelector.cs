namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Defines methods for assigning service keys to registrations produced by the service selection stage. This is
///     an optional stage between <see cref="IServiceSelector"/> and <see cref="IServiceSource"/> in the registration
///     fluent API.
/// </summary>
public interface IKeyedServiceSelector : IServiceSource
{
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
    IServiceSource Keyed();

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
    IServiceSource Keyed(object? serviceKey);

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
    IServiceSource Keyed(Func<Type, object?> serviceKeySelector);

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
    IServiceSource Keyed(Func<Type, Type, object?> serviceKeySelector);
}
