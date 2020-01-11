using System;
using System.Linq;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("A13666BD-48E3-414A-BD13-5C696F2EA87E", BehaviorNodeType.Decorate)]
    public class RepeatForeverNode
    {
        [Serializable]
        public struct Data : INodeData
        {
            public NodeState BreakStates;
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var childState = blob.TickChildren(index, blackboard).FirstOrDefault();
            if (childState == 0)
            {
                blob.ResetChildren(index, blackboard);
                childState = blob.TickChildren(index, blackboard).FirstOrDefault();
            }
            ref var data = ref blob.GetNodeData<Data>(index);
            if (data.BreakStates.HasFlag(childState)) return childState;
            
            return NodeState.Running;
        }
    }
}
