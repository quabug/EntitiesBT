using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("2F6009D3-1314-42E6-8E52-4AEB7CDDB4CD")]
    public struct DelayTimerNode : INodeData
    {
        public BlobVariable TimerSeconds;
        
        public static readonly ComponentType[] Types = {ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>()};

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<DelayTimerNode>(index);
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            ref var timerSeconds = ref data.TimerSeconds.GetData<float>(bb);
            timerSeconds -= deltaTime;
            return timerSeconds <= 0 ? NodeState.Success : NodeState.Running;
        }
    }
}
