using Fixtures.SmallProject.Application.Caching;

namespace Fixtures.SmallProject.Infrastructure.Caching;

public class DistributedCacheProvider : ICacheProvider
{
    public void Dispose() { }
}

public class InMemoryCacheProvider : ICacheProvider
{
    public void Dispose() { }
}

public class InMemoryCacheProvider<T> : ICacheProvider<T>
{
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
