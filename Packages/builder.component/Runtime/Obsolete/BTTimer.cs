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
    public class BTTimer : BTNode<EntitiesBT.Nodes.TimerNode>
    {
        [Header("obsolete: use BTDynamicNode")]
        private int _;
        
        public EntitiesBT.Variant.SerializedVariantRW<System.Single> CountdownSeconds;
        public EntitiesBT.Core.NodeState BreakReturnState;
        protected override void Build(ref EntitiesBT.Nodes.TimerNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            CountdownSeconds.Allocate(ref builder, ref data.CountdownSeconds, Self, tree);
            data.BreakReturnState = BreakReturnState;
        }
    }
}
