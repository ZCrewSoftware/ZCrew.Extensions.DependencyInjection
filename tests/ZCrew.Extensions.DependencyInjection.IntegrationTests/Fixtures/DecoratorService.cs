namespace ZCrew.Extensions.DependencyInjection.IntegrationTests.Fixtures;

internal class DecoratorService : IService
{
    private readonly Guid instanceId;

    private readonly IService next;

    public DecoratorService(IService next)
        : this(next, Guid.NewGuid()) { }

    public DecoratorService(IService next, Guid instanceId)
    {
        this.next = next;
        this.instanceId = instanceId;
    }

    public Stack<InstanceData> GetInstanceData()
    {
        var data = this.next.GetInstanceData();
        data.Push(new InstanceData(this.instanceId, GetType()));
        return data;
    }
}
