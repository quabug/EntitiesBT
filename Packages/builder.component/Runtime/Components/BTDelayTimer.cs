// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTDelayTimer : BTNode<EntitiesBT.Nodes.DelayTimerNode>
    {
        
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, System.NonSerialized]
        public EntitiesBT.Variable.VariableProperty<System.Single> TimerSeconds;
#else
        [UnityEngine.SerializeReference, SerializeReferenceButton]
        public EntitiesBT.Variable.SingleProperty TimerSeconds;
#endif

        protected override void Build(ref EntitiesBT.Nodes.DelayTimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            TimerSeconds.Allocate(ref builder, ref data.TimerSeconds, Self, tree);
        }
    }
}
