using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("57BB1429-1ECC-431D-BD8C-6A70FDD14516", BehaviorNodeType.Composite)]
    public struct PrioritySelectorNode : INodeData
    {
        public BlobArray<int> Weights;
        
        public NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            if (blob.GetState(index) == NodeState.Running)
            {
                var childIndex = blob.FirstOrDefaultChildIndex(index, state => state == NodeState.Running);
                return childIndex != default ? VirtualMachine.Tick(childIndex, blob, blackboard) : 0;
            }

            var weightIndex = 0;
            var currentMaxWeight = int.MinValue;
            var maxChildIndex = 0;
            foreach (var childIndex in blob.GetChildrenIndices(index))
            {
                var weight = Weights[weightIndex];
                if (weight > currentMaxWeight)
                {
                    maxChildIndex = childIndex;
                    currentMaxWeight = weight;
                }

                weightIndex++;
            }

            return VirtualMachine.Tick(maxChildIndex, blob, blackboard);
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}