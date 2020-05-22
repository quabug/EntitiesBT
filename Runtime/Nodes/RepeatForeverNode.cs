using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("A13666BD-48E3-414A-BD13-5C696F2EA87E", BehaviorNodeType.Decorate)]
    public struct RepeatForeverNode : INodeData
    {
        public NodeState BreakStates;
        
        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var childState = blob.TickChildrenReturnFirstOrDefault(index, blackboard);
            if (childState == 0)
            {
                blob.ResetChildren(index, blackboard);
                childState = blob.TickChildrenReturnFirstOrDefault(index, blackboard);
            }
            return BreakStates.HasFlagFast(childState) ? childState : NodeState.Running;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}
