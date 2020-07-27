using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Variable;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("46540F67-6145-4433-9A3A-E470992B952E", BehaviorNodeType.Decorate)]
    public struct TimerNode : INodeData
    {
        [ReadWrite] public BlobVariable<float> CountdownSeconds;
        public NodeState BreakReturnState;

        [ReadOnly(typeof(BehaviorTreeTickDeltaTime))]
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var countdown = ref CountdownSeconds.GetDataRef(index, ref blob, ref bb);
            if (countdown <= 0f) return 0;

            var childState = index.TickChild(ref blob, ref bb);
            if (BreakReturnState.HasFlagFast(childState)) return childState;

            countdown -= bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            return countdown <= 0f ? NodeState.Success : NodeState.Running;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
