using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public class RepeatTimesNode : IBehaviorNode
    {
        public struct Data : INodeData
        {
            public int TargetTimes;
            public int CurrentTimes;
            public NodeState BreakStates;
        }
        
        public void Reset(VirtualMachine vm, int index, IBlackboard blackboard)
        {
            ref var data = ref vm.GetNodeData<Data>(index);
            data.CurrentTimes = 0;
        }

        public NodeState Tick(VirtualMachine vm, int index, IBlackboard blackboard)
        {
            ref var data = ref vm.GetNodeData<Data>(index);
            var childIndex = index + 1;
            var endIndex = vm.EndIndex(index);
            while (childIndex < endIndex)
            {
                NodeState childState;
                try
                {
                    childState = vm.Tick(childIndex);
                } catch (IndexOutOfRangeException)
                {
                    // TODO: reset ticked children only?
                    for (var i = index + 1; i < endIndex; i++) vm.Reset(i);
                    childState = vm.Tick(childIndex);
                }
                if (data.BreakStates.HasFlag(childState)) return childState;
                childIndex = vm.EndIndex(childIndex);
            }
            data.CurrentTimes++;
            if (data.CurrentTimes == data.TargetTimes) return NodeState.Success;
            return NodeState.Running;
        }
    }
}
