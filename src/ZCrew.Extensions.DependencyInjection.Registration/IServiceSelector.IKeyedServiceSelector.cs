namespace ZCrew.Extensions.DependencyInjection.Registration;

// Default interface implementations that bridge IServiceSelector to IKeyedServiceSelector.
// When a keyed service selector method is called on an IServiceSelector, the implementation
// first calls AsSelf() to accept all remaining types, then delegates to the corresponding IKeyedServiceSelector method.
public partial interface IServiceSelector
{
    IServiceSource IKeyedServiceSelector.Unkeyed()
    {
        return this.AsSelf().Unkeyed();
    }

    IServiceSource IKeyedServiceSelector.Keyed()
    {
        return this.AsSelf().Keyed();
    }

    IServiceSource IKeyedServiceSelector.Keyed(object? serviceKey)
    {
        return this.AsSelf().Keyed(serviceKey);
    }

    IServiceSource IKeyedServiceSelector.Keyed(Func<Type, object?> serviceKeySelector)
    {
        return this.AsSelf().Keyed(serviceKeySelector);
    }

    IServiceSource IKeyedServiceSelector.Keyed(Func<Type, Type, object?> serviceKeySelector)
    {
        return this.AsSelf().Keyed(serviceKeySelector);
    }
}
