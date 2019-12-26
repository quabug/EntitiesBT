using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTDelayTimer : BTNode
    {
        public float DelayInSeconds;
        
        public override unsafe void Build(void* dataPtr) =>
            ((DelayTimerNode.Data*) dataPtr)->Target = TimeSpan.FromSeconds(DelayInSeconds);

        public override IBehaviorNode BehaviorNode => new DelayTimerNode();
        public override unsafe int Size => sizeof(DelayTimerNode.Data);
    }
}
