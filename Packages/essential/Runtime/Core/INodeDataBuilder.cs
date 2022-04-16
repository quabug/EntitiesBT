using System;
using System.Collections.Generic;
using Blob;
using JetBrains.Annotations;

namespace EntitiesBT.Core
{
    public interface INodeDataBuilder
    {
        int NodeId { get; }
        int NodeIndex { get; set; }
        ITreeNode Node { get; }
    }

    public static partial class NodeDataBuilderExtension
    {
        public static Type GetNodeType(this INodeDataBuilder builder) => MetaNodeRegister.NODES[builder.NodeId].Type;
        public static BehaviorNodeAttribute GetBehaviorNodeAttribute(this INodeDataBuilder builder) => GetNodeType(builder).GetBehaviorNodeAttribute();
        public static BehaviorNodeType GetBehaviorNodeType(this INodeDataBuilder builder) => GetBehaviorNodeAttribute(builder).Type;
    }
}
