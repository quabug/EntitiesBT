using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.DebugView
{
    public class BTDebugDelayTimer : BTDebugView<DelayTimerNode>
    {
        public float Countdown;
        
        public override void Tick()
        {
            base.Tick();
            Countdown = RuntimeData.TimerSeconds.GetData<float>(Blackboard);
        }
        
        protected override void OnValidate()
        {
            if (!IsValid) return;
            Blob.GetNodeData<DelayTimerNode>(Index).TimerSeconds.SetData(Blackboard, Countdown);
        }
    }
}
