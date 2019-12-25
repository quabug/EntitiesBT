using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public class BTDelayTimer : BTNode
    {
        public float DelayInSeconds;
        
        public override unsafe void Build(void* dataPtr) =>
            ((DelayTimerNode.Data*) dataPtr)->Value = TimeSpan.FromSeconds(DelayInSeconds);

        public override IBehaviorNode BehaviorNode =>
            new DelayTimerNode(() => TimeSpan.FromSeconds(Time.deltaTime));
        public override unsafe int Size => sizeof(DelayTimerNode.Data);
    }
}
