using EntitiesBT.Attributes;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.DebugView;
using EntitiesBT.Entities;
using EntitiesBT.Variant;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class BTVariantTest : BTNode<VariablesTestNode>
    {
        [SerializeReference, SerializeReferenceButton] public Int64VariantReader LongReader;
        public string String;
        public int[] IntArray;
        [SerializeReference, SerializeReferenceButton] public Int64VariantWriter LongWriter;
        [SerializeReference, SerializeReferenceButton] public SingleVariantReader SingleReader;
        public long LongValue;
        public SingleSerializedReaderAndWriterVariant SingleReaderAndWriter;

        protected override unsafe void Build(ref VariablesTestNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            LongReader.Allocate(ref builder, ref data.LongReader, this, tree);
            builder.AllocateString(ref data.String, String);
            builder.AllocateArray(ref data.IntArray, IntArray);
            LongWriter.Allocate(ref builder, ref data.LongWriter, this, tree);
            SingleReader.Allocate(ref builder, ref data.SingleReader, this, tree);
            data.Long = LongValue;
            SingleReaderAndWriter.Allocate(ref builder, ref data.SingleReaderAndWriter, this, tree);
        }
    }
    
    [BehaviorNode("867BFC14-4293-4D4E-B3F0-280AD4BAA403")]
    public struct VariablesTestNode : INodeData
    {
        [Optional] public BlobVariantReader<long> LongReader;
        public BlobString String;
        public BlobArray<int> IntArray;
        public BlobVariantWriter<long> LongWriter;
        public BlobVariantReader<float> SingleReader;
        public BlobVariantReaderAndWriter<float> SingleReaderAndWriter;
        public long Long;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            LongWriter.Write(index, ref blob, ref bb, (int)SingleReader.Read(index, ref blob, ref bb));
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {}
    }

    [BehaviorTreeDebugView(typeof(VariablesTestNode))]
    public class BlobStringDebugView : BTDebugView
    {
        public long LongVariable;
        public string String;

        public int[] IntArray;
        // public int IntVariable;
        public float FloatVariable;
        public long LongValue;

        public override void Tick()
        {
            var blob = Blob;
            var bb = Blackboard.Value;
            ref var data = ref blob.GetNodeData<VariablesTestNode, NodeBlobRef>(Index);
            LongVariable = data.LongReader.Read(Index, ref blob, ref bb);
            // IntVariable = data.DestVariable.Read(Index, ref blob, ref bb);
            FloatVariable = data.SingleReader.Read(Index, ref blob, ref bb);
            String = data.String.ToString();
            IntArray = data.IntArray.ToArray();
            LongValue = data.Long;
        }
    }
}
