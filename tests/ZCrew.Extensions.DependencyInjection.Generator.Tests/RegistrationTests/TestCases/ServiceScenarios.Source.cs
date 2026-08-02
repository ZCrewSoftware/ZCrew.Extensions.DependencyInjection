using ZCrew.Extensions.DependencyInjection.Registration;

namespace Sample;

public interface IFoo;

public interface IBar;

public interface IRepository<T>;

public enum Region
{
    East,
    West,
}

[Service]
public class PlainService;

[Service]
[As<IFoo>]
public class FooService : IFoo;

[Service, As<IFoo>, As<IBar>]
public class MultiShared : IFoo, IBar;

[Service, Scoped]
[As<IFoo>]
public class ScopedFoo : IFoo;

[Service, Keyed("primary")]
public class StringKeyed : IBar;

[Service, Keyed(Region.West)]
public class EnumKeyed : IBar;

[Service, Keyed(5L)]
public class LongKeyed : IBar;

[Service]
[As<IBar>("smtp"), As<IBar>("ses")]
public class MultiKeyed : IBar;

[Service]
[As<IFoo>("Database"), As<IBar>]
public class MixedKeyed : IFoo, IBar;

[Service, As(typeof(IRepository<>))]
public class OpenGeneric<T> : IRepository<T>;

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
