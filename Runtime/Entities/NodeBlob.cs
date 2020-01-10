using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using Unity.Collections;
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
        public unsafe void* GetNodeDataPtr(int nodeIndex) => _blob.GetNodeDataPtr(nodeIndex);
        public ref T GetNodeData<T>(int nodeIndex) where T : struct, INodeData => ref _blob.GetNodeData<T>(nodeIndex);
    }

    public static class NodeBlobExtensions
    {
        public static unsafe BlobAssetReference<NodeBlob> ToBlob(this IList<ITreeNode<INodeDataBuilder>> nodes, Allocator allocator)
        {
            var blobSize = nodes.Select(n => n.Value.Size).Sum();
            var size = NodeBlob.CalculateSize(nodes.Count, blobSize);
            using (var blobBuilder = new BlobBuilder(Allocator.Temp, size))
            {
                ref var blob = ref blobBuilder.ConstructRoot<NodeBlob>();
                var types = blobBuilder.Allocate(ref blob.Types, nodes.Count);
                var offsets = blobBuilder.Allocate(ref blob.Offsets, nodes.Count);
                var unsafePtr = (byte*) blobBuilder.Allocate(ref blob.DataBlob, blobSize).GetUnsafePtr();

                var offset = 0;
                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    types[i] = node.Value.NodeId;
                    offsets[i] = offset;
                    node.Value.Build(unsafePtr + offset);
                    offset += node.Value.Size;
                }
                
                var endIndices = blobBuilder.Allocate(ref blob.EndIndices, nodes.Count);
                // make sure the memory is clear to 0 (even it had been cleared on allocate)
                UnsafeUtility.MemSet(endIndices.GetUnsafePtr(), 0, endIndices.Length);
                for (var i = nodes.Count - 1; i >= 0; i--)
                {
                    var endIndex = i + 1;
                    var node = nodes[i];
                    while (node != null && endIndices[node.Index] == 0)
                    {
                        endIndices[node.Index] = endIndex;
                        node = node.Parent;
                    }
                }
                return blobBuilder.CreateBlobAssetReference<NodeBlob>(allocator);
            }
        }
    }
}
