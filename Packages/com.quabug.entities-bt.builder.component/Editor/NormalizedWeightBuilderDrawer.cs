using System;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Nuwa.Editor;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(NormalizedWeightBuilder))]
    public class NormalizedWeightBuilderDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var weightsProperty = property.FindPropertyRelative(nameof(NormalizedWeightBuilder.Weights));
            return weightsProperty.arraySize * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            var transform = ((UnityEngine.Component)property.serializedObject.targetObject).GetComponent<Transform>();
            var weightsProperty = property.FindPropertyRelative(nameof(NormalizedWeightBuilder.Weights));
            var weightsObject = (int[])weightsProperty.GetObject();

            var children = transform.Children<BTNode>().Where(child => child.gameObject.activeInHierarchy);
            var childCount = children.Count();

            if (weightsProperty.arraySize != childCount)
            {
                weightsProperty.arraySize = childCount;
                Array.Resize(ref weightsObject, childCount);
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            var index = 0;
            foreach (var child in children)
            {
                EditorGUI.IntSlider(position, weightsProperty.GetArrayElementAtIndex(index), 0, 100, new GUIContent(child.name));
                position.y += EditorGUIUtility.singleLineHeight;
                index++;
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}