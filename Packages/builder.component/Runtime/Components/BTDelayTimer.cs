// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;
using static EntitiesBT.Variable.Utilities;

namespace EntitiesBT.Components
{
    public class BTDelayTimer : BTNode<EntitiesBT.Nodes.DelayTimerNode>
    {
        [UnityEngine.SerializeReference, SerializeReferenceButton] public EntitiesBT.Variable.ISinglePropertyReader TimerSecondsReader;
        [UnityEngine.SerializeReference, SerializeReferenceButton] public EntitiesBT.Variable.ISinglePropertyWriter TimerSecondsWriter;

        protected override void Build(ref EntitiesBT.Nodes.DelayTimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            TimerSecondsReader.Allocate(ref builder, ref data.TimerSecondsReader, Self, tree);
            TimerSecondsWriter.Allocate(ref builder, ref data.TimerSecondsWriter, Self, tree);
        }
    }
}
