using EntitiesBT.Core;
using EntitiesBT.Components;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class TransformMove : BTNode<TransformMoveNode, TransformMoveNode.Data>
    {
        public Vector3 Velocity;
        
        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (TransformMoveNode.Data*) dataPtr;
            ptr->Velocity = Velocity;
        }
    }
    
    [BehaviorNode("B6DBD77F-1C83-4B0A-BB46-ECEE8D3C1BEF")]
    public class TransformMoveNode
    {
        public struct Data : INodeData
        {
            public Vector3 Velocity;
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var deltaTime = (float)bb.GetData<TickDeltaTime>().Value.TotalSeconds;
            ref var data = ref blob.GetNodeData<Data>(index);
            var transform = bb.GetData<Transform>();
            var deltaMove = data.Velocity * deltaTime;
            transform.position += deltaMove;
            return NodeState.Running;
        }
    }
}
