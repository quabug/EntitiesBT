using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(UnboxSinglePropertyAttribute))]
    public class UnboxSinglePropertyAttributeDrawer : BaseMultiPropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            property = property.FindProperProperty();
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property = property.FindProperProperty();
            base.OnGUI(position, property, label);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            property = property.FindProperProperty();
            return base.CreatePropertyGUI(property);
        }
    }
}