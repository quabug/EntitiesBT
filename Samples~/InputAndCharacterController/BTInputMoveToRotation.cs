using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;
using Unity.Mathematics;

namespace EntitiesBT.Samples
{
    public class BTInputMoveToRotation : BTNode<InputMoveToRotationNode>
    {
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, System.NonSerialized]
#endif
        public VariableProperty<float2> InputMoveProperty;
        
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, System.NonSerialized]
#endif
        public VariableProperty<quaternion> OutputDirectionProperty;

        protected override void Build(ref InputMoveToRotationNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            InputMoveProperty.Allocate(ref builder, ref data.InputMove, this, tree);
            OutputDirectionProperty.Allocate(ref builder, ref data.OutputDirection, this, tree);
        }
    }

    [BehaviorNode("2164B3CA-C12E-4C86-9F80-F45A99124FAD")]
    public struct InputMoveToRotationNode : INodeData
    {
        [ReadOnly] public BlobVariable<float2> InputMove;
        public BlobVariable<quaternion> OutputDirection;
        
        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var move = InputMove.GetData(index, blob, bb);
            if (math.lengthsq(move) <= math.FLT_MIN_NORMAL) return NodeState.Success;
            
            var direction = quaternion.LookRotationSafe(new float3(move.x, 0, move.y), math.up());
            OutputDirection.GetDataRef(index, blob, bb) = direction;
            return NodeState.Success;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
}
