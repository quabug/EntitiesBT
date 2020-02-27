using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTDelayTimer : BTNode<DelayTimerNode>
    {
        public float DelayInSeconds;
        protected override void Build(ref DelayTimerNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            data.TimerSeconds = DelayInSeconds;
        }
    }
}
