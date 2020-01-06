using EntitiesBT.Nodes;

namespace EntitiesBT.Components
{
    public class BTDelayTimer : BTNode<DelayTimerNode, DelayTimerNode.Data>
    {
        public float DelayInSeconds;

        public override unsafe void Build(void* dataPtr) =>
            ((DelayTimerNode.Data*) dataPtr)->TargetSeconds = DelayInSeconds;
    }
}
