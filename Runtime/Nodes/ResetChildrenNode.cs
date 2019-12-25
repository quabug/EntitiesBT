using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class ResetChildrenNode : IBehaviorNode
    {
        public void Reset(VirtualMachine vm, int index) {}

        public NodeState Tick(VirtualMachine vm, int index)
        {
            foreach (var childIndex in vm.GetChildrenIndices(index)) vm.Reset(childIndex);
            return NodeState.Success;
        }
    }
}