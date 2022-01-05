#if UNITY_EDITOR

using System;
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

        public static PortId CreatePort(in NodeId nodeId, SerializedProperty property, PortDirection direction)
        {
            return new PortId(nodeId, CreatePortName(property, direction));
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