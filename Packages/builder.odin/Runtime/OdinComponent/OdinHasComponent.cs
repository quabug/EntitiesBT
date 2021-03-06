// automatically generate from `OdinNodeComponentTemplate.cs`
#if ODIN_INSPECTOR

using EntitiesBT.Core;
using Unity.Entities;
using Sirenix.Serialization;
using System;
using EntitiesBT.Variant;

namespace EntitiesBT.Components.Odin
{
    public class OdinHasComponent : OdinNode<EntitiesBT.Nodes.HasComponentNode>
    {
        public System.UInt64 StableTypeHash;
        protected override void Build(ref EntitiesBT.Nodes.HasComponentNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.StableTypeHash = StableTypeHash;
        }
    }
}

#endif
