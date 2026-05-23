namespace Fixtures.SmallProject.Application.Caching;

public interface ICacheProvider : IDisposable;

public interface ICacheProvider<T> : IAsyncDisposable;

[AttributeUsage(AttributeTargets.Class)]
public class CacheableAttribute(string region) : Attribute
{
    public string Region => region;
}
