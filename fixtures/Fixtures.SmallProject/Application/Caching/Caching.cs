namespace Fixtures.SmallProject.Application.Caching;

public interface ICacheProvider : IDisposable;

public interface ICacheProvider<T> : IAsyncDisposable;
