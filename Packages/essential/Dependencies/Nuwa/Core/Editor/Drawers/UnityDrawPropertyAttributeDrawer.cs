using System;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(UnityDrawPropertyAttribute))]
    public class UnityDrawPropertyAttributeDrawer : BaseMultiPropertyDrawer
    {
        private class DefaultDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.PropertyField(position, property, label, includeChildren: true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetOrCreateCustomDrawer(property).GetPropertyHeight(property, label);
        }

        protected override void OnGUISelf(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            GetOrCreateCustomDrawer(property).OnGUI(position, property, label);
            property.serializedObject.ApplyModifiedProperties();
        }

        PropertyDrawer GetOrCreateCustomDrawer(SerializedProperty property)
        {
            var propertyType = property?.GetObject()?.GetType();
            var customDrawerType = propertyType != null ? property.GetDrawerTypeForPropertyAndType(propertyType) : null;
            return customDrawerType == null ? new DefaultDrawer() : (PropertyDrawer)Activator.CreateInstance(customDrawerType);
        }
    }
}