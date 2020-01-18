using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("4790499A-D4D6-4998-BF53-043323162A7F", BehaviorNodeType.Composite)]
    public struct WeightRandomSelectorNode : INodeData
    {
        public float Sum;
        public SimpleBlobArray<float> Weights;
        public static int Size(int count) => SimpleBlobArray<float>.Size(count) + sizeof(float);
        
        public static readonly ComponentType[] Types = { ComponentType.ReadWrite<BehaviorTreeRandom>() };
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            if (blob.GetState(index) == NodeState.Running)
            {
                var childIndex = blob.GetChildrenIndices(index, state => state == NodeState.Running).FirstOrDefault();
                return childIndex != default ? VirtualMachine.Tick(childIndex, blob, blackboard) : 0;
            }
            
            ref var data = ref blob.GetNodeData<WeightRandomSelectorNode>(index);
            var rn = blackboard.GetDataRef<BehaviorTreeRandom>().Value.NextFloat(data.Sum);
            var weightIndex = 0;
            var currentWeightSum = 0f;
            foreach (var childIndex in blob.GetChildrenIndices(index))
            {
                currentWeightSum += data.Weights[weightIndex];
                if (rn < currentWeightSum) return VirtualMachine.Tick(childIndex, blob, blackboard);
                weightIndex++;
            }
            return 0;
        }
    }
}