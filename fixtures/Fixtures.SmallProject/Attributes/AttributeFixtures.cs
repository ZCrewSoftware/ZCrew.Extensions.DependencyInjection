using Microsoft.Extensions.DependencyInjection;
using ZCrew.Extensions.DependencyInjection.Registration;

namespace Fixtures.SmallProject.Attributes;

/// <summary>
///     Marker interface implemented by several attributes. Exercises attribute filtering by an interface the
///     attribute implements (rather than by the concrete attribute type), including reading a property the
///     interface defines.
/// </summary>
public interface IRegionAware
{
    string Region { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class RegionCacheAttribute(string region) : Attribute, IRegionAware
{
    public string Region => region;
}

[AttributeUsage(AttributeTargets.Class)]
public class RegionPartitionAttribute(string region) : Attribute, IRegionAware
{
    public string Region => region;
}

[RegionCache("customers")]
public class RegionalCustomerStore;

[RegionCache("orders")]
public class RegionalOrderStore;

[RegionPartition("payments")]
public class PartitionedPaymentStore;

public class UnmarkedStore;

/// <summary>
///     Default <see cref="AttributeUsageAttribute"/> — <c>Inherited = true</c> — so derived types match when requested.
///     Carries data so it also exercises the inherited condition overloads.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TracedAttribute(string channel) : Attribute
{
    public string Channel => channel;
}

[Traced("audit")]
public class TracedBase;

public class TracedDerived : TracedBase;

/// <summary><c>Inherited = false</c> — derived types never match, even when inherited attributes are requested.</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class LocalOnlyAttribute : Attribute;

[LocalOnly]
public class LocalOnlyBase;

public class LocalOnlyDerived : LocalOnlyBase;

/// <summary><c>AllowMultiple = true</c> — a type may carry several instances, exercising the collection conditions.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class TagAttribute(string name) : Attribute
{
    public string Name => name;
}

[Tag("a")]
[Tag("b")]
public class MultiTagged;

[Tag("only")]
public class SingleTagged;

[Tag("base1")]
[Tag("base2")]
public class TaggedBase;

public class TaggedDerived : TaggedBase;

/// <summary>Applicable to every type kind, so it can decorate an interface, struct, and enum for type-kind coverage.</summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Class)]
public class MetaAttribute : Attribute;

[Meta]
public interface IMarkedContract;

[Meta]
public struct MarkedValue;

[Meta]
public enum MarkedEnum
{
    None,
}

[Meta]
public class MarkedClass;

/// <summary>
///     A second <see cref="IServiceKeyProvider"/> attribute. The shipped <see cref="KeyedAttribute"/> is
///     <c>AllowMultiple = false</c>, so a distinct provider is needed to give a single type two service-key
///     attributes (the ambiguous-match path). Declared <c>Inherited = true</c> (the default) so it also
///     exercises the <c>inherited</c> flag on the <see cref="IServiceKeyProvider"/> path.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AltKeyedAttribute(object? key) : Attribute, IServiceKeyProvider
{
    public object? ServiceKey => key;
}

[Keyed("customers")]
public class KeyProvidedStore;

[Keyed(null)]
public class NullKeyProvidedStore;

[Keyed("first")]
[AltKeyed("second")]
public class MultiKeyProvidedStore;

// [Keyed] is declared Inherited = false, so its key does not flow to KeyedDerived.
[Keyed("base-key")]
public class KeyedBase;

public class KeyedDerived : KeyedBase;

// AltKeyed is inheritable, so it exercises the inherited flag on the IServiceKeyProvider path.
[AltKeyed("alt-base")]
public class InheritableKeyedBase;

public class InheritableKeyedDerived : InheritableKeyedBase;

/// <summary>
///     Empty service interfaces targeted by the <see cref="AsServicesAttribute"/> fixtures below. Two are provided so a
///     single implementation can be mapped to multiple service types (exercising the shared-service path).
/// </summary>
public interface IProvidedServiceA;

public interface IProvidedServiceB;

/// <summary>
///     A second <see cref="IServiceTypesProvider"/> attribute. The shipped <see cref="AsServicesAttribute"/> is
///     <c>AllowMultiple = false</c>, so a distinct provider is needed to give a single type two service-type
///     attributes (the ambiguous-match path). Declared <c>Inherited = true</c> (the default) so it also exercises the
///     <c>inherited</c> flag on the <see cref="IServiceTypesProvider"/> path.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AltServicesAttribute(params Type[] serviceTypes) : Attribute, IServiceTypesProvider
{
    public IEnumerable<Type> ServiceTypes => serviceTypes;
}

[AsServices(typeof(IProvidedServiceA))]
public class SingleServiceStore : IProvidedServiceA;

[AsServices(typeof(IProvidedServiceA), typeof(IProvidedServiceB))]
public class MultiServiceStore : IProvidedServiceA, IProvidedServiceB;

// [AsServices] is declared Inherited = false, so its service types do not flow to ServicesDerived.
[AsServices(typeof(IProvidedServiceA))]
public class ServicesBase : IProvidedServiceA;

public class ServicesDerived : ServicesBase;

// AltServices is inheritable, so it exercises the inherited flag on the IServiceTypesProvider path.
[AltServices(typeof(IProvidedServiceB))]
public class InheritableServicesBase : IProvidedServiceB;

public class InheritableServicesDerived : InheritableServicesBase;

// Two IServiceTypesProvider attributes on one type -> AmbiguousMatchException.
[AsServices(typeof(IProvidedServiceA))]
[AltServices(typeof(IProvidedServiceB))]
public class MultiServiceProvidedStore;

/// <summary>
///     Default <see cref="AttributeUsageAttribute"/> — <c>Inherited = true</c> — carrying a <see cref="Type"/> array
///     but deliberately not implementing <see cref="IServiceTypesProvider"/>, so it exercises the projector overloads
///     (<c>AsServicesFromAttribute&lt;TAttribute&gt;</c> / <c>AsServicesFromAttribute(Type, ...)</c>) and the
///     inherited condition.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ContractAttribute(params Type[] contracts) : Attribute
{
    public Type[] Contracts => contracts;
}

[Contract(typeof(IProvidedServiceA))]
public class ContractBase;

public class ContractDerived : ContractBase;

// Declares a contract it also implements. ContractBase deliberately does not, which makes it the case where a
// service rejects an attribute-named service type it isn't based on.
[Contract(typeof(IProvidedServiceA))]
public class ContractStore : IProvidedServiceA;

public class ContractStoreDerived : ContractStore;

/// <summary>
///     Marker interface implemented by attributes that carry a <see cref="ServiceLifetime"/>. Exercises attribute
///     filtering by an interface the attribute implements (rather than by the concrete attribute type), including
///     reading a property the interface defines.
/// </summary>
public interface ILifestyleAware
{
    ServiceLifetime Lifetime { get; }
}

/// <summary>
///     A projection attribute exposing a <see cref="ServiceLifetime"/>, unrelated to
///     <see cref="IServiceLifetimeProvider"/>. Declared <c>Inherited = true</c> (the default) so it also exercises
///     the <c>inherited</c> flag on the projection overloads.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class LifestyleAttribute(ServiceLifetime lifetime) : Attribute, ILifestyleAware
{
    public ServiceLifetime Lifetime => lifetime;
}

/// <summary>
///     A second <see cref="IServiceLifetimeProvider"/> attribute. The shipped <see cref="AsLifetimeAttribute"/> is
///     <c>AllowMultiple = false</c>, so a distinct provider is needed to give a single type two lifetime attributes
///     (the ambiguous-match path). Declared <c>Inherited = true</c> (the default) so it also exercises the
///     <c>inherited</c> flag on the <see cref="IServiceLifetimeProvider"/> path.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AltLifetimeAttribute(ServiceLifetime lifetime) : Attribute, IServiceLifetimeProvider
{
    public ServiceLifetime Lifetime => lifetime;
}

public interface ILifetimeAlpha;

public interface ILifetimeBeta;

[AsLifetime(ServiceLifetime.Scoped)]
public class ScopedLifetimeStore;

[AsLifetime(ServiceLifetime.Transient)]
public class TransientLifetimeStore;

[AsLifetime(ServiceLifetime.Singleton)]
public class SingletonLifetimeStore;

[AsLifetime(ServiceLifetime.Scoped)]
[AltLifetime(ServiceLifetime.Transient)]
public class MultiLifetimeStore;

// [AsLifetime] is declared Inherited = false, so its lifetime does not flow to LifetimeDerived.
[AsLifetime(ServiceLifetime.Scoped)]
public class LifetimeBase;

public class LifetimeDerived : LifetimeBase;

// AltLifetime is inheritable, so it exercises the inherited flag on the IServiceLifetimeProvider path.
[AltLifetime(ServiceLifetime.Scoped)]
public class InheritableLifetimeBase;

public class InheritableLifetimeDerived : InheritableLifetimeBase;

// Lifestyle is inheritable, so it exercises the inherited flag on the projection overloads.
[Lifestyle(ServiceLifetime.Scoped)]
public class LifestyleBase;

public class LifestyleDerived : LifestyleBase;

// Multi-interface implementations that declare their own lifetime, for verifying that Singleton services still
// share one instance while Transient services are registered independently.
[AsLifetime(ServiceLifetime.Singleton)]
public class SingletonMultiStore : ILifetimeAlpha, ILifetimeBeta;

[AsLifetime(ServiceLifetime.Transient)]
public class TransientMultiStore : ILifetimeAlpha, ILifetimeBeta;
