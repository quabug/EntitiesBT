using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine.UIElements;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("76E27039-91C1-4DEF-AFEF-1EDDBAAE8CCE", BehaviorNodeType.Decorate)]
    public struct RepeatTimesNode : INodeData
    {
        public int TickTimes;
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
            ref var data = ref blob.GetNodeData<RepeatTimesNode>(index);
            if (data.BreakStates.HasFlag(childState)) return childState;

            data.TickTimes--;
            return data.TickTimes <= 0 ? NodeState.Success : NodeState.Running;
        }
    }
}
