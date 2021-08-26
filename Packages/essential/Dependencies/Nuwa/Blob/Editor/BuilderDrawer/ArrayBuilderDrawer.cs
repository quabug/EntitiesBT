using System;
using Nuwa.Editor;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Blob.Editor
{
    public abstract class ArrayBuilderDrawer : PropertyDrawer
    {
        protected abstract Type FindElementType(SerializedProperty property);
        protected abstract string ElementsPropertyName { get; }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(FindElementsProperty(property), GUIContent.none, includeChildren: true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            var elementsProperty = FindElementsProperty(property);
            var builderFactory = FindElementType(property).GetBuilderFactory(customBuilder: null);
            for (var i = 0; i < elementsProperty.arraySize; i++)
            {
                var elementProperty = elementsProperty.GetArrayElementAtIndex(i);
                var element = elementProperty.GetObject();
                if (element == null || element.GetType() != builderFactory.BuilderType)
                {
                    elementProperty.managedReferenceValue = builderFactory.Create();
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            EditorGUI.PropertyField(position, elementsProperty, label, includeChildren: true);
            property.serializedObject.ApplyModifiedProperties();
        }

        private SerializedProperty FindElementsProperty(SerializedProperty property) => property.FindPropertyRelative(ElementsPropertyName);
    }

    [CustomPropertyDrawer(typeof(ArrayBuilder<>), useForChildren: true)]
    public class GenericArrayBuilderDrawer : ArrayBuilderDrawer
    {
        protected override Type FindElementType(SerializedProperty property)
        {
            return property.GetObject().GetType().FindGenericArgumentsOf(typeof(ArrayBuilder<>))[0];
        }

        protected override string ElementsPropertyName => nameof(ArrayBuilder<int>.Value);
    }

    [CustomPropertyDrawer(typeof(DynamicArrayBuilder))]
    public class DynamicArrayBuilderDrawer : ArrayBuilderDrawer
    {
        protected override Type FindElementType(SerializedProperty property)
        {
            return Type.GetType(property.FindPropertyRelative(nameof(DynamicArrayBuilder.ArrayElementType)).stringValue);
        }

        protected override string ElementsPropertyName => nameof(DynamicArrayBuilder.Value);
    }
}