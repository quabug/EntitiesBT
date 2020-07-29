using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("F17339E0-D401-451B-864B-007AD44E05A3")]
    public struct RunningNode : INodeData
    {
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return NodeState.Running;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
