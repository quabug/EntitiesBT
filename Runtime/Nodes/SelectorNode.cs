using System.Runtime.InteropServices;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [StructLayout(LayoutKind.Explicit)]
    [BehaviorNode("BD4C1D8F-BA8E-4D74-9039-7D1E6010B058", BehaviorNodeType.Composite)]
    public struct SelectorNode : INodeData
    {
        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            return blob.TickChildrenReturnLastOrDefault(index, blackboard, breakCheck: state => state.IsRunningOrSuccess());
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}