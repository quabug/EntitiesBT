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
        public override IBuilder BlobStreamBuilder => NodeData.Builder;
        public override INodeDataBuilder Node => RunOnMainThread ? _mainThreadNodeBuilder : base.Node;

        private readonly DecoratorNode<RunOnMainThreadNode> _mainThreadNodeBuilder;

        public BTDynamicNode()
        {
            _mainThreadNodeBuilder = new DecoratorNode<RunOnMainThreadNode>(base.Node);
        }
    }
}