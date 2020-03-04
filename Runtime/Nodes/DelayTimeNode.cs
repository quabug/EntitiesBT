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

        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>();
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<DelayTimerNode>(index);
            data.TimerSeconds -= bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            return data.TimerSeconds <= 0 ? NodeState.Success : NodeState.Running;
        }
    }
}
