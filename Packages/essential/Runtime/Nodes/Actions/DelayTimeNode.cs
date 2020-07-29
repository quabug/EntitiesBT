using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Variable;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("2F6009D3-1314-42E6-8E52-4AEB7CDDB4CD")]
    public struct DelayTimerNode : INodeData
    {
        [ReadWrite] public BlobVariable<float> TimerSeconds;

        [ReadOnly(typeof(BehaviorTreeTickDeltaTime))]
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var timer = ref TimerSeconds.GetDataRef(index, ref blob, ref bb);
            timer -= bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            return timer <= 0 ? NodeState.Success : NodeState.Running;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
