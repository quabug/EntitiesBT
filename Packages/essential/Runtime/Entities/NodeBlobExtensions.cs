using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blob;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

namespace EntitiesBT.Entities
{
    public static class NodeBlobExtensions
    {
        [Pure]
        public static BlobAssetReference<NodeBlob> ToBlob(
            [NotNull] this INodeDataBuilder root
            , IReadOnlyList<IGlobalValuesBuilder> globalValuesList
            , Allocator allocator = Allocator.Persistent
        )
        {
            return root.Flatten(builder => builder.Children, builder => builder.Self).ToArray().ToBlob(globalValuesList, allocator);
        }

        [Pure]
        public static unsafe BlobAssetReference<NodeBlob> ToBlob(
            [NotNull] this ITreeNode<INodeDataBuilder>[] nodes
            , IReadOnlyList<IGlobalValuesBuilder> globalValuesList
            , Allocator allocator
        )
        {
            var dataSize = 0;
            var nodeDataList = new NativeArray<BlobAssetReference>(nodes.Length, Allocator.Temp);
            try
            {
                var scopeValuesSize = 0;
                foreach (var values in globalValuesList)
                {
                    values.Offset = scopeValuesSize;
                    scopeValuesSize += values.Size;
                }

                for (var i = 0; i < nodes.Length; i++) nodes[i].Value.NodeIndex = i;

                for (var i = 0; i < nodes.Length; i++)
                {
                    var node = nodes[i];
                    var data = node.Value.Build(nodes);
                    nodeDataList[i] = data;
                    dataSize += data.Length;
                }

                var size = NodeBlob.CalculateSize(count: nodes.Length, dataSize: dataSize, scopeValuesSize: scopeValuesSize);

                var types = new int[nodes.Length];
                var offsets = new int[nodes.Length + 1];
                var binary = new byte[dataSize];
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

                var scopeValues = blobBuilder.Allocate(ref blob.DefaultGlobalValues, scopeValuesSize);
                var scopeValuesPtr = new IntPtr(scopeValues.GetUnsafePtr());
                var scopeValuesOffset = 0;
                foreach (var values in globalValuesList)
                {
                    var destPtr = scopeValuesPtr + scopeValuesOffset;
                    UnsafeUtility.MemCpy(destPtr.ToPointer(), values.ValuePtr.ToPointer(), values.Size);
                    scopeValuesOffset += values.Size;
                }
                var runtimeScopeValues = blobBuilder.Allocate(ref blob.RuntimeGlobalValues, scopeValuesSize);
                UnsafeUtility.MemCpy(runtimeScopeValues.GetUnsafePtr(), scopeValues.GetUnsafePtr(), scopeValuesSize);

                var states = blobBuilder.Allocate(ref blob.States, nodes.Length);
                UnsafeUtility.MemClear(states.GetUnsafePtr(), sizeof(NodeState) * states.Length);

                var runtimeDataBlob = blobBuilder.Allocate(ref blob.RuntimeDataBlob, dataSize);
                UnsafeUtility.MemCpy(runtimeDataBlob.GetUnsafePtr(), unsafeDataPtr, dataSize);

                return blobBuilder.CreateBlobAssetReference<NodeBlob>(allocator);
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

        public static unsafe void SaveToStream(
            [NotNull] this INodeDataBuilder builder
            , [NotNull] IReadOnlyList<IGlobalValuesBuilder> scopeValuesList
            , [NotNull] Stream stream
        )
        {
            using var blob = builder.ToBlob(scopeValuesList, Allocator.Temp);
            using var writer = new MemoryBinaryWriter();
            writer.Write(NodeBlob.VERSION);
            writer.Write(blob);
            var runtimePartSize = blob.Value.RuntimeSize;
            // HACK: truncate the runtime part of data (NodeState and RuntimeNodeData)
            var finalSize = writer.Length - runtimePartSize;
            using var writerData = new UnmanagedMemoryStream(writer.Data, finalSize);
            writerData.CopyTo(stream);
        }

        [Pure]
        public static Entity CloneBehaviorTree(this EntityManager dstManager, Entity behaviorTreeEntity)
        {
            var query = dstManager.GetSharedComponentData<BlackboardDataQuery>(behaviorTreeEntity);
            var comp = dstManager.GetComponentData<BehaviorTreeComponent>(behaviorTreeEntity);
            var clone = dstManager.CreateEntity();
#if UNITY_EDITOR
            dstManager.SetName(clone, dstManager.GetName(behaviorTreeEntity)+"-clone");
#endif
            dstManager.AddSharedComponentData(clone, query);
            var blob = new NodeBlobRef(comp.Blob.BlobRef.Clone());
            dstManager.AddComponentData(clone, new BehaviorTreeComponent(blob, comp.Thread, comp.AutoCreation));
            return clone;
        }

        [Pure]
        public static unsafe BlobAssetReference<T> Clone<T>(this BlobAssetReference<T> @this) where T : unmanaged
        {
            var untypedRef = BlobAssetReference.Create(@this);
            return BlobAssetReference<T>.Create(untypedRef.GetUnsafePtr(), untypedRef.Length);
        }
    }
}
