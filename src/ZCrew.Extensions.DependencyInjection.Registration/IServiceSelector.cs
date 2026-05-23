namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Defines methods for selecting which service types each implementation type should be registered as. This is
///     the service selection stage of the registration fluent API, analogous to Castle Windsor's <c>WithService</c>
///     methods. Maintains an immutable chain: each select method returns a new instance.
/// </summary>
public interface IServiceSelector : IKeyedServiceSelector
{
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
    IKeyedServiceSelector As(Func<Type, IEnumerable<Type>> serviceSelector);

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
    IKeyedServiceSelector As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>> serviceSelector);
}
