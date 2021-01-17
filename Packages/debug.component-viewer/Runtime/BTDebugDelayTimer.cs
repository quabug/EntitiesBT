using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Nodes;

namespace EntitiesBT.DebugView
{
    public class BTDebugDelayTimer : BTDebugView<DelayTimerNode>
    {
        public float DelayTime;

        public override void Tick()
        {
            if (!Blackboard.HasValue) return;

            var blob = Blob;
            var bb = Blackboard.Value;
            // DelayTime = blob.GetNodeData<DelayTimerNode, NodeBlobRef>(Index).TimerSecondsReader.Read(Index, ref blob, ref bb);
        }
    }
}