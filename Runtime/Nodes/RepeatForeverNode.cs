using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    public static class RepeatForeverNode
    {
        public static int Id = 4;

        static RepeatForeverNode()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }
        
        public struct Data : INodeData
        {
            public NodeState BreakStates;
        }
        
        public static void Reset(int index, INodeBlob blob, IBlackboard blackboard) {}

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
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
                    // TODO: reset ticked node only?
                    for (var i = index + 1; i < endIndex; i++) VirtualMachine.Reset(i, blob, blackboard);
                    childState = VirtualMachine.Tick(childIndex, blob, blackboard);
                }
                if (data.BreakStates.HasFlag(childState))
                    return childState;
                childIndex = blob.GetEndIndex(childIndex);
            }
            return NodeState.Running;
        }
    }
}
