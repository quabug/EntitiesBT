using System;
using EntitiesBT.Core;
using EntitiesBT.Components;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class TransformMove : BTNode<TransformMoveNode.Data>
    {
        public Vector3 Velocity;
        public override int NodeId => TransformMoveNode.Id;
        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (TransformMoveNode.Data*) dataPtr;
            ptr->Velocity = Velocity;
        }
    }
    
    public static class TransformMoveNode
    {
        public static readonly int Id = new Guid("B6DBD77F-1C83-4B0A-BB46-ECEE8D3C1BEF").GetHashCode();

        static TransformMoveNode()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }

        public struct Data : INodeData
        {
            public Vector3 Velocity;
        }
        
        public static void Reset(int index, INodeBlob blob, IBlackboard bb) {}

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var deltaTime = (float)((TickDeltaTime) bb[typeof(TickDeltaTime)]).Value.TotalSeconds;
            ref var data = ref blob.GetNodeData<Data>(index);
            var transform = (Transform)bb[typeof(Transform)];
            var deltaMove = data.Velocity * deltaTime;
            transform.position = transform.position + deltaMove;
            return NodeState.Running;
        }
    }
}
