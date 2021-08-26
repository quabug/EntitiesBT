using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Samples
{
    [BehaviorNode("B4559A1E-392B-4B8C-A074-B323AB31EEA7")]
    public struct InputMoveToCharacterVelocityNode : INodeData
    {
        public BlobVariantRO<float> Speed;
        public BlobVariantRO<float2> InputMove;
        public BlobVariantRW<float3> OutputVelocity;
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var input = InputMove.Read(index, ref blob, ref bb);
            var direction = new Vector3(input.x, 0, input.y).normalized;
            var speed = Speed.Read(index, ref blob, ref bb);
            OutputVelocity.Write(index, ref blob, ref bb, direction * speed);
            return NodeState.Success;
        }
    }
}
