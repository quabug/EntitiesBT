using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.DebugView;
using EntitiesBT.Variable;
using EntitiesBT.Variable.Sample;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class BTVariablesTest : BTNode<VariablesTestNode>
    {
        [SerializeReferenceButton, SerializeReference] public Int64Property LongVariable;
        public string String;
        public int[] IntArray;
        [SerializeReferenceButton, SerializeReference] public Int32Property DestVariable;
        [SerializeReferenceButton, SerializeReference] public SingleProperty SrcVariable;

        protected override void Build(ref VariablesTestNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] builders)
        {
            LongVariable.Allocate(ref builder, ref data.LongVariable);
            builder.AllocateString(ref data.String, String);
            builder.AllocateArray(ref data.IntArray, IntArray);
            DestVariable.Allocate(ref builder, ref data.DestVariable);
            SrcVariable.Allocate(ref builder, ref data.SrcVariable);
        }
    }
    
    [BehaviorNode("867BFC14-4293-4D4E-B3F0-280AD4BAA403")]
    public struct VariablesTestNode : INodeData
    {
        public BlobVariable<long> LongVariable;
        public BlobString String;
        public BlobArray<int> IntArray;
        public BlobVariable<int> DestVariable;
        public BlobVariable<float> SrcVariable;

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<VariablesTestNode>(index);
            data.DestVariable.GetDataRef(index, blob, blackboard) = (int)data.SrcVariable.GetData(index, blob, blackboard);
            return NodeState.Success;
        }
    }

    [BehaviorTreeDebugView(typeof(VariablesTestNode))]
    public class BlobStringDebugView : BTDebugView
    {
        public long LongVariable;
        public string String;
        public int[] IntArray;
        public int IntVariable;
        public float FloatVariable;

        public override void Tick()
        {
            ref var data = ref Blob.GetNodeData<VariablesTestNode>(Index);
            LongVariable = data.LongVariable.GetData(Index, Blob, Blackboard);
            IntVariable = data.DestVariable.GetData(Index, Blob, Blackboard);
            FloatVariable = data.SrcVariable.GetData(Index, Blob, Blackboard);
            String = data.String.ToString();
            IntArray = data.IntArray.ToArray();
        }
    }
}
