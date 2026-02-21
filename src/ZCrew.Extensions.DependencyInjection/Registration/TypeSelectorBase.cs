using Microsoft.Extensions.DependencyInjection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public abstract class TypeSelectorBase : ServiceSource, ITypeSelector
{
    public abstract IEnumerable<Type> SelectTypes();

    protected override IEnumerable<ServiceDescriptor> SelectServices()
    {
        return SelectTypes().Select(type => new ServiceDescriptor(type, type, ServiceLifetime.Singleton));
    }
}
