using System;
using EntitiesBT.Variant;
using UnityEditor.Experimental.GraphView;

namespace EntitiesBT.Editor
{
    internal static class Utilities
    {
        public const string VariantPortClass = "variant";

        public static string VariantAccessMode(Type variantType)
        {
            if (typeof(IVariantReader).IsAssignableFrom(variantType)) return "ReadOnly";
            if (typeof(IVariantWriter).IsAssignableFrom(variantType)) return "WriteOnly";
            if (typeof(IVariantReaderAndWriter).IsAssignableFrom(variantType)) return "ReadWrite";
            throw new ArgumentException();
        }

        public static Port CreateVariantPort(Direction direction, Port.Capacity capacity, Type type)
        {
            var port = Port.Create<Edge>(Orientation.Horizontal, direction, capacity, type);
            port.portName = "";
            port.AddToClassList(VariantPortClass);
            port.AddToClassList(VariantAccessMode(type));
            return port;
        }
    }
}