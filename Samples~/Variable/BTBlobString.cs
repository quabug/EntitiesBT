using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.DebugView;
using Unity.Entities;

namespace EntitiesBT.Sample
{
    public class BTBlobString : BTNode<BlobStringNode>
    {
        public string A;
        public string B;

        protected override void Build(ref BlobStringNode data, ITreeNode<INodeDataBuilder>[] builders)
        {
            data.A.Length;
            data.B.FromStringUnsafe(B);
        }
    }

    [BehaviorNode("867BFC14-4293-4D4E-B3F0-280AD4BAA403")]
    public struct BlobStringNode : INodeData
    {
        public BlobString A;
        public BlobString B;
    }

    [BehaviorTreeDebugView(typeof(BlobStringNode))]
    public class BlobStringDebugView : BTDebugView
    {
        public string A;
        public string B;

        public override void Tick()
        {
            A = Blob.GetNodeData<BlobStringNode>(Index).A.ToString();
            B = Blob.GetNodeData<BlobStringNode>(Index).B.ToString();
        }
    }
}
