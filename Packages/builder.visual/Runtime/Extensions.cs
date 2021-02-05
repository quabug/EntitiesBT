using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using JetBrains.Annotations;
using Runtime;
using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;

namespace EntitiesBT.Builder.Visual
{
    public readonly struct DataPortReaderAndWriter
    {
        public readonly bool IsLinked;
        public readonly InputDataPort Input;
        public readonly OutputDataPort Output;

        public DataPortReaderAndWriter(bool isLinked, InputDataPort input, OutputDataPort output)
        {
            IsLinked = isLinked;
            Input = input;
            Output = output;
        }
    }

    public readonly struct SerializedVisualReaderAndWriter<T> : ISerializedVariantReaderAndWriter<T> where T : unmanaged
    {
        public bool IsLinked { get; }
        public IVariantReaderAndWriter<T> ReaderAndWriter { get; }
        public IVariantReader<T> Reader { get; }
        public IVariantWriter<T> Writer { get; }

        public SerializedVisualReaderAndWriter(bool isLinked, IVariantReaderAndWriter<T> readerAndWriter, IVariantReader<T> reader, IVariantWriter<T> writer)
        {
            IsLinked = isLinked;
            ReaderAndWriter = readerAndWriter;
            Reader = reader;
            Writer = writer;
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
        public static IVariantReader<T> ToVariantReader<T>(this InputDataPort port, [NotNull] GraphInstance instance, [NotNull] GraphDefinition definition) where T : unmanaged
        {
            return ToVariantReader(port, instance, definition, () => new GraphVariant.Reader<T>(port));
        }

        [Pure]
        public static IVariantWriter<T> ToVariantWriter<T>(this OutputDataPort port, [NotNull] GraphInstance instance, [NotNull] GraphDefinition definition) where T : unmanaged
        {
            throw new NotImplementedException();
        }

        [Pure]
        public static ISerializedVariantReaderAndWriter<T> ToVariantReaderAndWriter<T>(this in DataPortReaderAndWriter port, [NotNull] GraphInstance instance, [NotNull] GraphDefinition definition) where T : unmanaged
        {
            if (!port.IsLinked) throw new NotImplementedException();
            return new SerializedVisualReaderAndWriter<T>(true, ToLocalReaderAndWriter<T>(instance, port.Input), null, null);
        }

        [Pure]
        private static IVariantReader<T> ToLocalReader<T>([NotNull] GraphInstance instance, InputDataPort port) where T : unmanaged
        {
            return new LocalVariant.Reader<T> {Value = ReadValue<T>(instance, port)};
        }

        [Pure]
        private static IVariantReaderAndWriter<T> ToLocalReaderAndWriter<T>([NotNull] GraphInstance instance, InputDataPort port) where T : unmanaged
        {
            return new LocalVariant.ReaderAndWriter<T> {Value = ReadValue<T>(instance, port)};
        }

        [Pure]
        public static unsafe T ReadValue<T>([NotNull] this GraphInstance instance, InputDataPort port) where T : unmanaged
        {
            T data;
            void* ptr = &data;
            var value = instance.ReadValue(port);
            Value.SetPtrToValue(ptr, value.Type, value);
            return data;
        }

        [Pure]
        private static IVariantReader<T> ToVariantReader<T>(
            InputDataPort port
          , [NotNull] GraphInstance instance
          , [NotNull] GraphDefinition definition
          , [NotNull] Func<IVariantReader<T>> createGraphVariant
        ) where T : unmanaged
        {
            var (dataNode, dataIndex) = GetDataNode(port, definition);

            if (dataNode is IVisualVariantNode propertyNode)
                return propertyNode.GetVariantReader<T>(dataIndex, instance, definition);

            if (dataNode is IVisualVariantNode<T> genericPropertyNode)
                return genericPropertyNode.GetVariantReader(instance, definition);

            if (dataNode is IConstantNode)
                return ToLocalReader<T>(instance, port);

            return createGraphVariant();
        }

        [Pure]
        private static (INode node, int index) GetDataNode(
            InputDataPort port
          , [NotNull] GraphDefinition definition
        )
        {
            var inputPortIndex = (int)port.Port.Index;
            Assert.IsTrue(inputPortIndex < definition.PortInfoTable.Count);
            var dataIndex = (int)definition.PortInfoTable[inputPortIndex].DataIndex;
            Assert.IsTrue(dataIndex < definition.DataPortTable.Count);
            var nodeIndex = (int)definition.DataPortTable[dataIndex].GetIndex();
            Assert.IsTrue(nodeIndex < definition.NodeTable.Count);
            return (definition.NodeTable[nodeIndex], nodeIndex);
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

        public static Runtime.ValueType ToRunTimeValueType([NotNull] this Type type)
        {
            if (type == typeof(bool)) return Runtime.ValueType.Bool;
            if (type == typeof(int)) return Runtime.ValueType.Int;
            if (type == typeof(float)) return Runtime.ValueType.Float;
            if (type == typeof(float2)) return Runtime.ValueType.Float2;
            if (type == typeof(float3)) return Runtime.ValueType.Float3;
            if (type == typeof(float4)) return Runtime.ValueType.Float4;
            if (type == typeof(quaternion)) return Runtime.ValueType.Quaternion;
            if (type == typeof(Entity)) return Runtime.ValueType.Entity;
            if (type == typeof(StringReference)) return Runtime.ValueType.StringReference;
            return Runtime.ValueType.Unknown;
        }
    }
}
