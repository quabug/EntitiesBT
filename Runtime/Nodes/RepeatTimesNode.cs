using System;
using System.Linq;
using EntitiesBT.Core;
using UnityEngine.UIElements;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("76E27039-91C1-4DEF-AFEF-1EDDBAAE8CCE", BehaviorNodeType.Decorate)]
    public class RepeatTimesNode
    {
        [Serializable]
        public struct Data : INodeData
        {
            public int TickTimes;
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

            data.TickTimes--;
            return data.TickTimes <= 0 ? NodeState.Success : NodeState.Running;
        }
    }
}
