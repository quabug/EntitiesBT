using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class ResetDescendantsNode
    {
        public void Initialize(VirtualMachine vm, int index) {}
        public void Reset(VirtualMachine vm, int index) {}

        public NodeState Tick(VirtualMachine vm, int index)
        {
            foreach (var childIndex in vm.GetDescendantsIndices(index)) vm.Reset(childIndex);
            return NodeState.Success;
        }
    }
}