using System;
using Nuwa.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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
            var array = UpdateArrayElements(property);
            EditorGUI.PropertyField(position, array, label, includeChildren: true);
            property.serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var array = UpdateArrayElements(property);
            // TODO: set array size by an integer field
            var root = new PropertyFieldFoldOut();
            Refresh(array);
            // TODO: working but throw NullReferenceException?
            //       NullReferenceException: Object reference not set to an instance of an object
            //       UnityEditor.UIElements.Bindings.SerializedObjectBindingContext.UpdateValidProperties () (at /Users/bokken/buildslave/unity/build/External/MirroredPackageSources/com.unity.ui/Editor/Bindings/BindingExtensions.cs:800)
            // root.TrackPropertyValue(array, Refresh);
            return root;

            void Refresh(SerializedProperty elements)
            {
                root.Clear();
                for (var i = 0; i < elements.arraySize; i++)
                {
                    var builder = elements.GetArrayElementAtIndex(i);
                    var field = new PropertyFieldWithoutLabel(builder);
                    field.AddToClassList(builder.propertyPath);
                    root.Add(field);
                }
            }
        }

        private SerializedProperty UpdateArrayElements(SerializedProperty property)
        {
            property.serializedObject.Update();
            var elementsProperty = FindElementsProperty(property);
            var builderFactory = FindElementType(property).GetBuilderFactory(customBuilder: null);
            for (var i = 0; i < elementsProperty.arraySize; i++)
            {
                var elementProperty = elementsProperty.GetArrayElementAtIndex(i);
                var element = elementProperty.GetObject();
                if (element == null || element.GetType() != builderFactory.Type)
                {
                    elementProperty.managedReferenceValue = builderFactory.Create();
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            return elementsProperty;
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