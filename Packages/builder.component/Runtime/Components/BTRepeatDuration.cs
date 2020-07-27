// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTRepeatDuration : BTNode<EntitiesBT.Nodes.RepeatDurationNode>
    {
        public System.Single CountdownSeconds;
        public EntitiesBT.Core.NodeState BreakStates;
        protected override void Build(ref EntitiesBT.Nodes.RepeatDurationNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.CountdownSeconds = CountdownSeconds;
            data.BreakStates = BreakStates;
        }
    }
}
