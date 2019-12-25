using System;
using EntitiesBT.Nodes;

namespace EntitiesBT.Editor
{
    public class BTDelayTimer : BTNode
    {
        public float DelayInSeconds;
        
        public override int Type => Factory.GetTypeId<DelayTimerNode>();

        public override unsafe void Build(void* dataPtr) =>
            ((DelayTimerNode.Data*) dataPtr)->Value = TimeSpan.FromSeconds(DelayInSeconds);

        public override unsafe int Size => sizeof(DelayTimerNode.Data);
    }
}
