// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTRepeatForever : BTNode<EntitiesBT.Nodes.RepeatForeverNode>
    {
        public EntitiesBT.Core.NodeState BreakStates;
        protected override void Build(ref EntitiesBT.Nodes.RepeatForeverNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.BreakStates = BreakStates;
        }
    }
}
