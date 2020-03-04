using System.Collections.Generic;
using System.Linq;
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
        public float2Property InputMoveProperty;
        
        [SerializeReference, SerializeReferenceButton]
        public quaternionProperty OutputDirectionProperty;

        protected override void Build(ref InputMoveToRotationNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            InputMoveProperty.Allocate(ref builder, ref data.InputMove, this, tree);
            OutputDirectionProperty.Allocate(ref builder, ref data.OutputDirection, this, tree);
        }
    }

    [BehaviorNode("2164B3CA-C12E-4C86-9F80-F45A99124FAD")]
    public struct InputMoveToRotationNode : INodeData
    {
        public BlobVariable<float2> InputMove;
        public BlobVariable<quaternion> OutputDirection;
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            ref var data = ref blob.GetNodeDefaultData<InputMoveToRotationNode>(index);
            return data.InputMove.ComponentAccessList
                .Concat(data.OutputDirection.ComponentAccessList)
            ;
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<InputMoveToRotationNode>(index);
            var move = data.InputMove.GetData(index, blob, bb);
            if (math.lengthsq(move) <= math.FLT_MIN_NORMAL) return NodeState.Success;
            
            var direction = quaternion.LookRotationSafe(new float3(move.x, 0, move.y), math.up());
            data.OutputDirection.GetDataRef(index, blob, bb) = direction;
            return NodeState.Success;
        }
    }
}
