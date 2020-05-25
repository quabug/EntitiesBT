using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;

namespace EntitiesBT.Core
{

    public interface INodeBlob
    {
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
        public static IEnumerable<int> GetChildrenIndices([NotNull] this INodeBlob blob, int parentIndex)
        {
            var endIndex = blob.GetEndIndex(parentIndex);
            var childIndex = parentIndex + 1;
            while (childIndex < endIndex)
            {
                yield return childIndex;
                childIndex = blob.GetEndIndex(childIndex);
            }
        }
        
        [LinqTunnel, Pure]
        public static IEnumerable<int> GetChildrenIndices([NotNull] this INodeBlob blob, int parentIndex, Predicate<NodeState> predicate)
        {
            return blob.GetChildrenIndices(parentIndex).Where(childIndex => predicate(blob.GetState(childIndex)));
        }
        
        public static void ForEachChildrenIndices([NotNull] this INodeBlob blob, int parentIndex, Action<int> action)
        {
            var endIndex = blob.GetEndIndex(parentIndex);
            var childIndex = parentIndex + 1;
            while (childIndex < endIndex)
            {
                action(childIndex);
                childIndex = blob.GetEndIndex(childIndex);
            }
        }
        
        [Pure]
        public static int FirstOrDefaultChildIndex([NotNull] this INodeBlob blob, int parentIndex, Predicate<NodeState> predicate)
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
        public static int ParentIndex([NotNull] this INodeBlob blob, int childIndex)
        {
            var endIndex = blob.GetEndIndex(childIndex);
            for (var i = childIndex - 1; i >= 0; i--)
            {
                if (blob.GetEndIndex(i) >= endIndex)
                    return i;
            }
            return -1;
        }
        
        public static unsafe void ResetRuntimeData([NotNull] this INodeBlob blob, int index, int count = 1)
        {
            var dest = (void*)blob.GetRuntimeDataPtr(index);
            var src = (void*)blob.GetDefaultDataPtr(index);
            UnsafeUtility.MemCpy(dest, src, blob.GetNodeDataSize(index, count));
        }

        [Pure]
        public static IntPtr GetNodeDataPtr([NotNull] this INodeBlob blob, int index)
        {
            return blob.GetRuntimeDataPtr(index);
        }

        [Pure]
        public static unsafe ref T GetNodeData<T>([NotNull] this INodeBlob blob, int index) where T : struct
        {
            return ref UnsafeUtilityEx.AsRef<T>((void*)blob.GetRuntimeDataPtr(index));
        }
        
        [Pure]
        public static unsafe ref T GetNodeDefaultData<T>([NotNull] this INodeBlob blob, int index) where T : struct
        {
            return ref UnsafeUtilityEx.AsRef<T>((void*)blob.GetDefaultDataPtr(index));
        }
    }

}
