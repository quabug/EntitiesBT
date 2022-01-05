#if UNITY_EDITOR

using System;
using EntitiesBT.Variant;
using GraphExt;
using UnityEditor;

namespace EntitiesBT.Editor
{
    public static class VariantPort
    {
        public static string CreatePortName(SerializedProperty property, PortDirection direction)
        {
            return $"{property.propertyPath}|{direction.ToString().ToLower()}";
        }

        public static PortId CreatePortId(in NodeId nodeId, SerializedProperty property, PortDirection direction)
        {
            return new PortId(nodeId, CreatePortName(property, direction));
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

        public static void Deconstruct(this in PortId portId, out NodeId nodeId, out string propertyPath, out PortDirection direction)
        {
            nodeId = portId.NodeId;
            var portName = portId.Name;
            var splitIndex = portName.LastIndexOf('|');
            propertyPath = portName.Substring(0, splitIndex - 1);
            Enum.TryParse(portName.Substring(splitIndex), out direction);
        }
    }
}

#endif