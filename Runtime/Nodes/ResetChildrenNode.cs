using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("3F494113-5404-49D6-ABCC-8BB285B730F8")]
    public static class ResetChildrenNode
    {
        public static readonly int Id = typeof(ResetChildrenNode).GetBehaviorNodeId();
        
        static ResetChildrenNode()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }

        private static void Reset(int index, INodeBlob blob, IBlackboard blackboard) {}

        private static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            foreach (var childIndex in blob.GetChildrenIndices(index))
                VirtualMachine.Reset(childIndex, blob, blackboard);
            return NodeState.Success;
        }
    }
}