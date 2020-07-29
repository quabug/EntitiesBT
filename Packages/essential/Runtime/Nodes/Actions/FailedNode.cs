using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("AC5CB763-5F7A-4301-9670-D4E38A5557CB")]
    public struct FailedNode : INodeData
    {
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return NodeState.Failure;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
