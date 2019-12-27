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
    }

    public static class NodeBlobExtensions
    {
        public static IEnumerable<int> GetChildrenIndices(this INodeBlob blob, int parentIndex)
        {
            var endIndex = blob.GetEndIndex(parentIndex);
            for (var childIndex = parentIndex + 1; childIndex < endIndex; childIndex = blob.GetEndIndex(childIndex))
                yield return childIndex;
        }
    }

}
