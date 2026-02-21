using Fixtures.SmallProject.Domain.Entities;

namespace Fixtures.SmallProject.Domain.Repositories;

public interface IReadOnlyRepository<T> : IDisposable, IAsyncDisposable;

public interface IRepository<T> : IReadOnlyRepository<T>;

public interface IOrderRepository : IRepository<Order>;

public interface ICustomerRepository : IRepository<Customer>;

public interface IProductRepository : IRepository<Product>;
