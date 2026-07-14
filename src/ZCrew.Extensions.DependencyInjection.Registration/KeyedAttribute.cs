namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Marks a service as a keyed service. This can be detected automatically with
///     <see cref="ServiceKeySelectorExtensions.KeyedByAttribute(ServiceKeySelector)"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class KeyedAttribute : Attribute, IServiceKeyProvider
{
    /// <summary>
    /// Initializes a new <see cref="KeyedAttribute"/> for specifying the key for this keyed service.
    /// </summary>
    /// <param name="serviceKey"></param>
    public KeyedAttribute(object? serviceKey)
    {
        ServiceKey = serviceKey;
    }

    /// <inheritdoc />
    public object? ServiceKey { get; }
}
