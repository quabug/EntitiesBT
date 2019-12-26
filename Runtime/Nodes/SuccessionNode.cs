using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public abstract class SuccessionNode : IBehaviorNode
    {
        public struct Data : INodeData
        {
            public int ChildIndex;
        }
        
        protected abstract NodeState ContinueState { get; }

        public virtual void Reset(VirtualMachine vm, int index, IBlackboard blackboard)
        {
            vm.GetNodeData<Data>(index).ChildIndex = index + 1;
        }

        public virtual NodeState Tick(VirtualMachine vm, int index, IBlackboard blackboard)
        {
            ref var childIndex = ref vm.GetNodeData<Data>(index).ChildIndex;
            if (childIndex >= vm.EndIndex(index)) throw new IndexOutOfRangeException();
            
            while (childIndex < vm.EndIndex(index))
            {
                var childState = vm.Tick(childIndex);
                
                if (childState == NodeState.Running)
                    return childState;
                
                if (childState != ContinueState)
                {
                    childIndex = vm.EndIndex(index);
                    return childState;
                }
                
                childIndex = vm.EndIndex(childIndex);
            }
            return ContinueState;
        }
    }
}