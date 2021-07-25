using EntitiesBT.Attributes;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Sample;
using EntitiesBT.Variant;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using SingleVariantReader = EntitiesBT.Sample.SingleVariantReader;

namespace EntitiesBT.Samples
{
    public class BTInputMoveToCharacterVelocity : BTNode<InputMoveToCharacterVelocityNode>
    {
        [SerializeReference, SerializeReferenceDrawer]
        public SingleVariantReader SpeedPropertyReader;
        
        [SerializeReference, SerializeReferenceDrawer]
        public float2VariantReader InputMovePropertyReader;
        
        public float3SerializedReaderAndWriterVariant OutputVelocityPropertyWriter;

        protected override unsafe void Build(ref InputMoveToCharacterVelocityNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            SpeedPropertyReader.Allocate(ref builder, ref data.Speed, this, tree);
            InputMovePropertyReader.Allocate(ref builder, ref data.InputMove, this, tree);
            OutputVelocityPropertyWriter.Allocate(ref builder, ref data.OutputVelocity, this, tree);
        }
    }

    [BehaviorNode("B4559A1E-392B-4B8C-A074-B323AB31EEA7")]
    public struct InputMoveToCharacterVelocityNode : INodeData
    {
        public BlobVariantReader<float> Speed;
        public BlobVariantReader<float2> InputMove;
        public BlobVariantReaderAndWriter<float3> OutputVelocity;
        
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var input = InputMove.Read(index, ref blob, ref bb);
            var direction = new Vector3(input.x, 0, input.y).normalized;
            var speed = Speed.Read(index, ref blob, ref bb);
            OutputVelocity.Write(index, ref blob, ref bb, direction * speed);
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
