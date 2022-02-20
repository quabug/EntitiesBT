#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Variant;
using GraphExt;
using Nuwa.Editor;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public class VariantPorts
    {
        private SerializedProperty[] _variantProperties;

        public IEnumerable<PortData> FindNodePorts(SerializedProperty property)
        {
            _variantProperties ??= GetVariantProperties(property).ToArray();
            foreach (var variant in _variantProperties)
            {
                var variantType = variant.GetManagedFullType();
                if (variantType != null && typeof(GraphNodeVariant.Any).IsAssignableFrom(variantType))
                {
                    yield return CreateVariantPortData(variant, variantType, PortDirection.Input);
                    yield return CreateVariantPortData(variant, variantType, PortDirection.Output);
                }
            }
        }

        private IEnumerable<SerializedProperty> GetVariantProperties(SerializedProperty property)
        {
            while (property.NextVisible(true))
            {
                var fieldType = property.GetManagedFieldType();
                if (fieldType != null && typeof(IVariant).IsAssignableFrom(fieldType))
                    yield return property.Copy();
            }
        }

        public static string CreatePortName(SerializedProperty property, PortDirection direction)
        {
            return $"{property.propertyPath}|{direction.ToString().ToLower()}";
        }

        public static void Deconstruct(in PortId portId, out NodeId nodeId, out string propertyPath, out PortDirection direction)
        {
            nodeId = portId.NodeId;
            var portName = portId.Name;
            var splitIndex = portName.LastIndexOf('|');
            propertyPath = portName.Substring(0, splitIndex);
            var directionName = portName.Substring(splitIndex + 1);
            Enum.TryParse(directionName, out direction);
        }

        public static Type GetPortType(Type variantType)
        {
            var portType = typeof(IVariant);
            var interfaces = variantType.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IVariantReader<>))
                {
                    portType = @interface;
                    break;
                }

                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IVariantWriter<>))
                {
                    portType = @interface;
                    break;
                }

                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IVariantReaderAndWriter<>))
                {
                    portType = @interface;
                    break;
                }
            }
            return portType;
        }

        public static SerializedProperty FindVariantPortProperty<TNode, TComponent>(in PortId portId, GameObjectNodes<TNode, TComponent> nodes)
            where TNode : INode<GraphRuntime<TNode>>
            where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
        {
            try
            {
                Deconstruct(portId, out var nodeId, out var propertyPath, out _);
                return nodes.SerializedObjects.TryGetValue(nodeId, out var node) ? node.FindProperty(propertyPath) : null;
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<string> GetPortClasses(Type variantType)
        {
            yield return "variant";
            yield return VariantNode.GetVariantAccessName(variantType);
        }

        public static PortData CreateVariantPortData(SerializedProperty property, Type variantType, PortDirection direction)
        {
            return new PortData(
                CreatePortName(property, direction),
                PortOrientation.Horizontal,
                direction,
                1,
                GetPortType(variantType),
                GetPortClasses(variantType).ToArray()
            );
        }
    }
}

#endif