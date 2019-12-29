using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("A316D182-7D8C-4075-A46D-FEE08CAEEEAF")]
    public static class ParallelNode
    {
        public static readonly int Id = typeof(ParallelNode).GetBehaviorNodeId();
        
        static ParallelNode()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }
        
        private static unsafe void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var childrenStates = new SimpleBlobArray<NodeState>(blob.GetNodeDataPtr(index));
            for (var i = 0; i < childrenStates.Length; i++) childrenStates[i] = NodeState.Running;
        }

        private static unsafe NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var childrenStates = new SimpleBlobArray<NodeState>(blob.GetNodeDataPtr(index));
            var hasAnyRunningChild = false;
            var state = NodeState.Success;
            var localChildIndex = 0;
            var childIndex = index + 1;
            while (childIndex < blob.GetEndIndex(index))
            {
                var childState = childrenStates[localChildIndex];
                if (childState == NodeState.Running)
                {
                    childState = VirtualMachine.Tick(childIndex, blob, blackboard);
                    childrenStates[localChildIndex] = childState;
                    hasAnyRunningChild = true;
                }

                childIndex = blob.GetEndIndex(childIndex);
                localChildIndex++;
                
                if (state == NodeState.Running) continue;
                if (childState != NodeState.Success) state = childState;
            }
            
            if (hasAnyRunningChild) return state;
            throw new IndexOutOfRangeException();
        }
    }
}