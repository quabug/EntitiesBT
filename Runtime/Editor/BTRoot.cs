using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [DisallowMultipleComponent]
    public class BTRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        public BehaviorNodeFactory Factory = new BehaviorNodeFactory();
        public BTNode RootNode;

        private void Reset()
        {
            RootNode = GetComponentInChildren<BTNode>();
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var blobRef = RootNode.ToBlob(Factory);
            dstManager.AddComponentData(entity, new NodeBlobRef(blobRef));
        }
    }

    public static class BehaviorTreeExtensions
    {
        public static unsafe BlobAssetReference<NodeBlob> ToBlob(this BTNode root, BehaviorNodeFactory factory)
        {
            var nodes = root.DescendantsWithSelf().ToArray();
            var blobSize = nodes.Select(n => n.Size).Sum();
            var size = NodeBlob.Size(nodes.Length, blobSize);
            using (var blobBuilder = new BlobBuilder(Allocator.Temp, size))
            {
                ref var blob = ref blobBuilder.ConstructRoot<NodeBlob>();
                var types = blobBuilder.Allocate(ref blob.Types, nodes.Length);
                var endIndices = blobBuilder.Allocate(ref blob.EndIndices, nodes.Length);
                var offsets = blobBuilder.Allocate(ref blob.Offsets, nodes.Length);
                var unsafePtr = (byte*) blobBuilder.Allocate(ref blob.DataBlob, blobSize).GetUnsafePtr();

                var offset = 0;
                for (var i = 0; i < nodes.Length; i++)
                {
                    var node = nodes[i];
                    node.Factory = factory;
                    node.Index = i;
                    types[i] = node.Type;
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
                return blobBuilder.CreateBlobAssetReference<NodeBlob>(Allocator.Persistent);
            }
        }
    }
}
