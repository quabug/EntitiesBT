using System;
using EntitiesBT.Core;
using EntitiesBT.Editor;

namespace EntitiesBT
{
    public class BTDelayTimer : BTNode
    {
        public float DelayInSeconds;
        
        public override int Type => Factory.GetTypeId<DelayTimerNode>();

        public override unsafe void Build(void* dataPtr) =>
            ((DelayTimerNode.Data*) dataPtr)->Value = TimeSpan.FromSeconds(DelayInSeconds);

        public override unsafe int Size => sizeof(DelayTimerNode.Data);
    }
    
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
        
        public void Initialize(VirtualMachine vm, int index) {}

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
