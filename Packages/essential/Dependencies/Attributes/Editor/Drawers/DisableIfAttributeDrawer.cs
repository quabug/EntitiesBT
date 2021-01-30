using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Attributes.Editor
{
    [CustomMultiPropertyDrawer(typeof(DisableIfAttribute))]
    public class DisableIfAttributeDrawer : BaseMultiPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = (DisableIfAttribute) Decorator;
            var shouldDisable = (bool) property.GetSiblingFieldValue(attribute.ConditionFieldName) == attribute.Value;
            using (new EditorGUI.DisabledScope(shouldDisable))
            {
                base.OnGUI(position, property, label);
            }
        }
    }
}