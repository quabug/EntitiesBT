using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("2F6009D3-1314-42E6-8E52-4AEB7CDDB4CD")]
    public class DelayTimerNode
    {
        public static readonly ComponentType[] Types = {ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>()};
        
        [Serializable]
        public struct Data : INodeData
        {
            public float TimerSeconds;
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.TimerSeconds -= bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            return data.TimerSeconds <= 0 ? NodeState.Success : NodeState.Running;
        }
    }
}
