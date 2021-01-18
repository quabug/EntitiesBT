// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;
using static EntitiesBT.Variant.Utilities;

namespace EntitiesBT.Components
{
    public class BTTimer : BTNode<EntitiesBT.Nodes.TimerNode>
    {
        public EntitiesBT.Variant.SingleSerializedReaderAndWriterVariant CountdownSeconds;

        public EntitiesBT.Core.NodeState BreakReturnState;
        protected override unsafe void Build(ref EntitiesBT.Nodes.TimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            if (CountdownSeconds.IsLinked)
            {
                CountdownSeconds.ReaderAndWriter.Allocate(ref builder, ref data.CountdownSeconds, Self, tree);
            }
            else
            {
                CountdownSeconds.Reader.Allocate(ref builder, ref data.CountdownSeconds.Reader, Self, tree);
                CountdownSeconds.Writer.Allocate(ref builder, ref data.CountdownSeconds.Writer, Self, tree);
            }
            data.BreakReturnState = BreakReturnState;
        }
    }
}
