using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfAttributeDrawer : BaseMultiPropertyDrawer
    {
        private HideIfAttribute _attribute => (HideIfAttribute) Decorator;

        private bool ShouldHide(SerializedProperty property)
        {
            return (bool) property.GetSiblingValue(_attribute.ConditionName) == _attribute.Value;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldHide(property)
                ? (_attribute.LeaveEmptySpace ? EditorGUI.GetPropertyHeight(property, false) : 0)
                : base.GetPropertyHeight(property, label)
            ;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!ShouldHide(property)) base.OnGUI(position, property, label);
        }
    }
}