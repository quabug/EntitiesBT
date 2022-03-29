using System;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Blob.Editor
{
    public abstract class EnumBuilderDrawer : PropertyDrawer
    {
        protected abstract object GetValue(SerializedProperty property);
        protected abstract void SetValue(SerializedProperty property, object value);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            var enumType = Type.GetType(property.FindPropertyRelative(nameof(DynamicEnumBuilder<int>.EnumTypeName)).stringValue);
            var valueProperty = property.FindPropertyRelative(nameof(DynamicEnumBuilder<int>.Value));
            var value = EditorGUI.EnumPopup(position, label, (Enum)Enum.ToObject(enumType, GetValue(valueProperty)));
            SetValue(valueProperty, value);
            property.serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof(DynamicByteEnumBuilder))]
    [CustomPropertyDrawer(typeof(DynamicSByteEnumBuilder))]
    [CustomPropertyDrawer(typeof(DynamicShortEnumBuilder))]
    [CustomPropertyDrawer(typeof(DynamicUShortEnumBuilder))]
    [CustomPropertyDrawer(typeof(DynamicIntEnumBuilder))]
    [CustomPropertyDrawer(typeof(DynamicUIntEnumBuilder))]
    public class IntEnumBuilderDrawer : EnumBuilderDrawer
    {
        protected override object GetValue(SerializedProperty property) => property.intValue;
        protected override void SetValue(SerializedProperty property, object value) => property.intValue = Convert.ToInt32(value);
    }

    [CustomPropertyDrawer(typeof(DynamicLongEnumBuilder))]
    [CustomPropertyDrawer(typeof(DynamicULongEnumBuilder))]
    public class LongEnumBuilderDrawer : EnumBuilderDrawer
    {
        protected override object GetValue(SerializedProperty property) => property.longValue;
        protected override void SetValue(SerializedProperty property, object value) => property.longValue = Convert.ToInt64(value);
    }
}