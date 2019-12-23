using System.Collections.Generic;
using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTSelector : BTNode
    {
        public override int Type => Factory.GetTypeId<SelectorNode>();
        public override int Size => 0;
        public override unsafe void Build(void* dataPtr) {}
    }
}
