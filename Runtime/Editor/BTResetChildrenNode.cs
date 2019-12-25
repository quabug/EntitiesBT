using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTResetChildrenNode : BTNode
    {
        public override unsafe void Build(void* dataPtr) {}
        public override IBehaviorNode BehaviorNode => new ResetChildrenNode();
        public override int Size => 0;
    }
}
