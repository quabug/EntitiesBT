using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Variant;
using JetBrains.Annotations;
using Nuwa.Blob;
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
            return serializedProperties.Select(property => new ConnectableVariant((GraphNodeVariant.Any)property.GetObject(), property.propertyPath, VariantName(property)));

            string VariantName(SerializedProperty property)
            {
                return string.Join(".", FieldPath(property).Skip(1).Where(fieldName => !fieldName.StartsWith("_")));
            }
        }

        static IEnumerable<string> FieldPath(SerializedProperty property)
        {
            IReadOnlyList<string> fieldNames = null;
            IReadOnlyList<IBuilder> builders = null;
            FieldInfo fieldInfo = null;
            foreach (var (fieldObject, fi) in property.GetFieldsByPath())
            {
                if (ReferenceEquals(fieldObject, builders))
                {
                    continue;
                }
                if (fieldNames != null && builders != null)
                {
                    var index = builders.IndexOf(fieldObject);
                    yield return fieldNames[index];
                    fieldNames = null;
                    builders = null;
                }
                else if (fieldObject is IObjectBuilder objectBuilder)
                {
                    fieldNames = objectBuilder.GetFieldNames();
                    builders = objectBuilder.GetBuilders();
                }
                else if (fieldInfo == fi)
                {
                    continue;
                }
                else
                {
                    yield return fi.Name;
                }
                fieldInfo = fi;
            }
        }
    }
}