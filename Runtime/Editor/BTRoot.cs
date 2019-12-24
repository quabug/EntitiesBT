using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [RequireComponent(typeof(BTNode)), DisallowMultipleComponent]
    public class BTRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var blobRef = GetComponent<BTNode>().ToBlob();
            dstManager.AddComponentData(entity, new NodeBlobRef(blobRef));
        }
    }

    public static class BehaviorTreeExtensions
    {
        public static unsafe BlobAssetReference<NodeBlob> ToBlob(this BTNode root)
        {
            var nodes = root.DescendantsWithSelf().ToArray();
            var size = NodeBlob.Size(nodes.Length, nodes.Select(n => n.Size).Sum());
            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var blob = ref blobBuilder.ConstructRoot<NodeBlob>();
                var types = blobBuilder.Allocate(ref blob.Types, nodes.Length);
                var endIndices = blobBuilder.Allocate(ref blob.EndIndices, nodes.Length);
                var offsets = blobBuilder.Allocate(ref blob.Offsets, nodes.Length);
                var unsafePtr = (byte*) blobBuilder.Allocate(ref blob.DataBlob, size).GetUnsafePtr();

                var offset = 0;
                for (var i = 0; i < nodes.Length; i++)
                {
                    var node = nodes[i];
                    node.Index = i;
                    offset += node.Size;
                    types[i] = node.Type;
                    offsets[i] = offset;
                    node.Build(unsafePtr + offset);

                    var endIndex = i + 1;
                    while (true)
                    {
                        endIndices[node.Index] = endIndex;
                        var parent = node.transform.parent;
                        if (parent == null) break;
                        node = parent.GetComponent<BTNode>();
                    }
                }
                return blobBuilder.CreateBlobAssetReference<NodeBlob>(Allocator.Persistent);
            }
        }
    }
}
