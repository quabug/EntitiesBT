using System;
using EntitiesBT.Attributes;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    public class BTDynamicNode : BTNode
    {
        [SerializeReference, SerializeReferenceDrawer, OnValueChanged(nameof(OnNodeChanged))]
        public ISerializableNodeData NodeData;

        protected override Type NodeType => NodeData?.NodeType ?? typeof(ZeroNode);

        protected override unsafe void Build(void* dataPtr, BlobBuilder blobBuilder, ITreeNode<INodeDataBuilder>[] builders)
        {
            NodeData?.Build(dataPtr, blobBuilder, Self, builders);
        }

        private void OnNodeChanged()
        {
            name = NodeData?.NodeType?.Name ?? "[Null]";
        }
    }
}