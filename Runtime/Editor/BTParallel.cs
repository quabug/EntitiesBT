using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTParallel : BTNode
    {
        public override int Type => Factory.GetTypeId<ParallelNode>();
        public override unsafe void Build(void* dataPtr) {}
        public override int Size => 0;
    }
}
