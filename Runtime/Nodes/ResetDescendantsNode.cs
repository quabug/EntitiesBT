using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class ResetDescendantsNode : IBehaviorNode
    {
        public void Reset(VirtualMachine vm, int index, IBlackboard blackboard) {}

        public NodeState Tick(VirtualMachine vm, int index, IBlackboard blackboard)
        {
            foreach (var childIndex in vm.GetDescendantsIndices(index)) vm.Reset(childIndex);
            return NodeState.Success;
        }
    }
}