using UnityEditor;
using UnityEngine;

namespace Nuwa.Blob.Editor
{
    [CustomPropertyDrawer(typeof(BlobViewer))]
    public class BlobViewerDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            property = property.FindPropertyRelative(nameof(BlobViewer.Viewer));
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            var typeName = property.FindPropertyRelative(nameof(BlobViewer.TypeName)).stringValue;
            var writable = property.FindPropertyRelative(nameof(BlobViewer.Writable)).boolValue;
            var enabled = GUI.enabled;
            GUI.enabled = writable;
            property = property.FindPropertyRelative(nameof(BlobViewer.Viewer));
            EditorGUI.PropertyField(position, property, new GUIContent($"{label.text} : {typeName}"), true);
            GUI.enabled = enabled;
        }
    }
}