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
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, System.NonSerialized]
#endif
        public VariableProperty<float> SpeedProperty;
        
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, System.NonSerialized]
#endif
        public VariableProperty<float2> InputMoveProperty;
        
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, System.NonSerialized]
#endif
        public VariableProperty<float3> OutputVelocityProperty;

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
        [ReadOnly] public BlobVariable<float> Speed;
        [ReadOnly] public BlobVariable<float2> InputMove;
        public BlobVariable<float3> OutputVelocity;
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var input = InputMove.GetData(index, blob, bb);
            var direction = new Vector3(input.x, 0, input.y).normalized;
            var speed = Speed.GetData(index, blob, bb);
            OutputVelocity.GetDataRef(index, blob, bb) = direction * speed;
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
