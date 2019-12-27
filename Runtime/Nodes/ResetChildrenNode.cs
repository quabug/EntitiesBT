using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public static class ResetChildrenNode
    {
        public static int Id = 6;
        
        static ResetChildrenNode()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }
        
        public static void Reset(int index, INodeBlob blob, IBlackboard blackboard) {}

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            foreach (var childIndex in blob.GetChildrenIndices(index))
                VirtualMachine.Reset(childIndex, blob, blackboard);
            return NodeState.Success;
        }
    }
}