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
        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var childState = blob.TickChildrenReturnFirstOrDefault(index, bb);
            if (childState == 0)
            {
                blob.ResetChildren(index, bb);
                childState = blob.TickChildrenReturnFirstOrDefault(index, bb);
            }
            if (BreakStates.HasFlag(childState)) return childState;

            CountdownSeconds -= bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            return CountdownSeconds <= 0 ? NodeState.Success : NodeState.Running;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}
