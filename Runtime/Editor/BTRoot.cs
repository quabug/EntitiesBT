using System.Collections.Generic;
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
        public BTNode RootNode;
        public VirtualMachine VirtualMachine;

        private void Reset()
        {
            RootNode = GetComponentInChildren<BTNode>();
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var (blobRef, behaviorNodes) = RootNode.ToBlob();
            var nodeBlobRef = new NodeBlobRef(blobRef);
            dstManager.AddComponentData(entity, nodeBlobRef);
            VirtualMachine = new VirtualMachine(nodeBlobRef, behaviorNodes);
            dstManager.AddComponentObject(entity, this);
            Destroy(RootNode.gameObject);
        }
    }

    public static class BehaviorTreeExtensions
    {
        public static unsafe (BlobAssetReference<NodeBlob> blobRef, IList<IBehaviorNode> nodes) ToBlob(this BTNode root)
        {
            var nodes = root.DescendantsWithSelf().ToArray();
            var blobSize = nodes.Select(n => n.Size).Sum();
            var size = NodeBlob.Size(nodes.Length, blobSize);
            var behaviorNodes = new IBehaviorNode[nodes.Length];
            using (var blobBuilder = new BlobBuilder(Allocator.Temp, size))
            {
                ref var blob = ref blobBuilder.ConstructRoot<NodeBlob>();
                var endIndices = blobBuilder.Allocate(ref blob.EndIndices, nodes.Length);
                var offsets = blobBuilder.Allocate(ref blob.Offsets, nodes.Length);
                var unsafePtr = (byte*) blobBuilder.Allocate(ref blob.DataBlob, blobSize).GetUnsafePtr();

                var offset = 0;
                for (var i = 0; i < nodes.Length; i++)
                {
                    var node = nodes[i];
                    node.Index = i;
                    behaviorNodes[i] = node.BehaviorNode;
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
                return (blobBuilder.CreateBlobAssetReference<NodeBlob>(Allocator.Persistent), behaviorNodes);
            }
        }
    }
}
