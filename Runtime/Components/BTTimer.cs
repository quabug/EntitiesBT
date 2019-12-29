using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTTimer : BTNode<TimerNode, TimerNode.Data>
    {
        public float TimeInSeconds;
        public NodeState BreakReturnState;

        public override unsafe void Build(void* dataPtr)
        {
            var ptr = (TimerNode.Data*) dataPtr;
            ptr->Target = TimeSpan.FromSeconds(TimeInSeconds);
            ptr->BreakReturnState = BreakReturnState;
        }
    }
}
