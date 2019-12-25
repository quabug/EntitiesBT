using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTSequence : BTNode
    {
        public override IBehaviorNode BehaviorNode => new SequenceNode();
        public override unsafe int Size => sizeof(SuccessionNode.Data);
        public override unsafe void Build(void* dataPtr) {}
    }
}
