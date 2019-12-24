using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class RunningNode : IBehaviorNode
    {
        public void Initialize(VirtualMachine vm, int index) {}
        public void Reset(VirtualMachine vm, int index) {}
        public NodeState Tick(VirtualMachine vm, int index) => NodeState.Running;
    }
}
