using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("76E27039-91C1-4DEF-AFEF-1EDDBAAE8CCE", BehaviorNodeType.Decorate)]
    public struct RepeatTimesNode : INodeData
    {
        public int TickTimes;
        public NodeState BreakStates;
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var childState = index.TickChild(ref blob, ref blackboard);
            if (childState == 0)
            {
                index.ResetChildren(ref blob, ref blackboard);
                childState = index.TickChild(ref blob, ref blackboard);
            }
            if (BreakStates.HasFlag(childState)) return childState;

            TickTimes--;
            return TickTimes <= 0 ? NodeState.Success : NodeState.Running;
        }
    }
}
