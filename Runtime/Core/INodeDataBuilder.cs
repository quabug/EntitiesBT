namespace EntitiesBT.Core
{
    public interface INodeDataBuilder
    {
        // int GetTypeId(TypeRegistries<IBehaviorNode> registries);
        int Size { get; }
        unsafe void Build(void* dataPtr);
    }
}
