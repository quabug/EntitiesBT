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
    public class OdinWeightRandomSelector : OdinNode<EntitiesBT.Nodes.WeightRandomSelectorNode>
    {
        public System.Single Sum;
        public System.Single[] Weights;
        protected override void Build(ref EntitiesBT.Nodes.WeightRandomSelectorNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.Sum = Sum;
            builder.AllocateArray(ref data.Weights, Weights);
        }
    }
}

#endif
