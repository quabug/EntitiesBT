using System;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;

namespace EntitiesBT.Core
{
    public interface INodeBlob
    {
        int RuntimeId { [Pure] get; }
        int Count { [Pure] get; }
        
        [Pure] int GetTypeId(int nodeIndex);
        [Pure] int GetEndIndex(int nodeIndex);
        [Pure] int GetNodeDataSize(int startNodeIndex, int count = 1);
        
        [Pure] NodeState GetState(int nodeIndex);
        void SetState(int nodeIndex, NodeState state);
        void ResetStates(int index, int count = 1);
        
        [Pure] IntPtr GetDefaultDataPtr(int nodeIndex);
        [Pure] IntPtr GetRuntimeDataPtr(int nodeIndex);
    }

    public static class NodeBlobExtensions
    {
        [Pure]
        public static int FirstOrDefaultChildIndex<TNodeBlob>(this ref TNodeBlob blob, int parentIndex, Predicate<NodeState> predicate)
            where TNodeBlob : struct, INodeBlob
        {
            var endIndex = blob.GetEndIndex(parentIndex);
            var childIndex = parentIndex + 1;
            while (childIndex < endIndex)
            {
                if (predicate(blob.GetState(childIndex))) return childIndex;
                childIndex = blob.GetEndIndex(childIndex);
            }
            return default;
        }

        [Pure]
        public static int ParentIndex<TNodeBlob>(this ref TNodeBlob blob, int childIndex)
            where TNodeBlob : struct, INodeBlob
        {
            var endIndex = blob.GetEndIndex(childIndex);
            for (var i = childIndex - 1; i >= 0; i--)
            {
                if (blob.GetEndIndex(i) >= endIndex)
                    return i;
            }
            return -1;
        }
        
        public static unsafe void ResetRuntimeData<TNodeBlob>(this ref TNodeBlob blob, int index, int count = 1)
            where TNodeBlob : struct, INodeBlob
        {
            var dest = (void*)blob.GetRuntimeDataPtr(index);
            var src = (void*)blob.GetDefaultDataPtr(index);
            UnsafeUtility.MemCpy(dest, src, blob.GetNodeDataSize(index, count));
        }

        [Pure]
        public static unsafe ref T GetNodeData<T, TNodeBlob>(this ref TNodeBlob blob, int index) where T : struct
            where TNodeBlob : struct, INodeBlob
        {
            return ref UnsafeUtility.AsRef<T>((void*)blob.GetRuntimeDataPtr(index));
        }
        
        [Pure]
        public static unsafe ref T GetNodeDefaultData<T, TNodeBlob>(this ref TNodeBlob blob, int index) where T : struct
            where TNodeBlob : struct, INodeBlob
        {
            return ref UnsafeUtility.AsRef<T>((void*)blob.GetDefaultDataPtr(index));
        }
    }

}
