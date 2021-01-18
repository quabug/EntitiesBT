// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;
using static EntitiesBT.Variant.Utilities;

namespace EntitiesBT.Components
{
    public class BTDelayTimer : BTNode<EntitiesBT.Nodes.DelayTimerNode>
    {
        [UnityEngine.SerializeReference, SerializeReferenceButton] public EntitiesBT.Variant.SingleReaderAndWriterVariant TimerSeconds;

        protected override unsafe void Build(ref EntitiesBT.Nodes.DelayTimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            TimerSeconds.Allocate(ref builder, ref data.TimerSeconds, Self, tree);
        }
    }
}
