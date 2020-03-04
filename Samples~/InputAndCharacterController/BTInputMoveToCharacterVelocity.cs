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
    public class BTInputMoveToCharacterVelocity : BTNode<InputMoveToCharacterVelocityNode>
    {
        [SerializeReference, SerializeReferenceButton]
        public SingleProperty SpeedProperty;
        
        [SerializeReference, SerializeReferenceButton]
        public float2Property InputMoveProperty;
        
        [SerializeReference, SerializeReferenceButton]
        public float3Property OutputVelocityProperty;

        protected override void Build(ref InputMoveToCharacterVelocityNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            SpeedProperty.Allocate(ref builder, ref data.Speed, this, tree);
            InputMoveProperty.Allocate(ref builder, ref data.InputMove, this, tree);
            OutputVelocityProperty.Allocate(ref builder, ref data.OutputVelocity, this, tree);
        }
    }

    [BehaviorNode("B4559A1E-392B-4B8C-A074-B323AB31EEA7")]
    public struct InputMoveToCharacterVelocityNode : INodeData
    {
        public BlobVariable<float> Speed;
        public BlobVariable<float2> InputMove;
        public BlobVariable<float3> OutputVelocity;
        
        public static IEnumerable<ComponentType> AccessTypes(int index, INodeBlob blob)
        {
            ref var data = ref blob.GetNodeDefaultData<InputMoveToCharacterVelocityNode>(index);
            return data.Speed.ComponentAccessList
                .Concat(data.InputMove.ComponentAccessList)
                .Concat(data.OutputVelocity.ComponentAccessList)
            ;
        }
        
        public static NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blob.GetNodeData<InputMoveToCharacterVelocityNode>(index);
            var input = data.InputMove.GetData(index, blob, bb);
            var direction = new Vector3(input.x, 0, input.y).normalized;
            var speed = data.Speed.GetData(index, blob, bb);
            data.OutputVelocity.GetDataRef(index, blob, bb) = direction * speed;
            return NodeState.Success;
        }
    }
}
