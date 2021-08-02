// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Attributes;
using Unity.Entities;
using static EntitiesBT.Variant.Utilities;
using UnityEngine;
using System;

namespace EntitiesBT.Components
{
    [Obsolete("Use BTDynamicNode")]
    public class BTErrorRerunOnce : BTNode<EntitiesBT.Nodes.ErrorRerunOnceNode>
    {
        [Header("obsolete: use BTDynamicNode")]
        private int _;
        
        
        protected override void Build(ref EntitiesBT.Nodes.ErrorRerunOnceNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            
        }
    }
}
