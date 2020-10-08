using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Variable;
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

#if UNITY_EDITOR
            var debugName = ctx.ReadString(DebugName);
            if (!string.IsNullOrEmpty(debugName)) dstManager.SetName(entity, debugName);
#endif

            ctx.Write(BehaviorTreeEntity, entity);
        }
    }

    public static class BehaviorTreeBuilderExtension
    {
        [Pure]
        public static GraphDefinition GetGraphDefinition<TCtx>([NotNull] this TCtx ctx) where TCtx : IGraphInstance
        {
            if (!(ctx is GraphInstance instance)) return null; // TODO: throw?

            return (GraphDefinition)typeof(GraphInstance)
                .GetField("m_Definition", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(instance)
            ;
        }

        [Pure]
        public static IEnumerable<INodeDataBuilder> ToBuilderNode(this OutputTriggerPort port, [NotNull] GraphInstance instance, [NotNull] GraphDefinition definition)
        {
            var outputPortIndex = (int)port.Port.Index;
            Assert.IsTrue(outputPortIndex < definition.PortInfoTable.Count);
            var triggerIndex = (int)definition.PortInfoTable[outputPortIndex].DataIndex;
            Assert.IsTrue(triggerIndex < definition.TriggerTable.Count);
            foreach (int inputPortIndex in definition.TriggerTable
                .Skip(triggerIndex)
                .TakeWhile(portIndex => portIndex != 0)
            ) {
                Assert.IsTrue(inputPortIndex < definition.PortInfoTable.Count);
                var nodeIndex = (int)definition.PortInfoTable[inputPortIndex].NodeId.GetIndex();
                Assert.IsTrue(nodeIndex < definition.NodeTable.Count);
                var childNode = definition.NodeTable[nodeIndex] as IVisualBuilderNode;
                Assert.IsNotNull(childNode);
                yield return childNode.GetBuilder(instance, definition);
            }
        }

        [Pure]
        public static IEnumerable<INodeDataBuilder> ToBuilderNode(this OutputTriggerMultiPort ports, [NotNull] GraphInstance instance, [NotNull] GraphDefinition definition)
        {
            return Enumerable.Range(0, ports.DataCount).SelectMany(i => ports.SelectPort((uint)i).ToBuilderNode(instance, definition));
        }

        [Pure]
        public static unsafe IVariableProperty<T> ToVariableProperty<T>(this InputDataPort port, [NotNull] GraphInstance instance, [NotNull] GraphDefinition definition) where T : unmanaged
        {
            var inputPortIndex = (int)port.Port.Index;
            Assert.IsTrue(inputPortIndex < definition.PortInfoTable.Count);
            var dataIndex = (int)definition.PortInfoTable[inputPortIndex].DataIndex;
            Assert.IsTrue(dataIndex < definition.DataPortTable.Count);
            var nodeIndex = (int)definition.DataPortTable[dataIndex].GetIndex();
            Assert.IsTrue(nodeIndex < definition.NodeTable.Count);
            var dataNode = definition.NodeTable[nodeIndex];

            if (dataNode is IVisualVariablePropertyNode propertyNode)
                return propertyNode.GetVariableProperty<T>(dataIndex, instance, definition);

            if (dataNode is IVisualVariablePropertyNode<T> genericPropertyNode)
                return genericPropertyNode.GetVariableProperty(instance, definition);

            T data;
            void* ptr = &data;
            var value = instance.ReadValue(port);
            Value.SetPtrToValue(ptr, value.Type, value);
            return new CustomVariableProperty<T> { CustomValue = data };
        }

        // copy from `GraphInstance.GetComponentFieldDescription`
        public static FieldDescription? GetComponentFieldDescription(this GraphDefinition definition, TypeReference componentType, int fieldIndex)
        {
            int index = Array.BinarySearch(definition.ComponentFieldDescriptions, new GraphDefinition.TypeFieldsDescription(componentType.TypeHash, null), GraphDefinition.TypeFieldsDescription.TypeHashComparer);
            if (index >= 0)
            {
                var fieldDescriptions = definition.ComponentFieldDescriptions[index].Fields;
                if (fieldIndex >= 0 && fieldIndex < fieldDescriptions.Count)
                {
                    return fieldDescriptions[fieldIndex];
                }
            }
            return null;
        }
    }

    public interface IVisualBuilderNode : IFlowNode
    {
        INodeDataBuilder GetBuilder([NotNull] GraphInstance instance, [NotNull] GraphDefinition definition);
    }

    public interface IVisualVariablePropertyNode : INode
    {
        IVariableProperty<T> GetVariableProperty<T>(int dataIndex, [NotNull] GraphInstance instance, [NotNull] GraphDefinition definition) where T : unmanaged;
    }

    public interface IVisualVariablePropertyNode<T> : INode where T : unmanaged
    {
        IVariableProperty<T> GetVariableProperty([NotNull] GraphInstance instance, [NotNull] GraphDefinition definition);
    }

    public readonly struct VisualBuilder<T> : INodeDataBuilder where T : struct, INodeData
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