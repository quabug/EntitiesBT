using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfAttributeDrawer : BaseMultiPropertyDrawer
    {
        private ShowIfAttribute _attribute => (ShowIfAttribute) Decorator;

        private bool ShouldShow(SerializedProperty property)
        {
            return (bool) property.GetSiblingValue(_attribute.ConditionName) == _attribute.Value;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!ShouldShow(property))
                return _attribute.LeaveEmptySpace ? EditorGUI.GetPropertyHeight(property, includeChildren: false) : 0;
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property)) base.OnGUI(position, property, label);
        }
    }
}