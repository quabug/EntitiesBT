using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Nodes;

namespace EntitiesBT.Components
{
    public class BTDelayTimer : BTNode<DelayTimerNode>
    {
        public Variable<float> DelayInSeconds;
        
        public override int Size => DelayInSeconds.BlobSize;

        protected override void Build(ref DelayTimerNode data, ITreeNode<INodeDataBuilder>[] builders)
        {
            data.TimerSeconds.FromVariableUnsafe(DelayInSeconds);
        }
    }
}
