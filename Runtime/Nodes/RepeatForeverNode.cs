using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("A13666BD-48E3-414A-BD13-5C696F2EA87E", BehaviorNodeType.Decorate)]
    public struct RepeatForeverNode : INodeData
    {
        public NodeState BreakStates;
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob) => Enumerable.Empty<ComponentType>();
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var childState = blob.TickChildren(index, blackboard).FirstOrDefault();
            if (childState == 0)
            {
                blob.ResetChildren(index, blackboard);
                childState = blob.TickChildren(index, blackboard).FirstOrDefault();
            }
            ref var data = ref blob.GetNodeData<RepeatForeverNode>(index);
            return data.BreakStates.HasFlagFast(childState) ? childState : NodeState.Running;
        }
    }
}
