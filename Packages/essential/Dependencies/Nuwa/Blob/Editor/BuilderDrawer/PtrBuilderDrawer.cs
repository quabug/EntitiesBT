using System;
using Nuwa.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nuwa.Blob.Editor
{
    public abstract class PtrBuilderDrawer : PropertyDrawer
    {
        protected abstract string PtrBuilderPropertyName { get; }
        protected abstract Type BlobType(SerializedProperty property);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProperty = ValueProperty(property);
            return EditorGUI.GetPropertyHeight(valueProperty, GUIContent.none, includeChildren: true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProperty = UpdateValueProperty(property);
            EditorGUI.PropertyField(position, valueProperty, label, includeChildren: true);
            property.serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var valueProperty = UpdateValueProperty(property);
            var propertyField = new PropertyField(valueProperty);
            propertyField.BindProperty(property.serializedObject);
            propertyField.name = valueProperty.name;
            propertyField.AddToClassList(valueProperty.propertyPath);
            return propertyField;
        }

        private SerializedProperty UpdateValueProperty(SerializedProperty property)
        {
            property.serializedObject.Update();
            var valueProperty = ValueProperty(property);
            var valueType = BlobType(property);
            var builderFactory = valueType.GetBuilderFactory(customBuilder: null);
            var value = valueProperty.GetObject();
            if (value == null || value.GetType() != builderFactory.Type) valueProperty.managedReferenceValue = builderFactory.Create();
            return valueProperty;
        }

        private SerializedProperty ValueProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative(PtrBuilderPropertyName);
        }
    }

    [CustomPropertyDrawer(typeof(PtrBuilder<>), useForChildren: true)]
    public class GenericBlobPtrDrawer : PtrBuilderDrawer
    {
        protected override string PtrBuilderPropertyName => nameof(PtrBuilder<int>.Value);
        protected override Type BlobType(SerializedProperty property) => property.GetObject().GetType().FindGenericArgumentsOf(typeof(PtrBuilder<>))[0];
    }

    [CustomPropertyDrawer(typeof(DynamicBlobPtrDrawer))]
    public class DynamicBlobPtrDrawer : PtrBuilderDrawer
    {
        protected override string PtrBuilderPropertyName => nameof(DynamicPtrBuilder.Value);
        protected override Type BlobType(SerializedProperty property) =>
            Type.GetType(property.FindPropertyRelative(nameof(DynamicPtrBuilder.DataType)).stringValue);
    }
}