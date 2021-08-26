using EntitiesBT.Components;
using Nuwa.Blob;
using Nuwa.Editor;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(NodeAsset))]
    public class NodeAssetDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var builder = property.FindPropertyRelative(nameof(NodeAsset.Builder));
            return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(builder, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            var nodeType = property.FindPropertyRelative(nameof(NodeAsset.NodeType));
            var builder = property.FindPropertyRelative(nameof(NodeAsset.Builder));
            var builderBlobType = builder.FindPropertyRelative(nameof(DynamicBlobDataBuilder.BlobDataType));

            var valueType = System.Type.GetType(nodeType.stringValue);
            var blobType = System.Type.GetType(builderBlobType.stringValue);

            if (valueType != blobType)
            {
                builderBlobType.stringValue = valueType == null ? "" : valueType.AssemblyQualifiedName;
                builderBlobType.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorGUI.PropertyField(position, nodeType, label);
            position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(position, builder, includeChildren: true);
        }
    }
}