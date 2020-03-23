using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("4ABB8326-24A7-45FF-955A-A4DACB127C1B")]
    public struct ModifyPriorityNode : INodeData
    {
        public int PrioritySelectorIndex;
        public int WeightIndex;
        public int AddWeight;
        
        public IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob) => Enumerable.Empty<ComponentType>();
        
        public NodeState Tick(int index, INodeBlob blob, IBlackboard _)
        {
            if (PrioritySelectorIndex < 0) return NodeState.Failure;
            
            ref var prioritySelectorData = ref blob.GetNodeDefaultData<PrioritySelectorNode>(PrioritySelectorIndex);
            prioritySelectorData.Weights[WeightIndex] += AddWeight;
            return NodeState.Success;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}
