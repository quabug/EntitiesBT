using System;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public struct NodeBlob : INodeBlob
    {
        public const int VERSION = 0;

        public BlobArray<int> Types;
        public BlobArray<int> EndIndices;
        public BlobArray<int> Offsets;
        public BlobArray<byte> DataBlob;

        public int Count => Offsets.Length;

        public int GetTypeId(int nodeIndex)
        {
            return Types[nodeIndex];
        }

        public int GetEndIndex(int nodeIndex)
        {
            return EndIndices[nodeIndex];
        }

        public unsafe ref T GetNodeData<T>(int nodeIndex) where T : struct, INodeData
        {
            return ref UnsafeUtilityEx.AsRef<T>(GetNodeDataPtr(nodeIndex));
        }

        public int GetNodeDataSize(int nodeIndex)
        {
            var currentOffset = Offsets[nodeIndex];
            var nextOffset = nodeIndex + 1 < Offsets.Length ? Offsets[nodeIndex + 1] : Offsets.Length;
            return nextOffset - currentOffset;
        }

        public unsafe void* GetNodeDataPtr(int nodeIndex)
        {
            return (void*) ((IntPtr) DataBlob.GetUnsafePtr() + Offsets[nodeIndex]);
        }

        public int Size => CalculateSize(Count, DataBlob.Length);

        public static int CalculateSize(int count, int dataSize) =>
            UnsafeUtility.SizeOf<NodeBlob>() + dataSize + sizeof(int) * count * 3 /* Types/EndIndices/Offsets */;
    }

    public struct NodeBlobRef : IComponentData, INodeBlob
    {
        private ref NodeBlob _blob => ref BlobRef.Value;
        public BlobAssetReference<NodeBlob> BlobRef;
        
        public NodeBlobRef(BlobAssetReference<NodeBlob> blobRef) => BlobRef = blobRef;
        
        public int Count => _blob.Count;
        public int GetTypeId(int nodeIndex) => _blob.GetTypeId(nodeIndex);
        public int GetEndIndex(int nodeIndex) => _blob.GetEndIndex(nodeIndex);
        public int GetNodeDataSize(int nodeIndex) => _blob.GetNodeDataSize(nodeIndex);

        public unsafe void* GetNodeDataPtr(int nodeIndex) => _blob.GetNodeDataPtr(nodeIndex);
        public ref T GetNodeData<T>(int nodeIndex) where T : struct, INodeData => ref _blob.GetNodeData<T>(nodeIndex);
    }
}
