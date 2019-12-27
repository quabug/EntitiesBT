using System.Linq;
using EntitiesBT.Core;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [DisallowMultipleComponent]
    public class BTRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private BTNode RootNode;
        private void Reset()
        {
            RootNode = GetComponentInChildren<BTNode>();
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // var blobRef = RootNode.ToBlob();
            //
            // var nodeBlobRef = new NodeBlobComponent(blobRef);
            // dstManager.AddComponentData(entity, nodeBlobRef);
            //
            // var blackboard = new EntityBlackboard(dstManager, entity);
            // dstManager.AddComponentData(entity, new BlackboardComponent {Value = blackboard});
            // dstManager.AddComponentData(entity, new VirtualMachineComponent { Value = vm });
            // if (_destroyNodes) Destroy(RootNode.gameObject);
        }
    }

    public static class BehaviorTreeExtensions
    {
        public static unsafe BlobAssetReference<NodeBlob> ToBlob(this BTNode root, Registries<IBehaviorNode> registries)
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
                    types[i] = node.GetTypeId(registries);
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
                return blobBuilder.CreateBlobAssetReference<NodeBlob>(Allocator.Persistent);
            }
        }
    }
}
