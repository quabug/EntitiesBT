using EntitiesBT.Core;
using EntitiesBT.Components;
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
    
    [BehaviorNode("F5C2EE7E-690A-4B5C-9489-FB362C949192")]
    public class EntityMoveNode
    {
        public struct Data : INodeData
        {
            public float3 Velocity;
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
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
