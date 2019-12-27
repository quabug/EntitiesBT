using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class DelayTimerNode : IBehaviorNode
    {
        public struct Data : INodeData
        {
            public TimeSpan Target;
            public TimeSpan Current;
        }
        
        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.Current = TimeSpan.Zero;
        }

        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            if (data.Current >= data.Target)
                return NodeState.Success;
            
            data.Current += ((TickDeltaTime)blackboard[typeof(TickDeltaTime)]).Value;
            return NodeState.Running;
        }
    }
}
