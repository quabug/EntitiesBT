using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public static class SelectorNode
    {
        public static int Id = 1;
        
        static SelectorNode()
        {
            VirtualMachine.Register(Id, SuccessionNode.Reset, SuccessionNode.Tick(NodeState.Failure));
        }
    }
}