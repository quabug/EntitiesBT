using System;
using EntitiesBT.Core;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("4ABB8326-24A7-45FF-955A-A4DACB127C1B")]
    public struct ModifyPriorityNode : INodeData
    {
        public int PrioritySelectorIndex;
        public int WeightIndex;
        public int AddWeight;
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard _)
        {
            ref var data = ref blob.GetNodeData<ModifyPriorityNode>(index);
            if (data.PrioritySelectorIndex < 0) return NodeState.Failure;
            
            ref var prioritySelectorData = ref blob.GetNodeDefaultData<PrioritySelectorNode>(data.PrioritySelectorIndex);
            prioritySelectorData.Weights[data.WeightIndex] += data.AddWeight;
            return NodeState.Success;
        }
    }
}
