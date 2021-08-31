// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;
using static EntitiesBT.Variant.Utilities;
using UnityEngine;
using System;

namespace EntitiesBT.Components
{
    [Obsolete("Use BTDynamicNode")]
    public class BTHasComponent : BTNode<EntitiesBT.Nodes.HasComponentNode>
    {
        [Header("obsolete: use BTDynamicNode")]
        private int _;
        
        public System.UInt64 StableTypeHash;
        protected override void Build(ref EntitiesBT.Nodes.HasComponentNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.StableTypeHash = StableTypeHash;
        }
    }
}
