using System.Collections.Generic;

namespace EntitiesBT.Core
{

    public interface INodeBlob
    {
        int Count { get; }
        int GetTypeId(int nodeIndex);
        int GetEndIndex(int nodeIndex);
        ref T GetNodeData<T>(int nodeIndex) where T : struct, INodeData;
        unsafe void* GetNodeDataPtr(int nodeIndex);
        IEnumerable<int> GetChildrenIndices(int parentIndex);
    }

}
