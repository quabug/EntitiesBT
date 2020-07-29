using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("47E251B8-6AFB-43AA-8CFC-221FDB2726C6")]
    public struct HasComponentNode : INodeData
    {
        public ulong StableTypeHash;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var typeIndex = TypeManager.GetTypeIndexFromStableTypeHash(StableTypeHash);
            var type = TypeManager.GetType(typeIndex);
            return bb.HasData(type).ToNodeState();
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
