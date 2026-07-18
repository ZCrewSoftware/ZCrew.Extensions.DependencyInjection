using System.Diagnostics;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Maps each implementation type to one or more service types based on the chosen selection strategy (e.g. all
///     interfaces, default interfaces, self, base types). This is the service selection stage of the registration
///     fluent API, analogous to Castle Windsor's <c>WithService</c> methods. Selection methods can be chained
///     (e.g. <c>AsSelf().AsAllInterfaces()</c>): each returns a new <see cref="ServiceSelector"/> and accumulates
///     the distinct service types, in-order.
/// </summary>
public class ServiceSelector : ServiceKeySelector
{
    private readonly IEnumerable<Service>? components;
    private readonly IEnumerable<Type>? types;
    private readonly IEnumerable<Type> baseTypes;

    // Single walk per terminal is verified by MultiEnumerationTests.
    // ReSharper disable PossibleMultipleEnumeration
    internal ServiceSelector(IEnumerable<Type> types, IEnumerable<Type> baseTypes)
        : base(types.Select(type => new Service(type, [type])))
    {
        this.types = types;
        this.baseTypes = baseTypes;
    }

    internal ServiceSelector(IEnumerable<Service> components, IEnumerable<Type> baseTypes)
        : base(components)
    {
        this.components = components;
        this.baseTypes = baseTypes;
    }
    // ReSharper restore PossibleMultipleEnumeration

    /// <summary>
    ///     Registers each type against the service types returned by the specified
    ///     <paramref name="serviceSelector"/> delegate, unioned with any service types already selected earlier in
    ///     the chain (duplicates are removed, preserving first-occurrence order).
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
    public ServiceSelector As(Func<Type, IEnumerable<Type>> serviceSelector)
    {
        ArgumentNullException.ThrowIfNull(serviceSelector);
        if (this.components != null)
        {
            return new ServiceSelector(
                this.components.Select(component =>
                    component.AsUnchecked(serviceSelector(component.ImplementationType).ToArray())
                ),
                this.baseTypes
            );
        }
        Debug.Assert(this.types != null);
        return new ServiceSelector(
            this.types.Select(type => new Service(type, serviceSelector(type).ToArray())),
            this.baseTypes
        );
    }

    /// <summary>
    ///     Registers each type against the service types returned by the specified
    ///     <paramref name="serviceSelector"/> delegate, which also receives the resolved base types, unioned with
    ///     any service types already selected earlier in the chain (duplicates are removed, preserving
    ///     first-occurrence order).
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
    public ServiceSelector As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>> serviceSelector)
    {
        ArgumentNullException.ThrowIfNull(serviceSelector);
        if (this.components != null)
        {
            return new ServiceSelector(
                this.components.Select(component =>
                {
                    var type = component.ImplementationType;
                    var assignableBaseTypes = type.GetMatchingBaseTypes(this.baseTypes).ToArray();
                    var services = serviceSelector(type, assignableBaseTypes).ToArray();
                    return component.AsUnchecked(services);
                }),
                this.baseTypes
            );
        }
        Debug.Assert(this.types != null);
        return new ServiceSelector(
            this.types.Select(type =>
            {
                var assignableBaseTypes = type.GetMatchingBaseTypes(this.baseTypes).ToArray();
                var services = serviceSelector(type, assignableBaseTypes).ToArray();
                return new Service(type, services);
            }),
            this.baseTypes
        );
    }
}
