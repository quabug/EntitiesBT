using System;
using System.Collections.Generic;
using System.Linq;
using DotsStencil;
using UnityEditor.GraphToolsFoundation.Overdrive;

namespace EntitiesBT.Builder.Visual.Editor
{
    internal static class Extension
    {
        /// <summary>
        /// Get PortMetadata for a each field in a type that can be matched to a ValueType
        /// </summary>
        /// <param name="componentType">type to parse fields from</param>
        /// <returns>Port Metadata corresponding to each valid field found in the type</returns>
        internal static IEnumerable<BaseDotsNodeModel.PortMetaData> ToPortMetaData(this Type componentType)
        {
            return componentType == null ? Enumerable.Empty<BaseDotsNodeModel.PortMetaData>() : componentType.GetFields()
                .Select(f => new BaseDotsNodeModel.PortMetaData { Name = f.Name, Type = f.FieldType.GenerateTypeHandle().ToValueType(out var t) ? t : Runtime.ValueType.Unknown });
        }
    }
}
