// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTDelayTimer : BTNode<EntitiesBT.Nodes.DelayTimerNode>
    {
        [UnityEngine.SerializeReference, SerializeReferenceButton] public EntitiesBT.Variable.ISinglePropertyReader TimerSeconds;

        protected override void Build(ref EntitiesBT.Nodes.DelayTimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            TimerSeconds.Allocate(ref builder, ref data.TimerSeconds, Self, tree);
        }
    }
}
