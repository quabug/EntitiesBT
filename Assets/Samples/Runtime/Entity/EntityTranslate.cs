using System;
using EntitiesBT.Core;
using Unity.Mathematics;
using Unity.Transforms;

namespace EntitiesBT.Sample
{
    [Serializable]
    [BehaviorNode("29A30E27-7A3C-42F4-A0A4-49EFBD890279")]
    public struct EntityTranslateNode : INodeData
    {
        public float3 Position;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var translation = ref bb.GetDataRef<Translation>();
            translation.Value = Position;
            return NodeState.Success;
        }
    }
}
