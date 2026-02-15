namespace ZCrew.Extensions.DependencyInjection.IntegrationTests.Fixtures;

internal class ConcreteService : IService
{
    private readonly Guid instanceId;

    public ConcreteService()
        : this(Guid.NewGuid()) { }

    public ConcreteService(Guid instanceId)
    {
        this.instanceId = instanceId;
    }

    public Stack<InstanceData> GetInstanceData()
    {
        var data = new Stack<InstanceData>();
        data.Push(new InstanceData(this.instanceId, GetType()));
        return data;
    }
}
