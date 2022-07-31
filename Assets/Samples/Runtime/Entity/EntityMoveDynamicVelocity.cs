using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Variant;
using Unity.Mathematics;
using Unity.Transforms;

namespace EntitiesBT.Sample
{
    [Serializable]
    [BehaviorNode("CBCA71B5-B674-4EFA-B227-83A53CAB37EF")]
    public struct EntityMoveDynamicVelocityNode : INodeData
    {
        public BlobVariantRW<float3> Velocity;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var translation = ref bb.GetDataRef<Translation>();
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>();
            translation.Value += Velocity.Read(index, ref blob, ref bb) * deltaTime.Value;
            return NodeState.Running;
        }
    }
}
