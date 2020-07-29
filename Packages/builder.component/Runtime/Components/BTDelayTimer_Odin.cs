// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTDelayTimer_Odin : BTNode<EntitiesBT.Nodes.DelayTimerNode>
    {
        
#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize, System.NonSerialized]
        public EntitiesBT.Variable.VariableProperty<System.Single> TimerSeconds;
#endif

        protected override void Build(ref EntitiesBT.Nodes.DelayTimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            
#if ODIN_INSPECTOR
            TimerSeconds.Allocate(ref builder, ref data.TimerSeconds, Self, tree);
#endif

        }
    }
}
