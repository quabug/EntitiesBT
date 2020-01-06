using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    public static class BehaviorTreeExtensions
    {
        public static BlobAssetReference<NodeBlob> ToBlob(this BTNode root)
        {
            using (var builder = root.ToBlobBuilder())
                return builder.CreateBlobAssetReference<NodeBlob>(Allocator.Persistent);
        }
        
        public static unsafe BlobBuilder ToBlobBuilder(this BTNode root)
        {
            var nodes = root.DescendantsWithSelf().ToArray();
            var blobSize = nodes.Select(n => n.Size).Sum();
            var size = NodeBlob.CalculateSize(nodes.Length, blobSize);
            var blobBuilder = new BlobBuilder(Allocator.Temp, size);
            
            ref var blob = ref blobBuilder.ConstructRoot<NodeBlob>();
            var types = blobBuilder.Allocate(ref blob.Types, nodes.Length);
            var endIndices = blobBuilder.Allocate(ref blob.EndIndices, nodes.Length);
            var offsets = blobBuilder.Allocate(ref blob.Offsets, nodes.Length);
            var unsafePtr = (byte*) blobBuilder.Allocate(ref blob.DataBlob, blobSize).GetUnsafePtr();

            var offset = 0;
            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                types[i] = node.NodeId;
                node.Index = i;
                offsets[i] = offset;
                node.Build(unsafePtr + offset);
                offset += node.Size;

                var endIndex = i + 1;
                while (true)
                {
                    endIndices[node.Index] = endIndex;
                    var parent = node.transform.parent;
                    if (parent == null) break;
                    node = parent.GetComponent<BTNode>();
                    if (node == null) break;
                }
            }
            return blobBuilder;
        }
        
        public static BlobAssetReference<NodeBlob> ToBlob(this TextAsset file)
        {
            var result = BlobAssetReference<NodeBlob>.TryRead(file.bytes, NodeBlob.VERSION, out var blobRef);
            if (!result) throw new FormatException("Version is not match.");
            return blobRef;
        }

        public static ISet<ComponentType> GetAccessTypes(this BlobAssetReference<NodeBlob> blob)
        {
            return new HashSet<ComponentType>(Enumerable
                .Range(0, blob.Value.Types.Length)
                .Select(i => blob.Value.Types[i])
                .SelectMany(VirtualMachine.GetAccessTypes)
            );
        }

        public static void AddBehaviorTree(this Entity entity, EntityManager dstManager, BlobAssetReference<NodeBlob> blobRef, bool enableJob)
        {
            var blob = new NodeBlobRef { BlobRef = blobRef };
            if (enableJob)
            {
                var dataQuery = new BlackboardDataQuery {Value = blobRef.GetAccessTypes()};
                var jobBlackboard = new EntityJobChunkBlackboard();
                VirtualMachine.Reset(blob, jobBlackboard);
                dstManager.AddComponentData(entity, new JobBlackboard { Value = jobBlackboard });
                dstManager.AddSharedComponentData(entity, dataQuery);
                dstManager.AddComponentData(entity, new IsMainThread { Value = false });
            }
            else
            {
                var mainThreadBlackboard = new EntityBlackboard(dstManager, entity);
                VirtualMachine.Reset(blob, mainThreadBlackboard);
                dstManager.AddComponentData(entity, new MainThreadOnlyBlackboard {Value = mainThreadBlackboard});
                dstManager.AddComponentData(entity, new IsMainThread { Value = false });
            }
            dstManager.AddComponentData(entity, blob);
            dstManager.AddComponentData(entity, new TickDeltaTime());
        }
    }
}
