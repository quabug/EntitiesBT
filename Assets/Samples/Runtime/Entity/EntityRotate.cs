using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace EntitiesBT.Sample
{
    [Serializable]
    [BehaviorNode("8E25032D-C06F-4AA9-B401-1AD31AF43A2F")]
    public struct EntityRotateNode : INodeData
    {
        public float3 Axis;
        public float RadianPerSecond;
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var rotation = ref bb.GetDataRef<Rotation>();
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>();
            rotation.Value = math.mul(
                math.normalize(rotation.Value)
              , quaternion.AxisAngle(Axis, RadianPerSecond * deltaTime.Value)
            );
            return NodeState.Running;
        }
    }
}
