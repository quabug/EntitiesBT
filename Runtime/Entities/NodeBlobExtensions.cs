using System;
using System.IO;
using System.Linq;
using EntitiesBT.Core;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

namespace EntitiesBT.Entities
{
    public static class NodeBlobExtensions
    {
        public static BlobAssetReference<NodeBlob> ToBlob(this INodeDataBuilder root, Allocator allocator = Allocator.Persistent)
        {
            return root.Flatten(builder => builder.Children, builder => builder.Self).ToArray().ToBlob(allocator);
        }

        public static unsafe BlobAssetReference<NodeBlob> ToBlob(this ITreeNode<INodeDataBuilder>[] nodes, Allocator allocator)
        {
            var dataSize = 0;
            var nodeDataList = new NativeArray<BlobAssetReference>(nodes.Length, Allocator.Temp);
            try
            {
                for (var i = 0; i < nodes.Length; i++)
                {
                    var node = nodes[i];
                    var data = node.Value.Build(nodes);
                    nodeDataList[i] = data;
                    dataSize += data.Length;
                }

                var size = NodeBlob.CalculateSize(nodes.Length, dataSize);
                using (var blobBuilder = new BlobBuilder(Allocator.Temp, size))
                {
                    ref var blob = ref blobBuilder.ConstructRoot<NodeBlob>();
                    var types = blobBuilder.Allocate(ref blob.Types, nodes.Length);
                    var offsets = blobBuilder.Allocate(ref blob.Offsets, nodes.Length + 1);
                    var unsafeDataPtr = (byte*) blobBuilder.Allocate(ref blob.DefaultDataBlob, dataSize).GetUnsafePtr();
                    var offset = 0;
                    for (var i = 0; i < nodes.Length; i++)
                    {
                        var node = nodes[i];
                        types[i] = node.Value.NodeId;
                        offsets[i] = offset;
                        
                        var nodeDataSize = nodeDataList[i].Length;
                        var srcPtr = nodeDataList[i].GetUnsafePtr();
                        var destPtr = unsafeDataPtr + offset;
                        UnsafeUtility.MemCpy(destPtr, srcPtr, nodeDataSize);
                        
                        offset += nodeDataSize;
                    }

                    offsets[nodes.Length] = offset;

                    var endIndices = blobBuilder.Allocate(ref blob.EndIndices, nodes.Length);
                    // make sure the memory is clear to 0 (even it had been cleared on allocate)
                    UnsafeUtility.MemSet(endIndices.GetUnsafePtr(), 0, sizeof(int) * endIndices.Length);
                    for (var i = nodes.Length - 1; i >= 0; i--)
                    {
                        var endIndex = i + 1;
                        var node = nodes[i];
                        while (node != null && endIndices[node.Index] == 0)
                        {
                            endIndices[node.Index] = endIndex;
                            node = node.Parent;
                        }
                    }

                    var states = blobBuilder.Allocate(ref blob.States, nodes.Length);
                    UnsafeUtility.MemClear(states.GetUnsafePtr(), sizeof(NodeState) * states.Length);

                    var runtimeDataBlob = blobBuilder.Allocate(ref blob.RuntimeDataBlob, dataSize);
                    UnsafeUtility.MemCpy(runtimeDataBlob.GetUnsafePtr(), unsafeDataPtr, dataSize);

                    return blobBuilder.CreateBlobAssetReference<NodeBlob>(allocator);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                throw;
            }
            finally
            {
                foreach (var data in nodeDataList.Where(data => data.IsCreated)) data.Dispose();
                nodeDataList.Dispose();
            }
        }

        public static unsafe void SaveToStream(this INodeDataBuilder builder, Stream stream)
        {
            using (var blob = builder.ToBlob(Allocator.Temp))
            using (var writer = new MemoryBinaryWriter())
            {
                writer.Write(NodeBlob.VERSION);
                writer.Write(blob);
                var runtimePartSize = NodeBlob.CalculateRuntimeSize(blob.Value.Count, blob.Value.RuntimeDataBlob.Length);
                // HACK: truncate the runtime part of data (NodeState and RuntimeNodeData)
                var finalSize = writer.Length - runtimePartSize;
                using (var writerData = new UnmanagedMemoryStream(writer.Data, finalSize))
                    writerData.CopyTo(stream);
            }
        }
    }
}
