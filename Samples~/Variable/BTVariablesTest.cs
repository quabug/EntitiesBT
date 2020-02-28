using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.DebugView;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT.Sample
{
    public class BTVariablesTest : BTNode<VariablesTestNode>
    {
        public Variable<long> LongVariable;
        public string String;
        public int[] IntArray;
        public Variable<int> IntVariable;
        public Variable<float> FloatVariable;

        protected override void Build(ref VariablesTestNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] builders)
        {
            builder.AllocateString(ref data.String, String);
            builder.AllocateArray(ref data.IntArray, IntArray);
            builder.AllocateVariable(ref data.LongVariable, LongVariable);
            builder.AllocateVariable(ref data.IntVariable, IntVariable);
            builder.AllocateVariable(ref data.FloatVariable, FloatVariable);
        }
    }

    [BehaviorNode("867BFC14-4293-4D4E-B3F0-280AD4BAA403")]
    public struct VariablesTestNode : INodeData
    {
        public BlobVariable<long> LongVariable;
        public BlobString String;
        public BlobArray<int> IntArray;
        public BlobVariable<int> IntVariable;
        public BlobVariable<float> FloatVariable;

        public static NodeState Tick(int index, INodeBlob blob, IBlackboard blackboard)
        {
            ref var data = ref blob.GetNodeData<VariablesTestNode>(index);
            data.IntVariable.SetData(index, blob, blackboard, (int)data.FloatVariable.GetData(index, blob, blackboard));
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
            IntVariable = data.IntVariable.GetData(Index, Blob, Blackboard);
            FloatVariable = data.FloatVariable.GetData(Index, Blob, Blackboard);
            String = data.String.ToString();
            IntArray = data.IntArray.ToArray();
        }
    }
}
