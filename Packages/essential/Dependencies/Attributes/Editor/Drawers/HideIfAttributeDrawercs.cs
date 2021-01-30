using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Attributes.Editor
{
    [CustomMultiPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfAttributeDrawer : BaseMultiPropertyDrawer
    {
        private bool ShouldHide(SerializedProperty property)
        {
            var attribute = (HideIfAttribute) Decorator;
            return (bool) property.GetSiblingFieldValue(attribute.ConditionFieldName) == attribute.Value;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldHide(property) ? 0 : base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!ShouldHide(property)) base.OnGUI(position, property, label);
        }
    }
}