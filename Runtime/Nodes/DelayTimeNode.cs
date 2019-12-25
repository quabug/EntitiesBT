using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class DelayTimerNode : IBehaviorNode
    {
        public struct Data : INodeData
        {
            public TimeSpan Value;
        }
        
        private TimeSpan _timer;
        private readonly Func<TimeSpan> _tickDelta;
        public DelayTimerNode(Func<TimeSpan> tickDelta)
        {
            _tickDelta = tickDelta;
        }
        
        public void Reset(VirtualMachine vm, int index)
        {
            _timer = TimeSpan.Zero;
        }

        public NodeState Tick(VirtualMachine vm, int index)
        {
            if (_timer >= vm.GetNodeData<Data>(index).Value)
                return NodeState.Success;
            
            _timer += _tickDelta();
            return NodeState.Running;
        }
    }
}
