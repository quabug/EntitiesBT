using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("BD4C1D8F-BA8E-4D74-9039-7D1E6010B058")]
    public static class SelectorNode
    {
        public static readonly int Id = typeof(SelectorNode).GetBehaviorNodeId();
        
        static SelectorNode()
        {
            VirtualMachine.Register(Id, SuccessionNode.Reset, SuccessionNode.Tick(NodeState.Failure));
        }
    }
}