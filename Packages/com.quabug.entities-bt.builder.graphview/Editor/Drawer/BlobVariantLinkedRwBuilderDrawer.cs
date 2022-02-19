using EntitiesBT.Variant;
using Nuwa.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(BlobVariantLinkedRWBuilder))]
    public class BlobVariantLinkedRWBuilderDrawer : PropertyDrawer
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
            var root = new PropertyFieldFoldOut();
            var isLinkedProperty = AddPropertyField("_isLinked", "Is Linked");
            var readerProperty = AddPropertyField("_reader", "Reader");
            var writerProperty = AddPropertyField("_writer", "Writer");
            var readerWriterProperty = AddPropertyField("_readerAndWriter", "Reader And Writer");

            var isLinkedToggle = isLinkedProperty.Q<Toggle>();
            isLinkedToggle.style.marginLeft = 0;
            isLinkedToggle.style.marginRight = 0;
            isLinkedToggle.style.marginTop = 0;
            isLinkedToggle.style.marginBottom = 0;
            isLinkedToggle.style.unityTextAlign = TextAnchor.MiddleLeft;

            isLinkedToggle.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    readerWriterProperty.style.display = DisplayStyle.Flex;
                    readerProperty.style.display = DisplayStyle.None;
                    writerProperty.style.display = DisplayStyle.None;
                }
                else
                {
                    readerWriterProperty.style.display = DisplayStyle.None;
                    readerProperty.style.display = DisplayStyle.Flex;
                    writerProperty.style.display = DisplayStyle.Flex;
                }
            });

            return root;

            ImmediatePropertyField AddPropertyField(string relativeProperty, string name)
            {
                var childProperty = property.FindPropertyRelative(relativeProperty);
                var propertyField = new ImmediatePropertyField(childProperty, name);
                propertyField.AddToClassList(childProperty.propertyPath);
                root.Add(propertyField);
                return propertyField;
            }
        }
    }
}