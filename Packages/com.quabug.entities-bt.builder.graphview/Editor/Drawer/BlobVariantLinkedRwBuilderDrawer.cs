using EntitiesBT.Variant;
using Nuwa.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(BlobVariantLinkedRWBuilder))]
    public class BlobVariantLinkedRwBuilderDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, includeChildren: true);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new PropertyFieldFoldOut();
            root.name = nameof(BlobVariantLinkedRWBuilder);
            root.Add(new PropertyField(property.FindPropertyRelative("_isLinked")));
            root.Add(new PropertyField(property.FindPropertyRelative("_reader")));
            root.Add(new PropertyField(property.FindPropertyRelative("_writer")));
            root.Add(new PropertyField(property.FindPropertyRelative("_readerAndWriter")));
            return root;
        }
    }
}