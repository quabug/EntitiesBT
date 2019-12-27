using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class SelectorNode : SuccessionNode
    {
        public SelectorNode(VirtualMachine vm) : base(vm) { }
        protected override NodeState ContinueState => NodeState.Failure;
    }
}