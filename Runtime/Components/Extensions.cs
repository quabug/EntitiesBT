using System;
using System.Linq;
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
    }
}
