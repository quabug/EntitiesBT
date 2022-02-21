using EntitiesBT.Variant;
using Nuwa.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(VariantNode))]
    public class VariantNodeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, includeChildren: true);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var endProperty = property.GetEndProperty();
            property.NextVisible(enterChildren: true);
            while (!SerializedProperty.EqualContents(endProperty, property))
            {
                if (property.propertyPath == $"{nameof(VariantNode)}.{nameof(VariantNode<IVariant>.Value)}")
                {
                    property.NextVisible(enterChildren: true);
                }
                else
                {
                    var field = new PropertyFieldWithAncestorName(property);
                    field.AddToClassList(property.propertyPath);
                    root.Add(field);
                    property.NextVisible(enterChildren: false);
                }
            }
            return root;
        }
    }
}
