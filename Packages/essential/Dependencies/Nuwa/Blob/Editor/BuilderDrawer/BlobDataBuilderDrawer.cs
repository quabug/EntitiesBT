using System;
using System.Reflection;
using Nuwa.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nuwa.Blob.Editor
{
    public abstract class BlobDataBuilderDrawer : PropertyDrawer
    {
        protected abstract string FieldNamePropertyName { get; }
        protected abstract string BuildersPropertyName { get; }
        protected abstract Type GetBlobType(SerializedProperty property);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded)
            {
                var builders = Builders(property);
                for (var i = 0; i < builders.arraySize; i++)
                {
                    var builderProperty = builders.GetArrayElementAtIndex(i);
                    height += EditorGUI.GetPropertyHeight(builderProperty, includeChildren: true);
                }
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var (buildersProperty, fieldNamesProperty) = UpdateBuilders(property);

            // create inspector GUI
            position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(position, property, label);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                position = EditorGUI.IndentedRect(position);
                var propertyHeight = position.height;
                for (var i = 0; i < buildersProperty.arraySize; i++)
                {
                    position = new Rect(position.x, position.y + propertyHeight, position.width, position.height);
                    var builderProperty = buildersProperty.GetArrayElementAtIndex(i);
                    var fieldName = fieldNamesProperty.GetArrayElementAtIndex(i).stringValue;
                    EditorGUI.PropertyField(position, builderProperty, new GUIContent(fieldName), includeChildren: true);
                    propertyHeight = EditorGUI.GetPropertyHeight(builderProperty, includeChildren: true);
                    // HACK (bug?): somehow, `ApplyModifiedProperties` must be place inside `for` loop, otherwise some properties will not be changed in some cases.
                    property.serializedObject.ApplyModifiedProperties();
                }
                EditorGUI.indentLevel--;
            }
            property.serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var (buildersProperty, fieldNamesProperty) = UpdateBuilders(property);
            var root = new Foldout();
            root.text = property.displayName;
            for (var i = 0; i < buildersProperty.arraySize; i++)
            {
                var builder = buildersProperty.GetArrayElementAtIndex(i);
                var fieldName = fieldNamesProperty.GetArrayElementAtIndex(i).stringValue;
                var field = new PropertyField(builder, fieldName);
                field.BindProperty(property.serializedObject);
                root.Add(field);
            }
            return root;
        }

        private SerializedProperty Builders(SerializedProperty property)
        {
            return property.FindPropertyRelative(BuildersPropertyName);
        }

        private SerializedProperty FieldNames(SerializedProperty property)
        {
            return property.FindPropertyRelative(FieldNamePropertyName);
        }

        private (SerializedProperty builders, SerializedProperty fieldNames) UpdateBuilders(SerializedProperty property)
        {
            property.serializedObject.Update();
            var blobType = GetBlobType(property);
            var blobFields = blobType?.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? Array.Empty<FieldInfo>();

            var buildersProperty = Builders(property);
            buildersProperty.arraySize = blobFields.Length;
            var builders = (IBuilder[]) buildersProperty.GetObject();
            Array.Resize(ref builders, blobFields.Length);

            var fieldNamesProperty = FieldNames(property);
            fieldNamesProperty.arraySize = blobFields.Length;
            var fieldNames = (string[]) fieldNamesProperty.GetObject();
            Array.Resize(ref fieldNames, blobFields.Length);

            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            for (var i = 0; i < blobFields.Length; i++)
            {
                var blobField = blobFields[i];
                var builderFactory = blobField.FindBuilderCreator();
                var builder = builders[i];
                if (builder == null || builder.GetType() != builderFactory.Type || blobField.Name != fieldNames[i])
                {
                    fieldNamesProperty.GetArrayElementAtIndex(i).stringValue = blobField.Name;
                    var builderIndex = Array.IndexOf(fieldNames, blobField.Name);
                    object newBuilder = null;
                    if (builderIndex >= 0) newBuilder = builders[builderIndex];
                    else if (builder != null && builder.GetType() == builderFactory.Type) newBuilder = builder;
                    else newBuilder = builderFactory.Create();
                    buildersProperty.GetArrayElementAtIndex(i).managedReferenceValue = newBuilder;
                    property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            return (buildersProperty, fieldNamesProperty);
        }
    }

    [CustomPropertyDrawer(typeof(BlobDataBuilder<>), useForChildren: true)]
    public class GenericBlobDataBuilderDrawer : BlobDataBuilderDrawer
    {
        protected override string FieldNamePropertyName => nameof(BlobDataBuilder<int>.FieldNames);
        protected override string BuildersPropertyName => nameof(BlobDataBuilder<int>.Builders);
        protected override Type GetBlobType(SerializedProperty property)
        {
            var fieldType = fieldInfo?.FieldType;
            if (fieldType == null || !typeof(Builder<>).IsAssignableFrom(fieldType))
                fieldType = property.GetObject().GetType();
            return fieldType.FindGenericArgumentsOf(typeof(Builder<>))[0];
        }
    }

    [CustomPropertyDrawer(typeof(DynamicBlobDataBuilder))]
    public class DynamicBlobDataBuilderDrawer : BlobDataBuilderDrawer
    {
        protected override string FieldNamePropertyName => nameof(DynamicBlobDataBuilder.FieldNames);
        protected override string BuildersPropertyName => nameof(DynamicBlobDataBuilder.Builders);
        protected override Type GetBlobType(SerializedProperty property) =>
            Type.GetType(property.FindPropertyRelative(nameof(DynamicBlobDataBuilder.BlobDataType)).stringValue);
    }
}