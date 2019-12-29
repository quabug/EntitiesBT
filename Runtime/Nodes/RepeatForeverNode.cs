using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("A13666BD-48E3-414A-BD13-5C696F2EA87E")]
    public static class RepeatForeverNode
    {
        public static readonly int Id = typeof(RepeatForeverNode).GetBehaviorNodeId();

        static RepeatForeverNode()
        {
            VirtualMachine.Register(Id, Reset, Tick);
        }
        
        public struct Data : INodeData
        {
            public NodeState BreakStates;
        }
        
        private static void Reset(int index, INodeBlob blob, IBlackboard blackboard) {}

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
