using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public abstract class SuccessionNode : IBehaviorNode
    {
        protected int ChildIndex;
        protected abstract NodeState ContinueState { get; }

        public virtual void Initialize(VirtualMachine vm, int index) {}
        
        public virtual void Reset(VirtualMachine vm, int index)
        {
            ChildIndex = index + 1;
        }

        public virtual NodeState Tick(VirtualMachine vm, int index)
        {
            while (ChildIndex < vm.EndIndex(index))
            {
                var childState = vm.Tick(ChildIndex);
                if (childState != ContinueState) return childState;
                ChildIndex = vm.EndIndex(ChildIndex);
            }
            return ContinueState;
        }
    }
}