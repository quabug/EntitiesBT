using System;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("46540F67-6145-4433-9A3A-E470992B952E", BehaviorNodeType.Decorate)]
    public struct TimerNode : INodeData
    {
        public float CountdownSeconds;
        public NodeState BreakReturnState;
        
        public static readonly ComponentType[] Types = { ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>() };

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var childState = blob.TickChildren(index, blackboard, state => state.IsCompleted()).FirstOrDefault();
            ref var data = ref blob.GetNodeData<TimerNode>(index);
            data.CountdownSeconds -= blackboard.GetData<BehaviorTreeTickDeltaTime>().Value;
            if (data.CountdownSeconds <= 0f) return childState.IsCompleted() ? childState : data.BreakReturnState;
            return NodeState.Running;
        }
    }
}
