using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class SequenceNode : SuccessionNode
    {
        public SequenceNode(VirtualMachine vm) : base(vm) { }
        protected override NodeState ContinueState => NodeState.Success;
    }
}