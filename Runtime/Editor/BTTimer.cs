using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public class BTTimer : BTNode
    {
        public float TimeInSeconds;
        public NodeState BreakReturnState;
        
        public override IBehaviorNode BehaviorNode => 
            new TimerNode(() => TimeSpan.FromSeconds(Time.deltaTime));
        public override int Size => UnsafeUtility.SizeOf<TimerNode.Data>();
        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (TimerNode.Data*) dataPtr;
            ptr->Target = TimeSpan.FromSeconds(TimeInSeconds);
            ptr->BreakReturnState = BreakReturnState;
        }
    }
}
