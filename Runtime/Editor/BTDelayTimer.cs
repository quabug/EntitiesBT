using System;
using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTDelayTimer : BTNode<DelayTimerNode, DelayTimerNode.Data>
    {
        public float DelayInSeconds;
        
        public override unsafe void Build(void* dataPtr) =>
            ((DelayTimerNode.Data*) dataPtr)->Target = TimeSpan.FromSeconds(DelayInSeconds);
    }
}
