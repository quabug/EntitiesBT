using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("46540F67-6145-4433-9A3A-E470992B952E", BehaviorNodeType.Decorate)]
    public struct TimerNode : INodeData
    {
        public float CountdownSeconds;
        public NodeState BreakReturnState;

        [ReadOnly(typeof(BehaviorTreeTickDeltaTime))]
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            if (CountdownSeconds <= 0f) return 0;

            var childState = index.TickChild(ref blob, ref blackboard);
            if (BreakReturnState.HasFlagFast(childState)) return childState;

            CountdownSeconds -= blackboard.GetData<BehaviorTreeTickDeltaTime>().Value;
            return CountdownSeconds <= 0f ? NodeState.Success : NodeState.Running;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
