using UnityEngine;

namespace EntitiesBT.Attributes.Editor
{
    [CustomMultiPropertyDrawer(typeof(UnityDrawPropertyAttribute))]
    public class UnityDrawPropertyAttributeDrawer : BaseMultiPropertyDrawer
    {
        protected override void OnGUISelf(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.PropertyField(position, property, label);
        }
    }
}