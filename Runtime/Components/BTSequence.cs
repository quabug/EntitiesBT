using EntitiesBT.Nodes;

namespace EntitiesBT.Components
{
    public class BTSequence : BTNode<SuccessionNode.Data>
    {
        public override int NodeId => SequenceNode.Id;
    }
}
