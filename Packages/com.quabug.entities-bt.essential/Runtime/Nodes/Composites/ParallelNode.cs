using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("A316D182-7D8C-4075-A46D-FEE08CAEEEAF", BehaviorNodeType.Composite)]
    public struct ParallelNode : INodeData
    {
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            NodeState flags = 0;
            var endIndex = blob.GetEndIndex(index);
            var childIndex = index + 1;
            while (childIndex < endIndex)
            {
                var prevState = blob.GetState(childIndex);
                flags |= prevState.IsCompleted() ? 0 : VirtualMachine.Tick(childIndex, ref blob, ref bb);
                childIndex = blob.GetEndIndex(childIndex);
            }
            
            // var flags = blob.TickChildren(index, blackboard)
            //     .Aggregate((NodeState)0, (childStateFlags, childState) => {
            //         childStateFlags |= childState;
            //         return childStateFlags;
            //     });

            if (flags.HasFlagFast(NodeState.Running)) return NodeState.Running;
            if (flags.HasFlagFast(NodeState.Failure)) return NodeState.Failure;
            if (flags.HasFlagFast(NodeState.Success)) return NodeState.Success;
            return 0;
        }
    }
}