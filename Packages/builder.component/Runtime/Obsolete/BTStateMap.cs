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
    public class BTStateMap : BTNode<EntitiesBT.Nodes.StateMapNode>
    {
        [Header("obsolete: use BTDynamicNode")]
        private int _;
        
        public EntitiesBT.Core.NodeState MapSuccess;
        public EntitiesBT.Core.NodeState MapFailure;
        public EntitiesBT.Core.NodeState MapRunning;
        public EntitiesBT.Core.NodeState MapError;
        protected override void Build(ref EntitiesBT.Nodes.StateMapNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.MapSuccess = MapSuccess;
            data.MapFailure = MapFailure;
            data.MapRunning = MapRunning;
            data.MapError = MapError;
        }
    }
}
