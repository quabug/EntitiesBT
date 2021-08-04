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
    public class OdinPrioritySelector : OdinNode<EntitiesBT.Nodes.PrioritySelectorNode>
    {
        public System.Int32[] Weights;
        protected override void Build(ref EntitiesBT.Nodes.PrioritySelectorNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            builder.AllocateArray(ref data.Weights, Weights);
        }
    }
}

#endif
