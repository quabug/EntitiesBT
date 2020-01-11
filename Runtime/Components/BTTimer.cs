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

        protected override void Build(ref TimerNode.Data data)
        {
            data.Seconds = TimeInSeconds;
            data.BreakReturnState = BreakReturnState;
        }
    }
}
