using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTSelector : BTNode
    {
        public override IBehaviorNode BehaviorNode => new SelectorNode();
        public override unsafe int Size => sizeof(SuccessionNode.Data);
        public override unsafe void Build(void* dataPtr) {}
    }
}
