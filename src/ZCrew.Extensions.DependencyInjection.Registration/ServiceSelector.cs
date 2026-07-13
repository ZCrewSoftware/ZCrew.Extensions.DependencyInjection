namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Maps each implementation type to one or more service types based on the chosen selection strategy (e.g. all
///     interfaces, default interfaces, self, base types). This is the service selection stage of the registration
///     fluent API, analogous to Castle Windsor's <c>WithService</c> methods. Maintains an immutable chain: each
///     select method returns a new instance. When the stage is skipped, each type is registered as itself.
/// </summary>
public class ServiceSelector : ServiceKeySelector
{
    private readonly IEnumerable<Type> types;
    private readonly IEnumerable<Type> baseTypes;

    // Single walk per terminal is verified by MultiEnumerationTests.
    // ReSharper disable PossibleMultipleEnumeration
    internal ServiceSelector(IEnumerable<Type> types, IEnumerable<Type> baseTypes)
        : base(types.Select(type => new ServiceComponent(type, [type])))
    {
        this.types = types;
        this.baseTypes = baseTypes;
    }
    // ReSharper restore PossibleMultipleEnumeration

    /// <summary>
    ///     Registers each type against service types returned by the specified <paramref name="serviceSelector"/> delegate.
    /// </summary>
    /// <param name="serviceSelector">
    ///     A function that receives the implementation type and returns the service types to register.
    /// </param>
    /// <example>
    ///     <code>
    ///     Classes.From(types).As(type => type.GetInterfaces()
    ///         .Where(i => i.Name.EndsWith("Service"))
    ///         .ToArray())
    ///     </code>
    /// </example>
    public ServiceKeySelector As(Func<Type, IEnumerable<Type>> serviceSelector)
    {
        ArgumentNullException.ThrowIfNull(serviceSelector);
        return new ServiceKeySelector(
            this.types.Select(type => new ServiceComponent(type, serviceSelector(type).ToArray()))
        );
    }

    /// <summary>
    ///     Registers each type against service types returned by the specified
    ///     <paramref name="serviceSelector"/> delegate, which also receives the resolved base types.
    /// </summary>
    /// <param name="serviceSelector">
    ///     A function that receives the implementation type and its resolved base types, and returns the service
    ///     types to register.
    /// </param>
    /// <example>
    ///     <code>
    ///     Classes.From(types)
    ///         .BasedOn&lt;IRepository&gt;()
    ///         .As((type, baseTypes) => baseTypes)
    ///     </code>
    /// </example>
    public ServiceKeySelector As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>> serviceSelector)
    {
        ArgumentNullException.ThrowIfNull(serviceSelector);
        return new ServiceKeySelector(
            this.types.Select(type =>
            {
                var assignableBaseTypes = type.GetMatchingBaseTypes(this.baseTypes).ToArray();
                var services = serviceSelector(type, assignableBaseTypes).ToArray();
                return new ServiceComponent(type, services);
            })
        );
    }
}
