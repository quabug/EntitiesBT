using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("1A5C1ED4-E5C9-402B-8862-A6106F440CCE", BehaviorNodeType.Decorate)]
    public struct RepeatDurationNode : INodeData
    {
        public float CountdownSeconds;
        public NodeState BreakStates;

        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>();
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var childState = blob.TickChildren(index, bb).FirstOrDefault();
            if (childState == 0)
            {
                blob.ResetChildren(index, bb);
                childState = blob.TickChildren(index, bb).FirstOrDefault();
            }
            ref var data = ref blob.GetNodeData<RepeatDurationNode>(index);
            if (data.BreakStates.HasFlag(childState)) return childState;

            data.CountdownSeconds -= bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            return data.CountdownSeconds <= 0 ? NodeState.Success : NodeState.Running;
        }
    }
}
