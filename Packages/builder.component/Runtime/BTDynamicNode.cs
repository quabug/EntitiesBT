using System;
using System.Collections.Generic;
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
        public override IBuilder ValueBuilder => NodeData.Builder;
        public override IReadOnlyList<ITreeNode> Children => RunOnMainThread ?
            new[] { new DecoratorBuilder<RunOnMainThreadNode>(new NodeDataBuilder(NodeType, ValueBuilder, base.Children)) } :
            base.Children
        ;
    }
}