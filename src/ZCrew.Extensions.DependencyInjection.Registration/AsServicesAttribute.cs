namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Declares the service types an implementation is registered against. This can be detected automatically with
///     <see cref="ServiceSelectorExtensions.AsServicesFromAttribute(ServiceSelector)"/>.
/// </summary>
/// <remarks>
///     The <c>As</c> prefix mirrors the fluent <c>AsServicesFromAttribute</c> step this attribute feeds and keeps it
///     distinct from the source generator's <c>ServiceAttribute</c>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class AsServicesAttribute : Attribute, IServiceTypesProvider
{
    /// <summary>
    ///     Initializes a new <see cref="AsServicesAttribute"/> for specifying the service types this implementation is
    ///     registered against.
    /// </summary>
    /// <param name="serviceTypes">The service types to register the implementation against.</param>
    public AsServicesAttribute(params Type[] serviceTypes)
    {
        ServiceTypes = serviceTypes ?? [];
    }

    /// <inheritdoc />
    public IEnumerable<Type> ServiceTypes { get; }
}
