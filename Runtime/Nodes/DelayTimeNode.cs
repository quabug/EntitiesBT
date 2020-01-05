using System;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("2F6009D3-1314-42E6-8E52-4AEB7CDDB4CD")]
    public class DelayTimerNode
    {
        public static ComponentType[] Types => new []{ComponentType.ReadOnly<TickDeltaTime>()};
        
        public struct Data : INodeData
        {
            public TimeSpan Target;
            public TimeSpan Current;
        }

        public static void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.Current = TimeSpan.Zero;
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            if (data.Current >= data.Target)
                return NodeState.Success;
            
            data.Current += blackboard.GetData<TickDeltaTime>().Value;
            return NodeState.Running;
        }
    }
}
