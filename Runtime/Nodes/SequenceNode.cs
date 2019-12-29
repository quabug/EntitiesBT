using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("8A3B18AE-C5E9-4F34-BCB7-BD645C5017A5")]
    public static class SequenceNode
    {
        public static readonly int Id = typeof(SequenceNode).GetBehaviorNodeId();

        static SequenceNode()
        {
            VirtualMachine.Register(Id, SuccessionNode.Reset, SuccessionNode.Tick(NodeState.Success));
        }
    }
}