using System;
using Blob;
using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.Components
{
    public class BTDynamicNode : BTNode
    {
        public NodeAsset NodeData;
        public bool RunOnMainThread = false;

        protected override Type NodeType => Type.GetType(NodeData.NodeType ?? "") ?? typeof(ZeroNode);
        protected override INodeDataBuilder SelfImpl => RunOnMainThread ? new BTVirtualDecorator<RunOnMainThreadNode>(this) : (INodeDataBuilder) this;

        protected override void Build(IBlobStream stream, ITreeNode<INodeDataBuilder>[] tree)
        {
            NodeData.Build(stream);
        }
    }
}