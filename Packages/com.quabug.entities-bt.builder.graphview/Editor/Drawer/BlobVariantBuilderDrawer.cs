using EntitiesBT.Variant;
using GraphExt;
using GraphExt.Editor;
using Nuwa.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(BlobVariantROBuilder))]
    [CustomPropertyDrawer(typeof(BlobVariantWOBuilder))]
    [CustomPropertyDrawer(typeof(BlobVariantRWBuilder))]
    public class BlobVariantBuilderDrawer : PropertyDrawer
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
            var variantProperty = property.FindPropertyRelative("_variant");
            var root = new VisualElement();
            var inputPortContainer = new PortContainer(VariantPort.CreatePortName(variantProperty, PortDirection.Input))
            {
                name = "input-port-container"
            };
            root.Add(inputPortContainer);
            var propertyField = new PropertyFieldWithAncestorName(variantProperty);
            propertyField.AddToClassList(variantProperty.propertyPath);
            root.Add(propertyField);
            var outputPortContainer = new PortContainer(VariantPort.CreatePortName(variantProperty, PortDirection.Output))
            {
                name = "output-port-container"
            };
            root.Add(outputPortContainer);
            root.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            return root;
        }
    }
}