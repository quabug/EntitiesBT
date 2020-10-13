// automatically generate from `OdinNodeComponentTemplate.cs`
#if ODIN_INSPECTOR

using EntitiesBT.Core;
using Unity.Entities;
using Sirenix.Serialization;
using System;

namespace EntitiesBT.Components
{
    public class OdinDelayTimer : OdinNode<EntitiesBT.Nodes.DelayTimerNode>
    {
        [OdinSerialize, NonSerialized]
        public EntitiesBT.Variable.VariableProperty<System.Single> TimerSeconds;

        protected override void Build(ref EntitiesBT.Nodes.DelayTimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            TimerSeconds.Allocate(ref builder, ref data.TimerSeconds, Self, tree);
        }
    }
}

#endif
