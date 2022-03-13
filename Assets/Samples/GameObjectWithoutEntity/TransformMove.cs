using EntitiesBT.Core;
using EntitiesBT.Entities;
using Nuwa.Blob;
using UnityEngine;

namespace EntitiesBT.Sample
{
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

    public class Vector3Builder : PlainDataBuilder<Vector3> {}
}
