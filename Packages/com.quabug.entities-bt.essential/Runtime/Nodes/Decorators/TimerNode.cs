using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Variant;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("46540F67-6145-4433-9A3A-E470992B952E", BehaviorNodeType.Decorate)]
    public struct TimerNode : INodeData
    {
        public BlobVariantRW<float> CountdownSeconds;
        public NodeState BreakReturnState;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var countdown = CountdownSeconds.Read(index, ref blob, ref bb);
            if (countdown <= 0f) return 0;

            var childState = index.TickChild(ref blob, ref bb);
            if (BreakReturnState.HasFlagFast(childState)) return childState;

            countdown -= bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            CountdownSeconds.Write(index, ref blob, ref bb, countdown);
            return countdown <= 0f ? NodeState.Success : NodeState.Running;
        }
    }
}
