using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Components;
using EntitiesBT.Core;
using Runtime;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Builder")]
    [Serializable]
    public struct BehaviorTreeBuilder : IConstantNode
    {
        [PortDescription("", Runtime.ValueType.Entity)]
        public OutputDataPort BehaviorTreeEntity;

        [PortDescription("")]
        public OutputTriggerPort BehaviorTree;

        public void Execute<TCtx>(TCtx ctx) where TCtx : IGraphInstance
        {
        }
    }

    public static class BehaviorTreeBuilderExtension
    {
        public static GraphDefinition GetGraphDefinition<TCtx>(this TCtx ctx) where TCtx : IGraphInstance
        {
            if (!(ctx is GraphInstance instance)) return null; // TODO: throw?

            return (GraphDefinition)typeof(GraphInstance)
                .GetField("m_Definition", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(instance)
            ;
        }

        public static IEnumerable<INodeDataBuilder> ToBuilderNode(this OutputTriggerPort port, GraphDefinition definition)
        {
            var portIndex = (int)port.Port.Index;
            Assert.IsTrue(portIndex < definition.PortInfoTable.Count);
            var nodeIndex = (int)definition.PortInfoTable[portIndex].NodeId.GetIndex();
            Assert.IsTrue(nodeIndex < definition.NodeTable.Count);
            var childNode = definition.NodeTable[nodeIndex] as IVisualBuilderNode;
            Assert.IsNotNull(childNode);
            return childNode.GetBuilder(definition).Yield();
        }

        public static IEnumerable<INodeDataBuilder> ToBuilderNode(this OutputTriggerMultiPort ports, GraphDefinition definition)
        {
            return Enumerable.Range(0, ports.DataCount).SelectMany(i => ports.SelectPort((uint)i).ToBuilderNode(definition));
        }
    }

    public interface IVisualBuilderNode
    {
        INodeDataBuilder GetBuilder(GraphDefinition definition);
    }

    public class VisualBuilder<T> : INodeDataBuilder where T : struct, INodeData
    {
        public delegate void BuildImpl(BlobBuilder blobBuilder, ref T data, ITreeNode<INodeDataBuilder>[] builders);
        private readonly BuildImpl _buildImpl;

        public int NodeId => typeof(T).GetBehaviorNodeAttribute().Id;
        public INodeDataBuilder Self { get; }
        public IEnumerable<INodeDataBuilder> Children { get; }

        public VisualBuilder(BuildImpl buildImpl = null, Func<INodeDataBuilder, INodeDataBuilder> decoSelf = null, IEnumerable<INodeDataBuilder> children = null)
        {
            _buildImpl = buildImpl ?? BuildNothing;
            Self = decoSelf == null ? this : decoSelf(this);
            Children = children ?? Enumerable.Empty<INodeDataBuilder>();
            void BuildNothing(BlobBuilder blobBuilder, ref T data, ITreeNode<INodeDataBuilder>[] builders) {}
        }

        public VisualBuilder(BuildImpl buildImpl, IEnumerable<INodeDataBuilder> children)
            : this(buildImpl, null, children)
        {
        }

        public VisualBuilder(BuildImpl buildImpl, Func<INodeDataBuilder, INodeDataBuilder> decoSelf)
            : this(buildImpl, decoSelf, null)
        {
        }

        public BlobAssetReference Build(ITreeNode<INodeDataBuilder>[] builders)
        {
            var minSize = UnsafeUtility.SizeOf<T>();
            if (minSize == 0) return BlobAssetReference.Null;
            using (var blobBuilder = new BlobBuilder(Allocator.Temp, minSize))
            {
                ref var data = ref blobBuilder.ConstructRoot<T>();
                _buildImpl(blobBuilder, ref data, builders);
                return blobBuilder.CreateReference<T>();
            }
        }
    }



}