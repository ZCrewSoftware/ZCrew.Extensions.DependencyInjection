using Fixtures.SmallProject.Application.Caching;

namespace Fixtures.SmallProject.Application.Services;

public interface ICustomerService;

public interface IOrderService;

public interface IProductService;

public interface IAuditService
{
    Stack<InstanceData> GetInstanceData();
}

public class CustomerService : ICustomerService;

public class OrderService : IOrderService;

public class ProductService : IProductService;

public class AuditService : IAuditService
{
    private readonly Guid instanceId = Guid.NewGuid();

    public Stack<InstanceData> GetInstanceData()
    {
        var data = new Stack<InstanceData>();
        data.Push(new InstanceData(this.instanceId, GetType()));
        return data;
    }
}

public class AuditServiceDecorator(IAuditService inner) : IAuditService
{
    private readonly Guid instanceId = Guid.NewGuid();

    public Stack<InstanceData> GetInstanceData()
    {
        var data = inner.GetInstanceData();
        data.Push(new InstanceData(this.instanceId, GetType()));
        return data;
    }
}

public class LegacyOrderProcessor : IOrderService;

public class CachingCustomerService(ICustomerService inner, ICacheProvider cache) : ICustomerService;
