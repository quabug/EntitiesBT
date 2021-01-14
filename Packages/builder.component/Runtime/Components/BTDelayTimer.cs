// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;
using static EntitiesBT.Variant.Utilities;

namespace EntitiesBT.Components
{
    public class BTDelayTimer : BTNode<EntitiesBT.Nodes.DelayTimerNode>
    {
        [UnityEngine.SerializeReference, SerializeReferenceButton] public EntitiesBT.Variant.SingleReaderVariant TimerSecondsReader;

        [UnityEngine.SerializeReference, SerializeReferenceButton] public EntitiesBT.Variant.SingleWriterVariant TimerSecondsWriter;

        protected override void Build(ref EntitiesBT.Nodes.DelayTimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            TimerSecondsReader.Allocate(ref builder, ref data.TimerSecondsReader, Self, tree);
            TimerSecondsWriter.Allocate(ref builder, ref data.TimerSecondsWriter, Self, tree);
        }
    }
}
