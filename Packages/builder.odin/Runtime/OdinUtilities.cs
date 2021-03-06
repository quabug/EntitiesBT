using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using JetBrains.Annotations;

namespace EntitiesBT.Components.Odin
{
    public static class OdinUtilities
    {
#if UNITY_EDITOR
        private static Dictionary<Type, string[]> _READABLE_FIELD_NAME_MAP = new Dictionary<Type, string[]>();

        internal static IEnumerable<string> GetReadableFieldName<T>([CanBeNull] INodeDataBuilder builder)
            where T : unmanaged
        {
            if (builder == null) return Enumerable.Empty<string>();
            return GetReadableFieldName<T>(VirtualMachine.GetNodeType(builder.NodeId));
        }

        internal static IEnumerable<string> GetReadableFieldName<T>([CanBeNull] Type nodeType)
            where T : unmanaged
        {
            if (nodeType == null) return Enumerable.Empty<string>();
            if (!_READABLE_FIELD_NAME_MAP.TryGetValue(nodeType, out var names))
            {
                names = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(fi => fi.FieldType == typeof(T)
                                 || fi.FieldType == typeof(BlobVariantReader<T>)
                                 || fi.FieldType == typeof(BlobVariantReaderAndWriter<T>))
                    .Select(fi => fi.Name)
                    .ToArray()
                ;
                _READABLE_FIELD_NAME_MAP[nodeType] = names;
            }
            return names;
        }
#endif
    }
}