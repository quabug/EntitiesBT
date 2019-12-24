using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class FailureNode : IBehaviorNode
    {
        public void Reset(VirtualMachine vm, int index) {}
        public NodeState Tick(VirtualMachine vm, int index) => NodeState.Failure;
    }
}
