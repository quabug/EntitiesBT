using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blob;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;

namespace EntitiesBT.Entities
{
    public static class NodeBlobExtensions
    {
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

        [Pure]
        public static BlobAssetReference<NodeBlob> ToBlob(
            [NotNull] this INodeDataBuilder root
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
                new ArrayBuilderWithMemoryCopy<byte, Blob.BlobArray<byte>>(() => (treeBuilder.ArrayBuilder.PatchPosition, treeBuilder.ArrayBuilder.PatchSize)) { Alignment = treeBuilder.Alignment }
            );
            builder.SetArray(ref builder.Value.RuntimeGlobalValues, globalValuesStream.ToArray());
            return builder.CreateUnityBlobAssetReference();

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

        public static unsafe void SaveToStream(
            [NotNull] this INodeDataBuilder builder
            , [NotNull] IReadOnlyList<IGlobalValuesBuilder> scopeValuesList
            , [NotNull] Stream stream
        )
        {
            using var blob = builder.ToBlob(scopeValuesList);
            using var writer = new MemoryBinaryWriter();
            writer.Write(NodeBlob.VERSION);
            writer.Write(blob);
            var runtimePartSize = blob.Value.RuntimeSize;
            // HACK: truncate the runtime part of data (NodeState and RuntimeNodeData)
            var finalSize = writer.Length - runtimePartSize;
            using var writerData = new UnmanagedMemoryStream(writer.Data, finalSize);
            writerData.CopyTo(stream);
        }

        [Pure]
        public static Entity CloneBehaviorTree(this EntityManager dstManager, Entity behaviorTreeEntity)
        {
            var query = dstManager.GetSharedComponentData<BlackboardDataQuery>(behaviorTreeEntity);
            var comp = dstManager.GetComponentData<BehaviorTreeComponent>(behaviorTreeEntity);
            var clone = dstManager.CreateEntity();
#if UNITY_EDITOR
            dstManager.SetName(clone, dstManager.GetName(behaviorTreeEntity)+"-clone");
#endif
            dstManager.AddSharedComponentData(clone, query);
            var blob = new NodeBlobRef(comp.Blob.BlobRef.Clone());
            dstManager.AddComponentData(clone, new BehaviorTreeComponent(blob, comp.Thread, comp.AutoCreation));
            return clone;
        }

        [Pure]
        public static unsafe BlobAssetReference<T> Clone<T>(this BlobAssetReference<T> @this) where T : unmanaged
        {
            var untypedRef = BlobAssetReference.Create(@this);
            return BlobAssetReference<T>.Create(untypedRef.GetUnsafePtr(), untypedRef.Length);
        }
    }
}
