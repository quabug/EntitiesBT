using System;
using EntitiesBT.Core;
using EntitiesBT.Editor;

namespace EntitiesBT
{
    public class BTRepeat : BTNode
    {
        public int RepeatTimes;
        public NodeState BreakStates;

        public override int Type => RepeatTimes <= 0
            ? Factory.GetTypeId<RepeatForeverNode>()
            : Factory.GetTypeId<RepeatTimesNode>()
        ;
        
        public override unsafe int Size => RepeatTimes <= 0
            ? sizeof(RepeatForeverNode.Data)
            : sizeof(RepeatTimesNode.Data)
        ;
        
        public override unsafe void Build(void* dataPtr)
        {
            if (RepeatTimes <= 0)
            {
                var ptr = (RepeatForeverNode.Data*) dataPtr;
                ptr->BreakStates = BreakStates;
            }
            else
            {
                var ptr = (RepeatTimesNode.Data*) dataPtr;
                ptr->TargetTimes = RepeatTimes;
                ptr->BreakStates = BreakStates;
            }
        }
    }

    public class RepeatTimesNode : IBehaviorNode
    {
        public struct Data : INodeData
        {
            public int TargetTimes;
            public int CurrentTimes;
            public NodeState BreakStates;
        }
        
        public void Reset(VirtualMachine vm, int index)
        {
            ref var data = ref vm.GetNodeData<Data>(index);
            data.CurrentTimes = 0;
        }

        public NodeState Tick(VirtualMachine vm, int index)
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
    
    public class RepeatForeverNode : IBehaviorNode
    {
        public struct Data : INodeData
        {
            public NodeState BreakStates;
        }
        
        public void Reset(VirtualMachine vm, int index) {}

        public NodeState Tick(VirtualMachine vm, int index)
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
                    // TODO: reset ticked node only?
                    for (var i = index + 1; i < endIndex; i++) vm.Reset(i);
                    childState = vm.Tick(childIndex);
                }
                if (data.BreakStates.HasFlag(childState))
                    return childState;
                childIndex = vm.EndIndex(childIndex);
            }
            return NodeState.Running;
        }
    }
}
