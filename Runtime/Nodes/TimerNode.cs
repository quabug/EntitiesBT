using System;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("46540F67-6145-4433-9A3A-E470992B952E", BehaviorNodeType.Decorate)]
    public class TimerNode
    {
        public static ComponentType[] Types => new [] { ComponentType.ReadOnly<TickDeltaTime>() };
        
        public struct Data : INodeData
        {
            public TimeSpan Target;
            public TimeSpan Current;
            public NodeState ChildState;
            public NodeState BreakReturnState;
        }

        public static void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);
            data.Current = TimeSpan.Zero;
            data.ChildState = NodeState.Running;
        }

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<Data>(index);

            if (data.Current >= data.Target)
                return data.ChildState == NodeState.Running ? data.BreakReturnState : data.ChildState;
            
            var childIndex = index + 1;
            if (data.ChildState == NodeState.Running && childIndex < blob.GetEndIndex(index))
            {
                var childState = VirtualMachine.Tick(childIndex, blob, blackboard);
                data.ChildState = childState;
            }
            data.Current += blackboard.GetData<TickDeltaTime>().Value;
            return NodeState.Running;
        }
    }
}
