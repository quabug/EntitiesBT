using System;
using System.Linq;
using Nuwa.Editor;
using EntitiesBT.Variant;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(VariantComponentDataAttribute))]
    public class VariantComponentDataAttributeDrawer : PropertyDrawer
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
                    _options = Variant.Utilities.GetComponentFields(_genericType)
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
