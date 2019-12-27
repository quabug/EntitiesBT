using EntitiesBT.Core;
using EntitiesBT.Editor;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class EntityMove : BTNode<EntityMoveNode, EntityMoveNode.Data>
    {
        public Vector3 Velocity;
        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (EntityMoveNode.Data*) dataPtr;
            ptr->Velocity = Velocity;
        }
    }
    
    public class EntityMoveNode : IBehaviorNode
    {
        public struct Data : INodeData
        {
            public float3 Velocity;
        }
        
        public void Reset(int index, INodeBlob blob, IBlackboard blackboard) {}

        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            var translation = (Translation) blackboard[typeof(Translation)];
            var deltaTime = (TickDeltaTime) blackboard[typeof(TickDeltaTime)];
            translation.Value += data.Velocity * (float)deltaTime.Value.TotalSeconds;
            blackboard[typeof(Translation)] = translation;
            return NodeState.Running;
        }
    }
}
