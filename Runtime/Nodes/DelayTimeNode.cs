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
        
        public void Reset(VirtualMachine vm, int index, IBlackboard blackboard)
        {
            ref var data = ref vm.GetNodeData<Data>(index);
            data.Current = TimeSpan.Zero;
        }

        public NodeState Tick(VirtualMachine vm, int index, IBlackboard blackboard)
        {
            ref var data = ref vm.GetNodeData<Data>(index);
            if (data.Current >= data.Target)
                return NodeState.Success;
            
            data.Current += ((TickDeltaTime)blackboard[typeof(TickDeltaTime)]).Value;
            return NodeState.Running;
        }
    }
}
