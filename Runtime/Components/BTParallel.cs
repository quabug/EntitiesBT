using EntitiesBT.Nodes;
using Unity.Collections.LowLevel.Unsafe;

namespace EntitiesBT.Components
{
    public class BTParallel : BTNode<ParallelNode>
    {
        public override unsafe void Build(void* dataPtr)
        {
            UnsafeUtilityEx.AsRef<int>(dataPtr) = ChildCount;
        }

        public override int Size => ParallelNode.DataSize(ChildCount);
    }
}
