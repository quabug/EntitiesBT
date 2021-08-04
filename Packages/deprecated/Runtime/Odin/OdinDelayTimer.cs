// automatically generate from `OdinNodeComponentTemplate.cs`
#if ODIN_INSPECTOR

using EntitiesBT.Core;
using Unity.Entities;
using Sirenix.Serialization;
using System;
using EntitiesBT.Variant;
using Sirenix.OdinInspector;

namespace EntitiesBT.Components.Odin
{
    public class OdinDelayTimer : OdinNode<EntitiesBT.Nodes.DelayTimerNode>
    {
        [OdinSerializeAttribute, NonSerializedAttribute, HideReferenceObjectPickerAttribute]
        public EntitiesBT.Components.Odin.OdinSerializedVariantReaderAndWriter<System.Single> TimerSeconds
            = new EntitiesBT.Components.Odin.OdinSerializedVariantReaderAndWriter<System.Single>();

        protected override void Build(ref EntitiesBT.Nodes.DelayTimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            TimerSeconds.Allocate(ref builder, ref data.TimerSeconds, Self, tree);
        }
    }
}

#endif
