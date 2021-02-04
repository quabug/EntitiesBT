using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Variant;
using JetBrains.Annotations;
using Runtime;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Builder")]
    [Serializable]
    public struct BehaviorTreeBuilder : IDataNode
    {
        [PortDescription("", Runtime.ValueType.Entity)]
        public OutputDataPort BehaviorTreeEntity;

        [PortDescription("")]
        public OutputTriggerPort BehaviorTree;

        [PortDescription(Runtime.ValueType.StringReference)]
        public InputDataPort DebugName;

        public BehaviorTreeThread Thread;
        public AutoCreateType AutoCreation;

        public void Execute<TCtx>(TCtx ctx) where TCtx : IGraphInstance
        {
            var instance = ctx as GraphInstance;
            Assert.IsNotNull(instance);
            if (instance == null) return;

            var definition = ctx.GetGraphDefinition();
            var builder = BehaviorTree.ToBuilderNode(instance, definition).Single();
            var dstManager = ctx.EntityManager;
            var entity = dstManager.CreateEntity();
            var blob = new NodeBlobRef(builder.ToBlob());
            var bb = new EntityBlackboard { Entity = entity, EntityManager = dstManager };
            VirtualMachine.Reset(ref blob, ref bb);

            var query = blob.GetAccessTypes();
            var dataQuery = new BlackboardDataQuery(query, components => dstManager.CreateEntityQuery(components.ToArray()));
            dstManager.AddSharedComponentData(entity, dataQuery);
            dstManager.AddComponentData(entity, new BehaviorTreeComponent
            {
                Blob = blob, Thread = Thread, AutoCreation = AutoCreation
            });

            dstManager.AddComponentObject(entity, new GraphInstanceComponent {Value = instance});

#if UNITY_EDITOR
            var debugName = ctx.ReadString(DebugName);
            if (!string.IsNullOrEmpty(debugName)) dstManager.SetName(entity, debugName);
#endif

            ctx.Write(BehaviorTreeEntity, entity);
        }
    }

    public interface IVisualBuilderNode : INode
    {
        INodeDataBuilder GetBuilder([NotNull] GraphInstance instance, [NotNull] GraphDefinition definition);
    }

    public interface IVisualVariantNode : INode
    {
        IVariantReader<T> GetVariant<T>(int dataIndex, [NotNull] GraphInstance instance, [NotNull] GraphDefinition definition) where T : unmanaged;
    }

    public interface IVisualVariantNode<T> : INode where T : unmanaged
    {
        IVariantReader<T> GetVariant([NotNull] GraphInstance instance, [NotNull] GraphDefinition definition);
    }

    public readonly struct VisualBuilder<T> : INodeDataBuilder where T : unmanaged, INodeData
    {
        public delegate void BuildImpl(BlobBuilder blobBuilder, ref T data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders);
        private readonly BuildImpl _buildImpl;

        public int NodeId => typeof(T).GetBehaviorNodeAttribute().Id;
        public INodeDataBuilder Self => this;
        public IEnumerable<INodeDataBuilder> Children { get; }

        public VisualBuilder(BuildImpl buildImpl = null, IEnumerable<INodeDataBuilder> children = null)
        {
            _buildImpl = buildImpl ?? BuildNothing;
            Children = children ?? Enumerable.Empty<INodeDataBuilder>();
            void BuildNothing(BlobBuilder blobBuilder, ref T data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders) {}
        }

        public VisualBuilder(IEnumerable<INodeDataBuilder> children) : this(null, children) {}

        public BlobAssetReference Build(ITreeNode<INodeDataBuilder>[] builders)
        {
            var minSize = UnsafeUtility.SizeOf<T>();
            if (minSize == 0) return BlobAssetReference.Null;
            using (var blobBuilder = new BlobBuilder(Allocator.Temp, minSize))
            {
                ref var data = ref blobBuilder.ConstructRoot<T>();
                _buildImpl(blobBuilder, ref data, this, builders);
                return blobBuilder.CreateReference<T>();
            }
        }
    }



}