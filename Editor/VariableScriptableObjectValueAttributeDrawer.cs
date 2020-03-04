using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.Components;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(VariableScriptableObjectValueAttribute))]
    public class VariableScriptableObjectValueAttributeDrawer : PropertyDrawer
    {
        private object _scriptableObject;
        private string[] _options;
        private Type _genericType;
        private VariableScriptableObjectValueAttribute _attribute;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (_genericType == null) _genericType = property.GetGenericType();
                if (_attribute == null) _attribute = property.GetCustomAttribute<VariableScriptableObjectValueAttribute>();
                var scriptableObject = property.GetSiblingFieldValue(_attribute.ScriptableObjectFieldName);
                if (!Equals(scriptableObject, _scriptableObject))
                {
                    _scriptableObject = scriptableObject;
                    _options = scriptableObject == null
                        ? new string[0]
                        : scriptableObject.GetType()
                            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(fi => fi.FieldType == _genericType)
                            .Select(fi => fi.Name)
                            .ToArray()
                    ;
                }
                property.PopupFunc()(position, label.text, _options);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}
