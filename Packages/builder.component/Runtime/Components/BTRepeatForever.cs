// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Attributes;
using Unity.Entities;
using static EntitiesBT.Variant.Utilities;

namespace EntitiesBT.Components
{
    public class BTRepeatForever : BTNode<EntitiesBT.Nodes.RepeatForeverNode>
    {
        public EntitiesBT.Core.NodeState BreakStates;
        protected override unsafe void Build(ref EntitiesBT.Nodes.RepeatForeverNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.BreakStates = BreakStates;
        }
    }
}
