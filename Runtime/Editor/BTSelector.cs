using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTSelector : BTNode
    {
        public override int Type => Factory.GetTypeId<SelectorNode>();
        public override unsafe int Size => sizeof(SuccessionNode.Data);
        public override unsafe void Build(void* dataPtr) {}
    }
}
