using EntitiesBT.Nodes;

namespace EntitiesBT.Components
{
    public class BTDelayTimer : BTNode<DelayTimerNode, DelayTimerNode.Data>
    {
        public float DelayInSeconds;
        protected override void Build(ref DelayTimerNode.Data data)
        {
            data.TimerSeconds = DelayInSeconds;
        }
    }
}
