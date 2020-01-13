using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.Components
{
    public class BTTimer : BTNode<TimerNode, TimerNode.Data>
    {
        public float TimeInSeconds;
        public NodeState BreakReturnState;

        protected override void Build(ref TimerNode.Data data, ITreeNode<INodeDataBuilder>[] builders)
        {
            data.CountdownSeconds = TimeInSeconds;
            data.BreakReturnState = BreakReturnState;
        }
    }
}
