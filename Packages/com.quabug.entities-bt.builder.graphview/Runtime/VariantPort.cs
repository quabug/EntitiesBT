#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using EntitiesBT.Variant;
using GraphExt;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public static class VariantPort
    {
        public static string CreatePortName(SerializedProperty property, PortDirection direction)
        {
            return $"{property.propertyPath}|{direction.ToString().ToLower()}";
        }

        public static void Deconstruct(this in PortId portId, out NodeId nodeId, out string propertyPath, out PortDirection direction)
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

        public static SerializedProperty FindVariantPortProperty(
            this in PortId portId,
            GameObjectNodes<VariantNode, VariantNodeComponent> variantNodes,
            GameObjectNodes<BehaviorTreeNode, BehaviorTreeNodeComponent> behaviorNodes
        )
        {
            return FindVariantPortProperty(portId, variantNodes) ?? FindVariantPortProperty(portId, behaviorNodes);
        }

        public static SerializedProperty FindVariantPortProperty<TNode, TComponent>(this in PortId portId, GameObjectNodes<TNode, TComponent> nodes)
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
    }
}

#endif