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
            var childIndex = parentIndex + 1;
            while (childIndex < endIndex)
            {
                yield return childIndex;
                childIndex = blob.GetEndIndex(childIndex);
            }
        }

        public static int ParentIndex(this INodeBlob blob, int childIndex)
        {
            var endIndex = blob.GetEndIndex(childIndex);
            for (var i = childIndex - 1; i >= 0; i--)
            {
                if (blob.GetEndIndex(i) >= endIndex)
                    return i;
            }
            return -1;
        }
    }

}
