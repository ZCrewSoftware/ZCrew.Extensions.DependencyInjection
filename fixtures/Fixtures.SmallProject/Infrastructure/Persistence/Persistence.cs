using Fixtures.SmallProject.Domain.Entities;
using Fixtures.SmallProject.Domain.Repositories;

namespace Fixtures.SmallProject.Infrastructure.Persistence;

public abstract class RepositoryBase<T> : IRepository<T>
{
    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}

public class InMemoryRepository<T> : RepositoryBase<T>, IRepository<T>;

public class SqlCustomerRepository : RepositoryBase<Customer>, ICustomerRepository;

public class SqlOrderRepository : RepositoryBase<Order>, IOrderRepository;

public class SqlRepository<T> : RepositoryBase<T>, IRepository<T>;
