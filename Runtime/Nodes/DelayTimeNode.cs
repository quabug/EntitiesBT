using System;
using EntitiesBT.Core;
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
            public float TargetSeconds;
            public float CurrentSeconds;
        }

        public static void Reset(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.CurrentSeconds = 0;
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            if (data.CurrentSeconds >= data.TargetSeconds)
                return NodeState.Success;
            data.CurrentSeconds += bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            return NodeState.Running;
        }
    }
}
