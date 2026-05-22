using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Produces <see cref="ServiceDescriptor"/> registrations by mapping each implementation type to one or more
///     service types based on the chosen selection strategy (e.g. all interfaces, default interfaces, self, base
///     types). All generated descriptors use <see cref="ServiceLifetime.Singleton"/> by default.
/// </summary>
internal sealed class ServiceSelector : IServiceSelector
{
    private readonly IEnumerable<Type> types;
    private readonly IEnumerable<Type> baseTypes;

    internal ServiceSelector(IEnumerable<Type> types, IEnumerable<Type> baseTypes)
    {
        this.types = types;
        this.baseTypes = baseTypes;
    }

    /// <inheritdoc />
    public IKeyedServiceSelector As(Func<Type, IEnumerable<Type>> serviceSelector)
    {
        ArgumentNullException.ThrowIfNull(serviceSelector);
        return new KeyedServiceSelector(
            this.types.Select(type => new ServiceComponent(type, serviceSelector(type).ToArray()))
        );
    }

    /// <inheritdoc />
    public IKeyedServiceSelector As(Func<Type, IReadOnlyList<Type>, IEnumerable<Type>> serviceSelector)
    {
        ArgumentNullException.ThrowIfNull(serviceSelector);
        return new KeyedServiceSelector(
            this.types.Select(type =>
            {
                var assignableBaseTypes = type.GetMatchingBaseTypes(this.baseTypes).ToArray();
                var services = serviceSelector(type, assignableBaseTypes).ToArray();
                return new ServiceComponent(type, services);
            })
        );
    }
}
