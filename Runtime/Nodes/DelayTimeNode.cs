using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public static class DelayTimerNode
    {
        public static int Id = 8;

        static DelayTimerNode()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }
        
        public struct Data : INodeData
        {
            public TimeSpan Target;
            public TimeSpan Current;
        }
        
        static void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.Current = TimeSpan.Zero;
        }

        static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            if (data.Current >= data.Target)
                return NodeState.Success;
            
            data.Current += ((TickDeltaTime)blackboard[typeof(TickDeltaTime)]).Value;
            return NodeState.Running;
        }
    }
}
