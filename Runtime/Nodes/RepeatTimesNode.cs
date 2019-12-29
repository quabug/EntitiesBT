using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("76E27039-91C1-4DEF-AFEF-1EDDBAAE8CCE")]
    public static class RepeatTimesNode
    {
        public static readonly int Id = typeof(RepeatTimesNode).GetBehaviorNodeId();
        
        static RepeatTimesNode()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }
        
        public struct Data : INodeData
        {
            public int TargetTimes;
            public int CurrentTimes;
            public NodeState BreakStates;
        }

        private static void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.CurrentTimes = 0;
        }

        private static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            var childIndex = index + 1;
            var endIndex = blob.GetEndIndex(index);
            while (childIndex < endIndex)
            {
                NodeState childState;
                try
                {
                    childState = VirtualMachine.Tick(childIndex, blob, blackboard);
                } catch (IndexOutOfRangeException)
                {
                    // TODO: reset ticked children only?
                    for (var i = index + 1; i < endIndex; i++) VirtualMachine.Reset(i, blob, blackboard);
                    childState = VirtualMachine.Tick(childIndex, blob, blackboard);
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
