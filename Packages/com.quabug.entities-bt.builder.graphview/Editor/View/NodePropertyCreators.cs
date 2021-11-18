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
        public static IEnumerable<SerializedProperty> FindGraphNodeVariantProperties([NotNull] this SerializedObject nodeObject)
        {
            var property = nodeObject.GetIterator();
            var stepInto = true;
            while (property.NextVisible(stepInto))
            {
                var fieldType = property.GetManagedFieldType();
                var valueType = property.GetManagedFullType();
                if (typeof(GraphNodeVariant.Any).IsAssignableFrom(valueType)) yield return property;
                stepInto = !typeof(IVariant).IsAssignableFrom(fieldType);
            }
        }
    }
}