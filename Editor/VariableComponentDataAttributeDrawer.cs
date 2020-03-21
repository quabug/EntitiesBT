using System;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Variable;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(VariableComponentDataAttribute))]
    public class VariableComponentDataAttributeDrawer : PropertyDrawer
    {
        private string[] _options;
        private Type _genericType;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (_genericType == null) _genericType = this.GetGenericType();
                if (_options == null)
                {
                    _options = Variable.Utility.GetComponentFields(_genericType)
                        .Select(data => data.Name)
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
