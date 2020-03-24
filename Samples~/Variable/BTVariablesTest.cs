using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.DebugView;
using EntitiesBT.Variable;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class BTVariablesTest : BTNode<VariablesTestNode>
    {
        [SerializeReference, SerializeReferenceButton] public Int64Property LongVariable;
        public string String;
        public int[] IntArray;
        [SerializeReference, SerializeReferenceButton] public Int32Property DestVariable;
        [SerializeReference, SerializeReferenceButton] public SingleProperty SrcVariable;
        public long LongValue;

        protected override void Build(ref VariablesTestNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            LongVariable.Allocate(ref builder, ref data.LongVariable, this, tree);
            builder.AllocateString(ref data.String, String);
            builder.AllocateArray(ref data.IntArray, IntArray);
            DestVariable.Allocate(ref builder, ref data.DestVariable, this, tree);
            SrcVariable.Allocate(ref builder, ref data.SrcVariable, this, tree);
            data.Long = LongValue;
        }
    }
    
    [BehaviorNode("867BFC14-4293-4D4E-B3F0-280AD4BAA403")]
    public struct VariablesTestNode : INodeData
    {
        [Optional] public BlobVariable<long> LongVariable;
        public BlobString String;
        public BlobArray<int> IntArray;
        [ReadWrite] public BlobVariable<int> DestVariable;
        [ReadOnly] public BlobVariable<float> SrcVariable;
        public long Long;

        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            DestVariable.GetDataRef(index, blob, bb) = (int)SrcVariable.GetData(index, blob, bb);
            return NodeState.Success;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard bb) {}
    }

    [BehaviorTreeDebugView(typeof(VariablesTestNode))]
    public class BlobStringDebugView : BTDebugView
    {
        public long LongVariable;
        public string String;
        public int[] IntArray;
        public int IntVariable;
        public float FloatVariable;
        public long LongValue;

        public override void Tick()
        {
            ref var data = ref Blob.GetNodeData<VariablesTestNode>(Index);
            LongVariable = data.LongVariable.GetData(Index, Blob, Blackboard);
            IntVariable = data.DestVariable.GetData(Index, Blob, Blackboard);
            FloatVariable = data.SrcVariable.GetData(Index, Blob, Blackboard);
            String = data.String.ToString();
            IntArray = data.IntArray.ToArray();
            LongValue = data.Long;
        }
    }
}
