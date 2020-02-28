using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.DebugView;
using EntitiesBT.Entities;
using EntitiesBT.Variable;
using Unity.Entities;

namespace EntitiesBT.Sample
{
    public class BTVariablesTest : BTNode<VariablesTestNode>
    {
        public VariableProperty<long> LongVariable;
        public string String;
        public int[] IntArray;
        public VariableProperty<int> DestVariable;
        public VariableProperty<float> SrcVariable;

        protected override void Build(ref VariablesTestNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] builders)
        {
            builder.AllocateString(ref data.String, String);
            builder.AllocateArray(ref data.IntArray, IntArray);
            builder.AllocateVariable(ref data.LongVariable, LongVariable);
            builder.AllocateVariable(ref data.DestVariable, DestVariable);
            builder.AllocateVariable(ref data.SrcVariable, SrcVariable);
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
            data.DestVariable.SetData(index, blob, blackboard, (int)data.SrcVariable.GetData(index, blob, blackboard));
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
