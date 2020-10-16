using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Samples
{
    public class BTInputMoveToRotation : BTNode<InputMoveToRotationNode>
    {
        [SerializeReference, SerializeReferenceButton]
        public IFloat2PropertyReader InputMovePropertyReader;
        
        [SerializeReference, SerializeReferenceButton]
        public IQuaternionPropertyReader OutputDirectionPropertyReader;

        protected override void Build(ref InputMoveToRotationNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            InputMovePropertyReader.Allocate(ref builder, ref data.InputMove, this, tree);
            OutputDirectionPropertyReader.Allocate(ref builder, ref data.OutputDirection, this, tree);
        }
    }

    [BehaviorNode("2164B3CA-C12E-4C86-9F80-F45A99124FAD")]
    public struct InputMoveToRotationNode : INodeData
    {
        [ReadOnly] public BlobVariableReader<float2> InputMove;
        public BlobVariableReader<quaternion> OutputDirection;
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var move = InputMove.Read(index, ref blob, ref bb);
            if (math.lengthsq(move) <= math.FLT_MIN_NORMAL) return NodeState.Success;
            
            var direction = quaternion.LookRotationSafe(new float3(move.x, 0, move.y), math.up());
            OutputDirection.GetDataRef(index, ref blob, ref bb) = direction;
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
