// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Attributes;
using Unity.Entities;
using static EntitiesBT.Variant.Utilities;

namespace EntitiesBT.Components
{
    public class BTTimer : BTNode<EntitiesBT.Nodes.TimerNode>
    {
        public EntitiesBT.Variant.SingleSerializedReaderAndWriterVariant CountdownSeconds;
        public EntitiesBT.Core.NodeState BreakReturnState;
        protected override void Build(ref EntitiesBT.Nodes.TimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            CountdownSeconds.Allocate(ref builder, ref data.CountdownSeconds, Self, tree);
            data.BreakReturnState = BreakReturnState;
        }
    }
}
