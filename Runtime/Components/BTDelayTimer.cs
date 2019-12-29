using System;
using EntitiesBT.Nodes;

namespace EntitiesBT.Components
{
    public class BTDelayTimer : BTNode<DelayTimerNode.Data>
    {
        public float DelayInSeconds;

        public override int NodeId => DelayTimerNode.Id;

        public override unsafe void Build(void* dataPtr) =>
            ((DelayTimerNode.Data*) dataPtr)->Target = TimeSpan.FromSeconds(DelayInSeconds);
    }
}
