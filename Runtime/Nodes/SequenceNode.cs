using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class SequenceNode : SuccessionNode
    {
        protected override NodeState ContinueState => NodeState.Success;
    }
}