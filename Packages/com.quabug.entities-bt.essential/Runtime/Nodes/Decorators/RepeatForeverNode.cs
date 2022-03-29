using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("A13666BD-48E3-414A-BD13-5C696F2EA87E", BehaviorNodeType.Decorate)]
    public struct RepeatForeverNode : INodeData
    {
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
            return BreakStates.HasFlagFast(childState) ? childState : NodeState.Running;
        }
    }
}
