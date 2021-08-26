using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Blob.Editor
{
    [CustomPropertyDrawer(typeof(DynamicBlobDataViewer))]
    public class BlobDataViewerDrawer : PropertyDrawer
    {
        public Type GetBlobType(SerializedProperty property)
        {
            return Type.GetType(property.FindPropertyRelative(nameof(DynamicBlobDataViewer.TypeName)).stringValue);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded)
            {
                var viewers = FieldsViewer(property);
                for (var i = 0; i < viewers.arraySize; i++)
                {
                    var viewProperty = viewers.GetArrayElementAtIndex(i);
                    height += EditorGUI.GetPropertyHeight(viewProperty, includeChildren: true);
                }
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            var blobType = GetBlobType(property);
            var blobFields = blobType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(position, property, label);

            if (property.isExpanded)
            {
                var viewers = FieldsViewer(property);
                EditorGUI.indentLevel++;
                position = EditorGUI.IndentedRect(position);
                var propertyHeight = position.height;
                for (var i = 0; i < blobFields.Length; i++)
                {
                    position = new Rect(position.x, position.y + propertyHeight, position.width, position.height);
                    var viewProperty = viewers.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(position, viewProperty, new GUIContent(blobFields[i].Name), includeChildren: true);
                    propertyHeight = EditorGUI.GetPropertyHeight(viewProperty, includeChildren: true);
                }
                EditorGUI.indentLevel--;
            }
        }

        private SerializedProperty FieldsViewer(SerializedProperty property)
        {
            return property.FindPropertyRelative(nameof(DynamicBlobDataViewer.FieldsViewer));
        }
    }
}