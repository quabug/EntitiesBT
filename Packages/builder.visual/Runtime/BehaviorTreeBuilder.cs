using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Entities;
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

        public string DebugName;
        public BehaviorTreeThread Thread;
        public AutoCreateType AutoCreation;

        public void Execute<TCtx>(TCtx ctx) where TCtx : IGraphInstance
        {
            var definition = ctx.GetGraphDefinition();
            var builder = BehaviorTree.ToBuilderNode(definition);
            var dstManager = ctx.EntityManager;
            var entity = dstManager.CreateEntity();
            var blob = new NodeBlobRef(builder.ToBlob());
            var bb = new EntityBlackboard { Entity = entity, EntityManager = dstManager };
            VirtualMachine.Reset(ref blob, ref bb);

#if UNITY_EDITOR
            dstManager.SetName(entity, $"[BT]{DebugName}");
#endif
            var query = blob.GetAccessTypes();
            var dataQuery = new BlackboardDataQuery(query, components => dstManager.CreateEntityQuery(components.ToArray()));
            dstManager.AddSharedComponentData(entity, dataQuery);
            dstManager.AddComponentData(entity, new BehaviorTreeComponent
            {
                Blob = blob, Thread = Thread, AutoCreation = AutoCreation
            });

            ctx.Write(BehaviorTreeEntity, entity);
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

        public static INodeDataBuilder ToBuilderNode(this OutputTriggerPort port, GraphDefinition definition)
        {
            var portIndex = (int)port.Port.Index;
            Assert.IsTrue(portIndex < definition.PortInfoTable.Count);
            var nodeIndex = (int)definition.PortInfoTable[portIndex].NodeId.GetIndex();
            Assert.IsTrue(nodeIndex < definition.NodeTable.Count);
            var childNode = definition.NodeTable[nodeIndex] as IVisualBuilderNode;
            Assert.IsNotNull(childNode);
            return childNode.GetBuilder(definition);
        }

        public static IEnumerable<INodeDataBuilder> ToBuilderNode(this OutputTriggerMultiPort ports, GraphDefinition definition)
        {
            return Enumerable.Range(0, ports.DataCount).Select(i => ports.SelectPort((uint)i).ToBuilderNode(definition));
        }
    }

    public interface IVisualBuilderNode
    {
        INodeDataBuilder GetBuilder(GraphDefinition definition);
    }

    public readonly struct VisualBuilder<T> : INodeDataBuilder where T : struct, INodeData
    {
        public delegate void BuildImpl(BlobBuilder blobBuilder, ref T data, ITreeNode<INodeDataBuilder>[] builders);
        private readonly BuildImpl _buildImpl;

        public int NodeId => typeof(T).GetBehaviorNodeAttribute().Id;
        public INodeDataBuilder Self => this;
        public IEnumerable<INodeDataBuilder> Children { get; }

        public VisualBuilder(BuildImpl buildImpl = null, IEnumerable<INodeDataBuilder> children = null)
        {
            _buildImpl = buildImpl ?? BuildNothing;
            Children = children ?? Enumerable.Empty<INodeDataBuilder>();
            void BuildNothing(BlobBuilder blobBuilder, ref T data, ITreeNode<INodeDataBuilder>[] builders) {}
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