using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("4790499A-D4D6-4998-BF53-043323162A7F", BehaviorNodeType.Composite)]
    public struct WeightRandomSelectorNode : INodeData
    {
        public float Sum;
        public BlobArray<float> Weights;

        [ReadWrite(typeof(BehaviorTreeRandom))]
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            if (blob.GetState(index) == NodeState.Running)
            {
                var childIndex = blob.FirstOrDefaultChildIndex(index, state => state == NodeState.Running);
                return childIndex != default ? VirtualMachine.Tick(childIndex, ref blob, ref blackboard) : 0;
            }
            
            ref var data = ref blob.GetNodeData<WeightRandomSelectorNode>(index);
            var rn = blackboard.GetDataRef<BehaviorTreeRandom>().Value.NextFloat(data.Sum);
            var weightIndex = 0;
            var currentWeightSum = 0f;
            foreach (var childIndex in blob.GetChildrenIndices(index))
            {
                currentWeightSum += data.Weights[weightIndex];
                if (rn < currentWeightSum) return VirtualMachine.Tick(childIndex, ref blob, ref blackboard);
                weightIndex++;
            }
            return 0;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}