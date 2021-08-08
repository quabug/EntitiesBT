using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Nodes
{
    [BehaviorNode("4790499A-D4D6-4998-BF53-043323162A7F", BehaviorNodeType.Composite)]
    public struct WeightRandomSelectorNode : INodeData
    {
        public BlobArray<float> NormalizedWeights;

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
                var rn = blackboard.GetDataRef<BehaviorTreeRandom>().Value.NextFloat();
                var weightIndex = 0;
                var currentWeightSum = 0f;

                var endIndex = blob.GetEndIndex(index);
                var childIndex = index + 1;
                while (childIndex < endIndex)
                {
                    currentWeightSum += NormalizedWeights[weightIndex];
                    if (rn < currentWeightSum) return VirtualMachine.Tick(childIndex, ref blob, ref blackboard);
                    weightIndex++;
                    childIndex = blob.GetEndIndex(childIndex);
                }
                return 0;
            }
        }

        public class Serializable : SerializableNodeData<WeightRandomSelectorNode>
        {
            public int[] Weights;

            protected override void Build(ref WeightRandomSelectorNode data, BlobBuilder builder, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
            {
                float sum = Weights.Sum();
                builder.AllocateArray(ref data.NormalizedWeights, Weights.Select(w => w / sum).ToArray());
            }
        }
    }
}