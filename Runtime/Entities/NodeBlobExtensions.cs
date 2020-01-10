using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace Entities
{
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

        public static int ParentIndex(this BlobAssetReference<NodeBlob> blob, int index)
        {
            var endIndex = blob.Value.EndIndices[index];
            for (var i = index - 1; i >= 0; i--)
            {
                if (blob.Value.EndIndices[i] >= endIndex)
                    return i;
            }
            return -1;
        }

        public static IEnumerable<int> ChildrenIndices(this BlobAssetReference<NodeBlob> blob, int index)
        {
            var endIndex = blob.Value.EndIndices[index];
            var childIndex = index + 1;
            while (childIndex < endIndex)
            {
                var childEndIndex = blob.Value.EndIndices[childIndex];
                yield return childIndex;
                childIndex = childEndIndex;
            }
        }
    }
}
