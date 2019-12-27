using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class ResetChildrenNode : IBehaviorNode
    {
        private readonly VirtualMachine _vm;
        public ResetChildrenNode(VirtualMachine vm)
        {
            _vm = vm;
        }
        
        public void Reset(int index, INodeBlob blob, IBlackboard blackboard) {}

        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            foreach (var childIndex in blob.GetChildrenIndices(index))
                _vm.Reset(childIndex, blob, blackboard);
            return NodeState.Success;
        }
    }
}