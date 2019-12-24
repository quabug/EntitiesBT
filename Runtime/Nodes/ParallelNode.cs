using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class ParallelNode : IBehaviorNode
    {
        public unsafe void Reset(VirtualMachine vm, int index)
        {
            var childrenStates = new SimpleBlobArray<NodeState>(vm.GetNodeDataPtr(index));
            for (var i = 0; i < childrenStates.Length; i++) childrenStates[i] = NodeState.Running;
        }

        public unsafe NodeState Tick(VirtualMachine vm, int index)
        {
            var childrenStates = new SimpleBlobArray<NodeState>(vm.GetNodeDataPtr(index));
            var hasAnyRunningChild = false;
            var state = NodeState.Success;
            var localChildIndex = 0;
            var childIndex = index + 1;
            while (childIndex < vm.EndIndex(index))
            {
                var childState = childrenStates[localChildIndex];
                if (childState == NodeState.Running)
                {
                    childState = vm.Tick(childIndex);
                    childrenStates[localChildIndex] = childState;
                    hasAnyRunningChild = true;
                }

                childIndex = vm.EndIndex(childIndex);
                localChildIndex++;
                
                if (state == NodeState.Running) continue;
                if (childState != NodeState.Success) state = childState;
            }
            
            if (hasAnyRunningChild) return state;
            throw new IndexOutOfRangeException();
        }
    }
}