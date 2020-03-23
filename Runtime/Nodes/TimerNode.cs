using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("46540F67-6145-4433-9A3A-E470992B952E", BehaviorNodeType.Decorate)]
    public struct TimerNode : INodeData
    {
        public float CountdownSeconds;
        public NodeState BreakReturnState;

        public IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            yield return ComponentType.ReadOnly<BehaviorTreeTickDeltaTime>();
        }

        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            var childState = blob.TickChildren(index, blackboard, state => state.IsCompleted()).FirstOrDefault();
            CountdownSeconds -= blackboard.GetData<BehaviorTreeTickDeltaTime>().Value;
            if (CountdownSeconds <= 0f) return childState.IsCompleted() ? childState : BreakReturnState;
            return NodeState.Running;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}
