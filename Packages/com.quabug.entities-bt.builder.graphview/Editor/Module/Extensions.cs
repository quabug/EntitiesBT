using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Variant;
using JetBrains.Annotations;
using Nuwa.Editor;
using UnityEditor;

namespace EntitiesBT.Editor
{
    internal static class Extensions
    {
        [NotNull, Pure] public static IEnumerable<SerializedProperty> FindVariantProperties([NotNull] this SerializedObject nodeObject)
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

        [NotNull, Pure] public static IEnumerable<SerializedProperty> FindGraphNodeVariantProperties([NotNull] this SerializedObject nodeObject)
        {
            return nodeObject.FindVariantProperties().Where(property => typeof(GraphNodeVariant.Any).IsAssignableFrom(property.GetManagedFullType()));
        }

        public static IEnumerable<ConnectableVariant> ToConnectableVariants(this IEnumerable<SerializedProperty> serializedProperties)
        {
            return serializedProperties.Select(property => new ConnectableVariant((GraphNodeVariant.Any)property.GetObject(), property.propertyPath));
        }
    }
}