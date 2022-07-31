using System;
using System.Collections.Generic;
using Blob;

namespace EntitiesBT.Core
{
    public class DecoratorNode<TNode, TBuilder> : INodeDataBuilder
        where TNode : unmanaged, INodeData
        where TBuilder : IBuilder<TNode>, new()
    {
        private readonly INodeDataBuilder _child;
        public TBuilder Builder { get; }

        public int NodeId => NodeType.GetBehaviorNodeAttribute().Id;
        public int NodeIndex { get; set; }
        public IBuilder BlobStreamBuilder => Builder;
        public IEnumerable<INodeDataBuilder> Children => _child.Yield();
        public Type NodeType => typeof(TNode);
        public BehaviorNodeType BehaviorNodeType => NodeType.GetBehaviorNodeAttribute().Type;

        public DecoratorNode(INodeDataBuilder child) : this(child, new TBuilder()) {}
        public DecoratorNode(INodeDataBuilder child, TBuilder builder)
        {
            _child = child;
            Builder = builder;
        }
    }

    public class DecoratorNode<TNode> : DecoratorNode<TNode, ValueBuilder<TNode>> where TNode : unmanaged, INodeData
    {
        public DecoratorNode(INodeDataBuilder child) : base(child) {}
        public DecoratorNode(INodeDataBuilder child, ValueBuilder<TNode> builder) : base(child, builder) {}
    }
}