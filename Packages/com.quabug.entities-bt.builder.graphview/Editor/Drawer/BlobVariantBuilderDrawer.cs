using System.Linq;
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
            var inputPortContainer = new PortContainer(VariantPorts.CreatePortName(variantProperty, PortDirection.Input))
            {
                name = "input-port-container",
                style = { position = Position.Absolute, left = -16 }
            };
            // HACK: set input port at the edge of left border
            inputPortContainer.RegisterCallback(new EventCallback<AttachToPanelEvent>(
                _ => inputPortContainer.style.left = -16 * inputPortContainer.Ancestors().Count(view => view is Foldout))
            );
            root.Add(inputPortContainer);
            var propertyField = new PropertyFieldWithAncestorName(variantProperty);
            propertyField.AddToClassList(variantProperty.propertyPath);
            propertyField.style.paddingRight = 12;
            root.Add(propertyField);
            var outputPortContainer = new PortContainer(VariantPorts.CreatePortName(variantProperty, PortDirection.Output))
            {
                name = "output-port-container",
                style = { position = Position.Absolute, right = -2 }
            };
            root.Add(outputPortContainer);
            root.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            return root;
        }
    }
}