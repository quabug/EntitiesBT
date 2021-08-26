using System;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTDynamicNode : BTNode
    {
        public bool RunOnMainThread = false;
        public NodeAsset NodeData;

        protected override unsafe void Build(void* dataPtr, BlobBuilder blobBuilder, ITreeNode<INodeDataBuilder>[] builders)
        {
            NodeData.Build(blobBuilder, new IntPtr(dataPtr));
        }
    }
}