using EntitiesBT.Core;
using EntitiesBT.Nodes;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public class BTSelector : BTNode<SequenceNode>
    {
        public override int Size => 0;
        public override unsafe void Build(void* dataPtr) {}
    }
}
