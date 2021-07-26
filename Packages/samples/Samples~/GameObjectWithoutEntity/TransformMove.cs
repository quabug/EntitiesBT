using System.ComponentModel;
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
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            var transform = bb.GetObject<Transform>();
            var deltaMove = Velocity * deltaTime;
            transform.position += deltaMove;
            return NodeState.Running;
        }
    }
}
