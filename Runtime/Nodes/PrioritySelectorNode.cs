using System;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("57BB1429-1ECC-431D-BD8C-6A70FDD14516", BehaviorNodeType.Composite)]
    public struct PrioritySelectorNode : INodeData
    {
        public BlobArray<int> Weights;
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            if (blob.GetState(index) == NodeState.Running)
            {
                var childIndex = blob.FirstOrDefaultChildIndex(index, state => state == NodeState.Running);
                return childIndex != default ? VirtualMachine.Tick(childIndex, ref blob, ref blackboard) : 0;
            }
            else
            {
                var weightIndex = 0;
                var currentMaxWeight = int.MinValue;
                var maxChildIndex = 0;
            
                var endIndex = blob.GetEndIndex(index);
                var childIndex = index + 1;
                while (childIndex < endIndex)
                {
                    var weight = Weights[weightIndex];
                    if (weight > currentMaxWeight)
                    {
                        maxChildIndex = childIndex;
                        currentMaxWeight = weight;
                    }
                    weightIndex++;
                    childIndex = blob.GetEndIndex(childIndex);
                }

                return VirtualMachine.Tick(maxChildIndex, ref blob, ref blackboard);
            }
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}