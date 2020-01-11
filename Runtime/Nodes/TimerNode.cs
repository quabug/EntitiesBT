using System;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("46540F67-6145-4433-9A3A-E470992B952E", BehaviorNodeType.Decorate)]
    public class TimerNode
    {
        public static readonly ComponentType[] Types = { ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>() };
        
        [Serializable]
        public struct Data : INodeData
        {
            public float Seconds;
            public NodeState BreakReturnState;
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var childState = blob.TickChildren(index, blackboard, state => state.IsCompleted()).FirstOrDefault();
            ref var data = ref blob.GetNodeData<Data>(index);
            data.Seconds -= blackboard.GetData<BehaviorTreeTickDeltaTime>().Value;
            if (data.Seconds <= 0f) return childState.IsCompleted() ? childState : data.BreakReturnState;
            return NodeState.Running;
        }
    }
}
