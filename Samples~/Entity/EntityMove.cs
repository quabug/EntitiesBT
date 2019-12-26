using EntitiesBT.Core;
using EntitiesBT.Editor;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class EntityMove : BTNode
    {
        public Vector3 Velocity;
        
        public override IBehaviorNode BehaviorNode => new EntityMoveNode();
        public override unsafe int Size => sizeof(EntityMoveNode.Data);
        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (EntityMoveNode.Data*) dataPtr;
            ptr->Velocity = Velocity;
        }
    }
    
    public class EntityMoveNode : IBehaviorNode
    {
        private readonly EntityManager _entityManager;

        public struct Data : INodeData
        {
            public float3 Velocity;
        }
        
        public void Reset(VirtualMachine vm, int index, IBlackboard blackboard) {}

        public NodeState Tick(VirtualMachine vm, int index, IBlackboard blackboard)
        {
            ref var data = ref vm.GetNodeData<Data>(index);
            var translation = (Translation) blackboard[typeof(Translation)];
            var deltaTime = (TickDeltaTime) blackboard[typeof(TickDeltaTime)];
            translation.Value += data.Velocity * (float)deltaTime.Value.TotalSeconds;
            blackboard[typeof(Translation)] = translation;
            return NodeState.Running;
        }
    }
}
