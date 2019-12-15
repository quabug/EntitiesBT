namespace EntitiesBT.Core
{
    public interface INodeDataBuilder
    {
        int Size { get; }
        int Type { get; }
        unsafe void Build(void* dataPtr);
    }
}
