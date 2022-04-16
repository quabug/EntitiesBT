using System;
using System.Collections.Generic;
using System.Linq;
using Blob;
using JetBrains.Annotations;

namespace EntitiesBT.Core
{
    public class NodeDataBuilder : INodeDataBuilder
    {
        public IBuilder ValueBuilder { get; }
        public IReadOnlyList<ITreeNode> Children { get; }
        public int NodeId => NodeType.GetBehaviorNodeAttribute().Id;
        public int NodeIndex { get; set; }
        public Type NodeType { get; }
        public BehaviorNodeType BehaviorNodeType => NodeType.GetBehaviorNodeAttribute().Type;
        public virtual INodeDataBuilder Self => this;

        public NodeDataBuilder(Type nodeType, [NotNull] IBuilder builder, [NotNull] IEnumerable<ITreeNode> children)
        {
            ValueBuilder = builder;
            Children = children.ToArray();
            NodeType = nodeType;
        }
    }

    public class NodeDataBuilder<T> : INodeDataBuilder where T : unmanaged, INodeData
    {
        public IBuilder ValueBuilder { get; }
        public IReadOnlyList<ITreeNode> Children { get; }
        public int NodeId => NodeType.GetBehaviorNodeAttribute().Id;
        public int NodeIndex { get; set; }
        public Type NodeType => typeof(T);
        public BehaviorNodeType BehaviorNodeType => NodeType.GetBehaviorNodeAttribute().Type;
        public virtual INodeDataBuilder Self => this;

        public NodeDataBuilder() : this(new ValueBuilder<T>(), Enumerable.Empty<ITreeNode>()) {}
        public NodeDataBuilder(IEnumerable<ITreeNode> children) : this(new ValueBuilder<T>(), children) {}
        public NodeDataBuilder([NotNull] IBuilder builder, [NotNull] IEnumerable<ITreeNode> children)
        {
            ValueBuilder = builder;
            Children = children.ToArray();
        }
    }
}