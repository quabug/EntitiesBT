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
    public class OdinModifyPriority : OdinNode<EntitiesBT.Nodes.ModifyPriorityNode>
    {
        public System.Int32 PrioritySelectorIndex;
        public System.Int32 WeightIndex;
        public System.Int32 AddWeight;
        protected override void Build(ref EntitiesBT.Nodes.ModifyPriorityNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.PrioritySelectorIndex = PrioritySelectorIndex;
            data.WeightIndex = WeightIndex;
            data.AddWeight = AddWeight;
        }
    }
}

#endif
