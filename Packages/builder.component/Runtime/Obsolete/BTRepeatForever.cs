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
    public class BTRepeatForever : BTNode<EntitiesBT.Nodes.RepeatForeverNode>
    {
        [Header("obsolete: use BTDynamicNode")]
        private int _;
        
        public EntitiesBT.Core.NodeState BreakStates;
        protected override void Build(ref EntitiesBT.Nodes.RepeatForeverNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.BreakStates = BreakStates;
        }
    }
}
