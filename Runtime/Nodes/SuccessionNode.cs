using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public abstract class SuccessionNode : IBehaviorNode
    {
        private readonly VirtualMachine _vm;

        public struct Data : INodeData
        {
            public int ChildIndex;
        }
        
        protected abstract NodeState ContinueState { get; }
        
        public SuccessionNode(VirtualMachine vm)
        {
            _vm = vm;
        }

        public virtual void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            blob.GetNodeData<Data>(index).ChildIndex = index + 1;
        }

        public virtual NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var childIndex = ref blob.GetNodeData<Data>(index).ChildIndex;
            if (childIndex >= blob.GetEndIndex(index)) throw new IndexOutOfRangeException();
            
            while (childIndex < blob.GetEndIndex(index))
            {
                var childState = _vm.Tick(childIndex, blob, blackboard);
                
                if (childState == NodeState.Running)
                    return childState;
                
                if (childState != ContinueState)
                {
                    childIndex = blob.GetEndIndex(index);
                    return childState;
                }
                
                childIndex = blob.GetEndIndex(childIndex);
            }
            return ContinueState;
        }
    }
}