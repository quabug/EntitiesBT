using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("2F6009D3-1314-42E6-8E52-4AEB7CDDB4CD")]
    public struct DelayTimerNode : INodeData
    {
        public float TimerSeconds;

        [ReadOnly(typeof(BehaviorTreeTickDeltaTime))]
        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            TimerSeconds -= bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            return TimerSeconds <= 0 ? NodeState.Success : NodeState.Running;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}
