using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("2F6009D3-1314-42E6-8E52-4AEB7CDDB4CD")]
    public static class DelayTimerNode
    {
        public static readonly int Id = typeof(DelayTimerNode).GetBehaviorNodeId();

        static DelayTimerNode()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }
        
        public struct Data : INodeData
        {
            public TimeSpan Target;
            public TimeSpan Current;
        }

        private static void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.Current = TimeSpan.Zero;
        }

        private static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            if (data.Current >= data.Target)
                return NodeState.Success;
            
            data.Current += ((TickDeltaTime)blackboard[typeof(TickDeltaTime)]).Value;
            return NodeState.Running;
        }
    }
}
