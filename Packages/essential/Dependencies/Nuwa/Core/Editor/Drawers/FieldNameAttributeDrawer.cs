using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(FieldNameAttribute))]
    public class FieldNameAttributeDrawer : BaseMultiPropertyDrawer
    {
        protected override void OnGUISelf(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String) return;
            property.serializedObject.Update();
            var attribute = (FieldNameAttribute) Decorator;
            var declaringType = attribute.DeclaringType ?? property.GetSiblingValue(attribute.DeclaringTypeVariable) as Type;
            var fieldType = attribute.FieldType ?? property.GetSiblingValue(attribute.FieldTypeVariable) as Type;
            var options = Array.Empty<string>();
            if (declaringType != null && fieldType != null) options = declaringType
                .GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(fi => fi.FieldType == fieldType).Select(fi => fi.Name)
                .ToArray()
            ;
            property.PopupFunc()(position, label.text, options);
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}