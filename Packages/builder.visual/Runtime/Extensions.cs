using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using JetBrains.Annotations;
using Runtime;
using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;

namespace EntitiesBT.Builder.Visual
{
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
        public static IVariablePropertyReader<T> ToVariablePropertyReader<T>(this InputDataPort port, [NotNull] GraphInstance instance, [NotNull] GraphDefinition definition) where T : unmanaged
        {
            return ToVariablePropertyReader(port, instance, definition, () => new GraphVariableProperty.Reader<T>(port));
        }
        //
        // [Pure]
        // public static IVariablePropertyReader<T> ToVariablePropertyReader<T>(this InputDataPort port, [NotNull] GraphInstance instance, [NotNull] GraphDefinition definition) where T : unmanaged
        // {
        //     return ToVariableProperty(port, instance, definition, () => ToConstVariable<T>(instance, port));
        // }

        [Pure]
        private static unsafe IVariablePropertyReader<T> ToConstVariable<T>([NotNull] GraphInstance instance, InputDataPort port) where T : unmanaged
        {
            T data;
            void* ptr = &data;
            var value = instance.ReadValue(port);
            Value.SetPtrToValue(ptr, value.Type, value);
            return new LocalVariableProperty.Reader<T> {Value = data};
        }

        [Pure]
        private static IVariablePropertyReader<T> ToVariablePropertyReader<T>(
            InputDataPort port
          , [NotNull] GraphInstance instance
          , [NotNull] GraphDefinition definition
          , [NotNull] Func<IVariablePropertyReader<T>> createGraphVariable
        ) where T : unmanaged
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

            if (dataNode is IConstantNode)
                return ToConstVariable<T>(instance, port);

            return createGraphVariable();
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

        public static Runtime.ValueType ToRunTimeValueType(this Type type)
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
