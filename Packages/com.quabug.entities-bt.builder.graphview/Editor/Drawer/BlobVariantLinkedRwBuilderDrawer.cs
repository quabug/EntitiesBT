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
            root.name = nameof(BlobVariantLinkedRWBuilder);
            AddPropertyField("_isLinked", "Is Linked");
            AddPropertyField("_reader", "Reader");
            AddPropertyField("_writer", "Writer");
            AddPropertyField("_readerAndWriter", "Reader And Writer");
            return root;

            void AddPropertyField(string relativeProperty, string name)
            {
                var childProperty = property.FindPropertyRelative(relativeProperty);
                var propertyField = new ImmediatePropertyField(childProperty, name);
                propertyField.BindProperty(childProperty.serializedObject);
                propertyField.AddToClassList(childProperty.propertyPath);
                root.Add(propertyField);
            }
        }
    }
}