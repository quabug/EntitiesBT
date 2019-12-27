namespace EntitiesBT.Core
{
    public interface INodeDataBuilder
    {
        int GetTypeId(Registries<IBehaviorNode> registries);
        int Size { get; }
        unsafe void Build(void* dataPtr);
    }
}
