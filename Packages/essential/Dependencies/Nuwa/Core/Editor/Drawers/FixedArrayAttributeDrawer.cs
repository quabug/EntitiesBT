using System;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(FixedArrayAttribute))]
    public class FixedArrayAttributeDrawer : BaseMultiPropertyDrawer
    {
        protected override void OnGUISelf(Rect position, SerializedProperty property, GUIContent label)
        {
            property = property.GetArrayProperty();
            property.serializedObject.Update();

            var attribute = (FixedArrayAttribute) Decorator;
            var length = attribute.FixedLength;
            if (!string.IsNullOrEmpty(attribute.GetLength))
            {
                var getLengthMethod = property.GetSiblingMethodInfo(attribute.GetLength);
                length = (int)getLengthMethod.Invoke(property.GetDeclaringObject(), Array.Empty<object>());
            }

            if (length != property.arraySize)
            {
                property.arraySize = length;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}