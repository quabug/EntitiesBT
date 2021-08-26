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

        private PropertyDrawer _customDrawer;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetOrCreateCustomDrawer(property).GetPropertyHeight(property, label);
        }

        protected override void OnGUISelf(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            GetOrCreateCustomDrawer(property).OnGUI(position, property, label);
        }

        PropertyDrawer GetOrCreateCustomDrawer(SerializedProperty property)
        {
            if (_customDrawer != null) return _customDrawer;

            var propertyType = property?.GetObject()?.GetType();
            var customDrawerType = propertyType != null ? property.GetDrawerTypeForPropertyAndType(propertyType) : null;
            if (customDrawerType == null)
            {
                _customDrawer = new DefaultDrawer();
            }
            else
            {
                _customDrawer = (PropertyDrawer) Activator.CreateInstance(customDrawerType);
            }
            return _customDrawer;
        }
    }
}