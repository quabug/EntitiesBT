using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public static class SequenceNode
    {
        public static int Id = 0;

        static SequenceNode()
        {
            VirtualMachine.Register(Id, SuccessionNode.Reset, SuccessionNode.Tick(NodeState.Success));
        }
    }
}