using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTSequence : BTNode
    {
        public override int Type => Factory.GetTypeId<SequenceNode>();
        public override unsafe int Size => sizeof(SuccessionNode.Data);
        public override unsafe void Build(void* dataPtr) {}
    }
}
