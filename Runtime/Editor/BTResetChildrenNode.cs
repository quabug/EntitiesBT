using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTResetChildrenNode : BTNode
    {
        public override int Type => Factory.GetTypeId<ResetChildrenNode>();
        public override unsafe void Build(void* dataPtr) {}
        public override int Size => 0;
    }
}
