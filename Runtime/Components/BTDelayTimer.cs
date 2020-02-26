using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.Components
{
    public class BTDelayTimer : BTNode<DelayTimerNode>
    {
        public float DelayInSeconds;
        protected override void Build(ref DelayTimerNode data, ITreeNode<INodeDataBuilder>[] builders)
        {
            data.TimerSeconds = DelayInSeconds;
        }
    }
}
