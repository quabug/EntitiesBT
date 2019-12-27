using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class TimerNode : IBehaviorNode
    {
        private readonly VirtualMachine _vm;

        public struct Data : INodeData
        {
            public TimeSpan Target;
            public TimeSpan Current;
            public NodeState ChildState;
            public NodeState BreakReturnState;
        }
        
        public TimerNode(VirtualMachine vm)
        {
            _vm = vm;
        }
        
        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.Current = TimeSpan.Zero;
            data.ChildState = NodeState.Running;
        }

        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);

            if (data.Current >= data.Target)
                return data.ChildState == NodeState.Running ? data.BreakReturnState : data.ChildState;
            
            var childIndex = index + 1;
            if (data.ChildState == NodeState.Running && childIndex < blob.GetEndIndex(index))
            {
                var childState = _vm.Tick(childIndex, blob, blackboard);
                data.ChildState = childState;
            }
            data.Current += ((TickDeltaTime)blackboard[typeof(TickDeltaTime)]).Value;
            return NodeState.Running;
        }
    }
}
