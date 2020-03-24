using System.Linq;
using System.Runtime.InteropServices;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [StructLayout(LayoutKind.Explicit)]
    [BehaviorNode("A316D182-7D8C-4075-A46D-FEE08CAEEEAF", BehaviorNodeType.Composite)]
    public struct ParallelNode : INodeData
    {
        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var flags = blob.TickChildren(index, blackboard)
                .Aggregate((NodeState)0, (childStateFlags, childState) => {
                    childStateFlags |= childState;
                    return childStateFlags;
                });

            if (flags.HasFlagFast(NodeState.Running)) return NodeState.Running;
            if (flags.HasFlagFast(NodeState.Failure)) return NodeState.Failure;
            if (flags.HasFlagFast(NodeState.Success)) return NodeState.Success;
            return 0;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}