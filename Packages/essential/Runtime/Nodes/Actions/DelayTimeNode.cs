using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Variant;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("2F6009D3-1314-42E6-8E52-4AEB7CDDB4CD")]
    public struct DelayTimerNode : INodeData
    {
        public BlobVariantReader<float> TimerSecondsReader;
        public BlobVariantWriter<float> TimerSecondsWriter;

        [ReadOnly(typeof(BehaviorTreeTickDeltaTime))]
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var timer = TimerSecondsReader.Read(index, ref blob, ref bb);
            timer -= bb.GetData<BehaviorTreeTickDeltaTime>().Value;
            TimerSecondsWriter.Write(index, ref blob, ref bb, timer);
            return timer <= 0 ? NodeState.Success : NodeState.Running;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
