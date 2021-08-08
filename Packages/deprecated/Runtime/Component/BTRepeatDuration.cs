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
    public class BTRepeatDuration : BTNode<EntitiesBT.Nodes.RepeatDurationNode>
    {
        [Header("obsolete: use BTDynamicNode")]
        private int _;
        
        public System.Single CountdownSeconds;
        public EntitiesBT.Core.NodeState BreakStates;
        protected override void Build(ref EntitiesBT.Nodes.RepeatDurationNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.CountdownSeconds = CountdownSeconds;
            data.BreakStates = BreakStates;
        }
    }
}
