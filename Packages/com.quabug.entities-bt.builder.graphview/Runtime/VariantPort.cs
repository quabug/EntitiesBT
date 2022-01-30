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
    }
}

#endif