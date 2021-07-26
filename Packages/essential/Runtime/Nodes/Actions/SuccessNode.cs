using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("A2E43D78-8993-4D0A-9CD0-70A98AAF9E8A")]
    public struct SuccessNode : INodeData
    {
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return NodeState.Success;
        }
    }
}
