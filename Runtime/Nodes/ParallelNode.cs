using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class ParallelNode : IBehaviorNode
    {
        private readonly VirtualMachine _vm;
        public ParallelNode(VirtualMachine vm)
        {
            _vm = vm;
        }
        
        public unsafe void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var childrenStates = new SimpleBlobArray<NodeState>(blob.GetNodeDataPtr(index));
            for (var i = 0; i < childrenStates.Length; i++) childrenStates[i] = NodeState.Running;
        }

        public unsafe NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
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
                    childState = _vm.Tick(childIndex, blob, blackboard);
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