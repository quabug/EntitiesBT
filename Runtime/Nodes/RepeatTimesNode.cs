using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class RepeatTimesNode : IBehaviorNode
    {
        private readonly VirtualMachine _vm;

        public struct Data : INodeData
        {
            public int TargetTimes;
            public int CurrentTimes;
            public NodeState BreakStates;
        }
        
        public RepeatTimesNode(VirtualMachine vm)
        {
            _vm = vm;
        }
        
        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.CurrentTimes = 0;
        }

        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            var childIndex = index + 1;
            var endIndex = blob.GetEndIndex(index);
            while (childIndex < endIndex)
            {
                NodeState childState;
                try
                {
                    childState = _vm.Tick(childIndex, blob, blackboard);
                } catch (IndexOutOfRangeException)
                {
                    // TODO: reset ticked children only?
                    for (var i = index + 1; i < endIndex; i++) _vm.Reset(i, blob, blackboard);
                    childState = _vm.Tick(childIndex, blob, blackboard);
                }
                if (data.BreakStates.HasFlag(childState)) return childState;
                childIndex = blob.GetEndIndex(childIndex);
            }
            data.CurrentTimes++;
            if (data.CurrentTimes == data.TargetTimes) return NodeState.Success;
            return NodeState.Running;
        }
    }
}
