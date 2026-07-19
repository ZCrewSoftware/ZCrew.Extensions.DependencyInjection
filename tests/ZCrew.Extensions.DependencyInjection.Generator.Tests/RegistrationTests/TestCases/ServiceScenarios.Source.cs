using Microsoft.Extensions.DependencyInjection;
using ZCrew.Extensions.DependencyInjection.Registration;

namespace Sample;

public interface IFoo;

public interface IBar;

public enum Region
{
    East,
    West,
}

[Service]
public class PlainService;

[Service(typeof(IFoo))]
public class FooService : IFoo;

[Service(typeof(IFoo), typeof(IBar))]
public class MultiShared : IFoo, IBar;

[Service(typeof(IFoo), Lifetime = ServiceLifetime.Scoped)]
public class ScopedFoo : IFoo;

[Service(typeof(IBar), Key = "primary")]
public class StringKeyed : IBar;

[Service(typeof(IBar), Key = Region.West)]
public class EnumKeyed : IBar;

[Service(typeof(IBar), Key = 5L)]
public class LongKeyed : IBar;

[Service(typeof(IFoo))]
[Service(typeof(IBar), Key = "second")]
public class MultiAttr : IFoo, IBar;

[Service(typeof(IFoo))]
public class OpenGeneric<T> : IFoo;

[Service]
public struct StructService;

public class Outer
{
    [Service]
    public class Nested;
}

[Service]
public abstract class AbstractService;

[Service]
public static class StaticService;
