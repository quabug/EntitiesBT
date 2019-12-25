using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Collections.LowLevel.Unsafe;

namespace EntitiesBT.Editor
{
    public class BTParallel : BTNode
    {
        public override int Type => Factory.GetTypeId<ParallelNode>();

        public override unsafe void Build(void* dataPtr)
        {
            UnsafeUtilityEx.AsRef<int>(dataPtr) = gameObject.Children<BTNode>().Count();
        }
        
        public override int Size => SimpleBlobArray<NodeState>.Size(
            gameObject.Children<BTNode>().Count()
        );
    }
}
