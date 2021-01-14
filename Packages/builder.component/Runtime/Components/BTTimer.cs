// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;
using static EntitiesBT.Variant.Utilities;

namespace EntitiesBT.Components
{
    public class BTTimer : BTNode<EntitiesBT.Nodes.TimerNode>
    {
        [UnityEngine.SerializeReference, SerializeReferenceButton] public EntitiesBT.Variant.SingleReaderVariant CountdownSecondsReader;

        [UnityEngine.SerializeReference, SerializeReferenceButton] public EntitiesBT.Variant.SingleWriterVariant CountdownSecondsWriter;

        public EntitiesBT.Core.NodeState BreakReturnState;
        protected override void Build(ref EntitiesBT.Nodes.TimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            CountdownSecondsReader.Allocate(ref builder, ref data.CountdownSecondsReader, Self, tree);
            CountdownSecondsWriter.Allocate(ref builder, ref data.CountdownSecondsWriter, Self, tree);
            data.BreakReturnState = BreakReturnState;
        }
    }
}
