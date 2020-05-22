using System.Runtime.InteropServices;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [StructLayout(LayoutKind.Explicit)]
    [BehaviorNode("8A3B18AE-C5E9-4F34-BCB7-BD645C5017A5", BehaviorNodeType.Composite)]
    public struct SequenceNode : INodeData
    {
        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            return blob.TickChildrenReturnLastOrDefault(index, blackboard, breakCheck: state => state.IsRunningOrFailure());
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}