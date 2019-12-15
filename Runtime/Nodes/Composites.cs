using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public struct SequenceNodeData : INodeData { }

    public abstract class SuccessionNode : IBehaviorNode
    {
        protected int ChildIndex;
        protected abstract NodeState ContinueState { get; }
        
        public virtual void Reset(VirtualMachine vm, int index)
        {
            ChildIndex = index + 1;
        }

        public virtual NodeState Tick(VirtualMachine vm, int index)
        {
            while (ChildIndex < vm.EndIndex(index))
            {
                var (childState, childEndIndex) = vm.Tick(ChildIndex);
                if (childState != ContinueState) return childState;
                ChildIndex = childEndIndex;
            }
            return ContinueState;
        }
    }
    
    public class SequenceNode : SuccessionNode
    {
        protected override NodeState ContinueState => NodeState.Success;
    }

    public class RestartSequenceNode : SequenceNode
    {
        public override NodeState Tick(VirtualMachine vm, int index)
        {
            ChildIndex = index + 1;
            return base.Tick(vm, index);
        }
    }
    
    public class SelectorNode : SuccessionNode
    {
        protected override NodeState ContinueState => NodeState.Failed;
    }

    public class RestartSelectorNode : SelectorNode
    {
        public override NodeState Tick(VirtualMachine vm, int index)
        {
            ChildIndex = index + 1;
            return base.Tick(vm, index);
        }
    }
}
