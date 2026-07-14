namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Declares the service types an implementation is registered against. This can be detected automatically with
///     <see cref="ServiceSelectorExtensions.AsServicesFromAttribute(ServiceSelector)"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class ServicesAttribute : Attribute, IServiceTypesProvider
{
    /// <summary>
    ///     Initializes a new <see cref="ServicesAttribute"/> for specifying the service types this implementation is
    ///     registered against.
    /// </summary>
    /// <param name="serviceTypes">The service types to register the implementation against.</param>
    public ServicesAttribute(params Type[] serviceTypes)
    {
        ServiceTypes = serviceTypes ?? [];
    }

    /// <inheritdoc />
    public IEnumerable<Type> ServiceTypes { get; }
}
