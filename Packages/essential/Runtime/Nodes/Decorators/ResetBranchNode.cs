using System.Runtime.InteropServices;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [StructLayout(LayoutKind.Explicit)]
    [BehaviorNode("3F494113-5404-49D6-ABCC-8BB285B730F8", BehaviorNodeType.Decorate)]
    public struct ResetBranchNode : INodeData
    {
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            VirtualMachine.Reset(index, ref blob, ref blackboard, blob.GetEndIndex(index) - index);
            return NodeState.Success;
        }
    }
}