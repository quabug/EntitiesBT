using System;
using System.Linq;
using EntitiesBT.Variable;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(VariableComponentDataAttribute))]
    public class VariableComponentDataAttributeDrawer : PropertyDrawer
    {
        private GUIContent[] _options = null;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (_options == null)
                {
                    var fieldInfo = property.GetFieldInfo();
                    var fieldValue = fieldInfo.GetValue(property.serializedObject.targetObject);
                    var genericType = fieldValue.GetType().GetGenericType();
                    _options = Variable.Utility.GetComponentFields(genericType)
                        .Select(data => new GUIContent(data.Name))
                        .ToArray()
                    ;
                }

                var optionIndex = Array.FindIndex(_options, opt => opt.text == property.stringValue);
                if (optionIndex < 0) optionIndex = 0;
                optionIndex = EditorGUI.Popup(position, label, optionIndex, _options);
                property.stringValue = optionIndex < _options.Length ? _options[optionIndex].text : "";
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}
