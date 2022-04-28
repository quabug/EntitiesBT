using System.Runtime.InteropServices;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [StructLayout(LayoutKind.Explicit)]
    [BehaviorNode("E99D019A-78E4-49B1-AE0D-6A1D0101E080", BehaviorNodeType.Decorate)]
    public struct StateReverseNode : INodeData
    {
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var childState = index.TickChild(ref blob, ref bb);
            switch (childState)
            {
            case NodeState.Success: return NodeState.Failure;
            case NodeState.Failure: return NodeState.Success;
            default: return childState;
            }
        }
    }
}
