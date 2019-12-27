using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTSequence : BTNode<SuccessionNode.Data>
    {
        public override int NodeId => SequenceNode.Id;
    }
}
