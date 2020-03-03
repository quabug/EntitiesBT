using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("57BB1429-1ECC-431D-BD8C-6A70FDD14516", BehaviorNodeType.Composite)]
    public struct PrioritySelectorNode : INodeData
    {
        public BlobArray<int> Weights;
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob) => Enumerable.Empty<ComponentType>();

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            if (blob.GetState(index) == NodeState.Running)
            {
                var childIndex = blob.GetChildrenIndices(index, state => state == NodeState.Running).FirstOrDefault();
                return childIndex != default ? VirtualMachine.Tick(childIndex, blob, blackboard) : 0;
            }

            ref var data = ref blob.GetNodeData<PrioritySelectorNode>(index);
            var weightIndex = 0;
            var currentMaxWeight = int.MinValue;
            var maxChildIndex = 0;
            foreach (var childIndex in blob.GetChildrenIndices(index))
            {
                var weight = data.Weights[weightIndex];
                if (weight > currentMaxWeight)
                {
                    maxChildIndex = childIndex;
                    currentMaxWeight = weight;
                }

                weightIndex++;
            }

            return VirtualMachine.Tick(maxChildIndex, blob, blackboard);
        }
    }
}