using System;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [ExecuteAlways]
    public class BTDynamicNode : BTNode
    {
        public NodeAsset NodeData;
        public bool RunOnMainThread = false;

        protected override Type NodeType => Type.GetType(NodeData.NodeType ?? "") ?? typeof(ZeroNode);
        protected override INodeDataBuilder SelfImpl => RunOnMainThread ? new BTVirtualDecorator<RunOnMainThreadNode>(this) : (INodeDataBuilder) this;

        protected override unsafe void Build(void* dataPtr, BlobBuilder blobBuilder, ITreeNode<INodeDataBuilder>[] builders)
        {
            NodeData.Build(blobBuilder, new IntPtr(dataPtr));
        }
    }
}