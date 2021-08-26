using EntitiesBT.Components;
using Nuwa.Blob;
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
            return EditorGUI.GetPropertyHeight(builder, true);
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

            builder.isExpanded = true;
            EditorGUI.PropertyField(position, nodeType, GUIContent.none);
            EditorGUI.PropertyField(position, builder, includeChildren: true);
        }
    }
}