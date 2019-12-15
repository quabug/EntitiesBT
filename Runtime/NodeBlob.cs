using System;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT
{
    public struct NodeBlob : INodeBlob
    {
        public BlobArray<int> Types;
        public BlobArray<int> EndIndices;
        public BlobArray<int> Offsets;
        public BlobArray<byte> DataBlob;

        public int Count => Offsets.Length;

        public int GetNodeType(int nodeIndex)
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

        public unsafe void* GetNodeDataPtr(int nodeIndex)
        {
            return (void*) ((IntPtr) DataBlob.GetUnsafePtr() + Offsets[nodeIndex]);
        }
    }

    public class NodeBlobRef : INodeBlob
    {
        private readonly BlobAssetReference<NodeBlob> _blobRef;

        private ref NodeBlob _blob => ref _blobRef.Value;
        public NodeBlobRef(BlobAssetReference<NodeBlob> blobRef)
        {
            _blobRef = blobRef;
        }

        public int Count => _blob.Count;
        public int GetNodeType(int nodeIndex) => _blob.GetNodeType(nodeIndex);
        public int GetEndIndex(int nodeIndex) => _blob.GetEndIndex(nodeIndex);
        public unsafe void* GetNodeDataPtr(int nodeIndex) => _blob.GetNodeDataPtr(nodeIndex);
        public ref T GetNodeData<T>(int nodeIndex) where T : struct, INodeData =>
            ref _blob.GetNodeData<T>(nodeIndex);
    }
}
