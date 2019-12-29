using System;
using EntitiesBT.Core;
using EntitiesBT.Components;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class EntityMove : BTNode<EntityMoveNode.Data>
    {
        public Vector3 Velocity;
        public override int NodeId => EntityMoveNode.Id;

        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (EntityMoveNode.Data*) dataPtr;
            ptr->Velocity = Velocity;
        }
    }
    
    public static class EntityMoveNode
    {
        public static readonly int Id = new Guid("F5C2EE7E-690A-4B5C-9489-FB362C949192").GetHashCode();

        static EntityMoveNode()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }
        
        public struct Data : INodeData
        {
            public float3 Velocity;
        }
        
        static void Reset(int index, INodeBlob blob, IBlackboard blackboard) {}

        static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            var translation = blackboard.GetData<Translation>();
            var deltaTime = blackboard.GetData<TickDeltaTime>();
            translation.Value += data.Velocity * (float)deltaTime.Value.TotalSeconds;
            blackboard.SetData(translation);;
            return NodeState.Running;
        }
    }
}
