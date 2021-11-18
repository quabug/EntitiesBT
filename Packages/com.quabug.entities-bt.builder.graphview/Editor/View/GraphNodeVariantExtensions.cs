using System.Collections.Generic;
using EntitiesBT.Variant;
using JetBrains.Annotations;
using Nuwa.Editor;
using UnityEditor;

namespace EntitiesBT.Editor
{
    public static partial class GraphNodeVariantExtensions
    {
        [NotNull, Pure]
        public static IEnumerable<SerializedProperty> FindVariantProperties([NotNull] this SerializedObject nodeObject)
        {
            var property = nodeObject.GetIterator();
            var stepInto = true;
            while (property.NextVisible(stepInto))
            {
                var fieldType = property.GetManagedFieldType();
                var isVariantProperty = typeof(IVariant).IsAssignableFrom(fieldType);
                stepInto = !isVariantProperty;
                if (isVariantProperty) yield return property;
            }
        }
    }
}