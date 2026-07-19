using Microsoft.Extensions.DependencyInjection;

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
///     Empty service interface named by the <see cref="ContractAttribute"/> fixtures below, so a projector overload
///     can map an implementation onto it.
/// </summary>
public interface IProvidedServiceA;

/// <summary>
///     Default <see cref="AttributeUsageAttribute"/> — <c>Inherited = true</c> — carrying a <see cref="Type"/> array,
///     so it exercises the projector overloads (<c>AsServicesFromAttribute&lt;TAttribute&gt;</c> /
///     <c>AsServicesFromAttribute(Type, ...)</c>) and the inherited condition.
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
///     A projection attribute exposing a <see cref="ServiceLifetime"/>. Declared <c>Inherited = true</c> (the default)
///     so it also exercises the <c>inherited</c> flag on the projection overloads.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class LifestyleAttribute(ServiceLifetime lifetime) : Attribute, ILifestyleAware
{
    public ServiceLifetime Lifetime => lifetime;
}

// Plain, undecorated stores used by the per-type AsLifetime(Func) selector tests, which choose the lifetime from
// the type itself rather than from any attribute.
public class ScopedLifetimeStore;

public class TransientLifetimeStore;

// Lifestyle is inheritable, so it exercises the inherited flag on the projection overloads.
[Lifestyle(ServiceLifetime.Scoped)]
public class LifestyleBase;

public class LifestyleDerived : LifestyleBase;
