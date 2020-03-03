using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Components;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class TransformMove : BTNode<TransformMoveNode>
    {
        public Vector3 Velocity;

        protected override unsafe void Build(void* dataPtr, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            var ptr = (TransformMoveNode*) dataPtr;
            ptr->Velocity = Velocity;
        }
    }
    
    [BehaviorNode("B6DBD77F-1C83-4B0A-BB46-ECEE8D3C1BEF")]
    public struct TransformMoveNode : INodeData
    {
        public Vector3 Velocity;
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>();
            yield return ComponentType.ReadWrite<Transform>();
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            ref var data = ref blob.GetNodeData<TransformMoveNode>(index);
            var transform = bb.GetData<Transform>();
            var deltaMove = data.Velocity * deltaTime;
            transform.position += deltaMove;
            return NodeState.Running;
        }
    }
}
