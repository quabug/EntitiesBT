using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace EntitiesBT.Sample
{
    [Serializable]
    [BehaviorNode("F5C2EE7E-690A-4B5C-9489-FB362C949192")]
    public struct EntityMoveNode : INodeData
    {
        public float3 Velocity;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var translation = ref bb.GetDataRef<Translation>();
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>();
            translation.Value += Velocity * deltaTime.Value;
            return NodeState.Running;
        }
    }
}
