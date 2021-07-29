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
    public class BTRepeatTimes : BTNode<EntitiesBT.Nodes.RepeatTimesNode>
    {
        [Header("obsolete: use BTDynamicNode")]
        private int _;
        
        public System.Int32 TickTimes;
        public EntitiesBT.Core.NodeState BreakStates;
        protected override void Build(ref EntitiesBT.Nodes.RepeatTimesNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.TickTimes = TickTimes;
            data.BreakStates = BreakStates;
        }
    }
}
