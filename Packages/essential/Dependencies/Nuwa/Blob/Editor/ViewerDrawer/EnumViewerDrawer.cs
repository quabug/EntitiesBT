using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Blob.Editor
{
    [CustomPropertyDrawer(typeof(DynamicEnumViewer))]
    public class EnumViewerDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            var enumType = Type.GetType(property.FindPropertyRelative(nameof(DynamicEnumViewer.EnumType)).stringValue);
            var dataPtr = new IntPtr(property.FindPropertyRelative(nameof(DynamicEnumViewer.Ptr)).longValue);
            EditorGUI.EnumPopup(position, label, (Enum)Enum.ToObject(enumType, GetEnum(enumType, dataPtr)));
        }

        Enum GetEnum(Type enumType, IntPtr dataPtr)
        {
            var underlingType = Enum.GetUnderlyingType(enumType);
            var value = Marshal.PtrToStructure(dataPtr, underlingType);
            return (Enum)Enum.ToObject(enumType, value);
        }
    }
}