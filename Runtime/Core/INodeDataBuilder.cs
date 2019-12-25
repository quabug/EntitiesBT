namespace EntitiesBT.Core
{
    public interface INodeDataBuilder
    {
        IBehaviorNode BehaviorNode { get; }
        int Size { get; }
        unsafe void Build(void* dataPtr);
    }
}
