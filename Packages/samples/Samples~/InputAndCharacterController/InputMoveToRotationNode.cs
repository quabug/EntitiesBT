using EntitiesBT.Core;
using EntitiesBT.Variant;
using Unity.Mathematics;

namespace EntitiesBT.Samples
{
    [BehaviorNode("2164B3CA-C12E-4C86-9F80-F45A99124FAD")]
    public struct InputMoveToRotationNode : INodeData
    {
        public BlobVariantRO<float2> InputMove;
        public BlobVariantRW<quaternion> OutputDirection;
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var move = InputMove.Read(index, ref blob, ref bb);
            if (math.lengthsq(move) <= math.FLT_MIN_NORMAL) return NodeState.Success;
            
            var direction = quaternion.LookRotationSafe(new float3(move.x, 0, move.y), math.up());
            OutputDirection.Write(index, ref blob, ref bb, direction);
            return NodeState.Success;
        }
    }
}
