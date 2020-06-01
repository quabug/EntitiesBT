using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("1A5C1ED4-E5C9-402B-8862-A6106F440CCE", BehaviorNodeType.Decorate)]
    public struct RepeatDurationNode : INodeData
    {
        public float CountdownSeconds;
        public NodeState BreakStates;

        [ReadOnly(typeof(BehaviorTreeTickDeltaTime))]
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var childState = index.TickChildrenReturnFirstOrDefault(ref blob, ref bb);
            if (childState == 0)
            {
                index.ResetChildren(ref blob, ref bb);
                childState = index.TickChildrenReturnFirstOrDefault(ref blob, ref bb);
            }
            if (BreakStates.HasFlag(childState)) return childState;

            CountdownSeconds -= bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            return CountdownSeconds <= 0 ? NodeState.Success : NodeState.Running;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
