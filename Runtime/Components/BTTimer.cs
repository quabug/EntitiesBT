using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTTimer : BTNode<TimerNode>
    {
        public float TimeInSeconds;
        public NodeState BreakReturnState;

        protected override void Build(ref TimerNode data, BlobBuilder blobBuilder, ITreeNode<INodeDataBuilder>[] builders)
        {
            data.CountdownSeconds = TimeInSeconds;
            data.BreakReturnState = BreakReturnState;
        }
    }
}
