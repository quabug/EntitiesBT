namespace EntitiesBT.Core
{

    public interface INodeBlob
    {
        int Count { get; }
        int GetNodeType(int nodeIndex);
        int GetEndIndex(int nodeIndex);
        ref T GetNodeData<T>(int nodeIndex) where T : struct, INodeData;
        unsafe void* GetNodeDataPtr(int nodeIndex);
    }

}
