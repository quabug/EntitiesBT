using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class SelectorNode : SuccessionNode
    {
        protected override NodeState ContinueState => NodeState.Failure;
    }
}