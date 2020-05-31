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
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard _)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            if (PrioritySelectorIndex < 0) return NodeState.Failure;
            
            ref var prioritySelectorData = ref blob.GetNodeDefaultData<PrioritySelectorNode, TNodeBlob>(PrioritySelectorIndex);
            prioritySelectorData.Weights[WeightIndex] += AddWeight;
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
