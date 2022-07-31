using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blob;
using JetBrains.Annotations;

namespace EntitiesBT.Core
{
    public interface INodeDataBuilder
    {
        int NodeId { get; }
        int NodeIndex { get; set; }
        IBuilder BlobStreamBuilder { get; }
        IEnumerable<INodeDataBuilder> Children { get; }
    }

    public static partial class NodeDataBuilderExtension
    {
        public static Type GetNodeType(this INodeDataBuilder builder) => MetaNodeRegister.NODES[builder.NodeId].Type;
        public static BehaviorNodeAttribute GetBehaviorNodeAttribute(this INodeDataBuilder builder) => GetNodeType(builder).GetBehaviorNodeAttribute();
        public static BehaviorNodeType GetBehaviorNodeType(this INodeDataBuilder builder) => GetBehaviorNodeAttribute(builder).Type;

        public static IBuilder<NodeBlob> ToBuilder(
            this INodeDataBuilder root
            , IEnumerable<IGlobalValuesBuilder> globalValuesList
        )
        {
            var builder = new StructBuilder<NodeBlob>();
            var typeList = new List<int>();
            foreach (var node in root.Flatten())
            {
                node.NodeIndex = typeList.Count;
                typeList.Add(node.NodeId);
            }
            builder.SetArray(ref builder.Value.Types, typeList);
            var treeRoot = root.ToTreeNode();
            var treeBuilder = builder.SetTreeAny(ref builder.Value.Nodes, treeRoot);

            var globalValueOffset = 0;
            using var globalValuesStream = new MemoryStream();
            foreach (var value in globalValuesList)
            {
                value.Offset = globalValueOffset;
                globalValueOffset += value.Size;
                Write(globalValuesStream, value);
            }
            builder.SetArray(ref builder.Value.DefaultGlobalValues, globalValuesStream.ToArray());
            builder.SetArray(ref builder.Value.States, new NodeState[typeList.Count]);
            builder.SetBuilder(
                ref builder.Value.RuntimeDataBlob,
                new ArrayBuilderWithMemoryCopy<Blob.BlobArray<byte>>(treeBuilder.ArrayBuilder.DataBuilder)
            );
            builder.SetArray(ref builder.Value.RuntimeGlobalValues, globalValuesStream.ToArray());
            return builder;

            unsafe void Write(MemoryStream stream, IGlobalValuesBuilder value)
            {
#if UNITY_2021_2_OR_NEWER
                stream.Write(new System.ReadOnlySpan<byte>(value.ValuePtr.ToPointer(), value.Size));
#else
                for (var i = 0; i < value.Size; i++)
                {
                    var @byte = *(byte*)(value.ValuePtr + i).ToPointer();
                    stream.WriteByte(@byte);
                }
#endif
            }
        }
        
        private class TreeNode : ITreeNode
        {
            public IBuilder ValueBuilder { get; }
            public IReadOnlyList<ITreeNode> Children { get; }

            public TreeNode(IBuilder builder, IEnumerable<TreeNode> children)
            {
                ValueBuilder = builder;
                Children = children.ToArray();
            }
        }

        private static TreeNode ToTreeNode([NotNull] this INodeDataBuilder nodeBuilder) =>
            new TreeNode(nodeBuilder.BlobStreamBuilder, nodeBuilder.Children.Select(child => child.ToTreeNode()));

        private static IEnumerable<INodeDataBuilder> Flatten(this INodeDataBuilder node) =>
            node.Yield().Concat(node.Children.SelectMany(Flatten));
    }
}
