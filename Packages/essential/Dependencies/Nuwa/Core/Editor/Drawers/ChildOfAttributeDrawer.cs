using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Nuwa.Editor
{
    [CustomMultiPropertyDrawer(typeof(ChildOfAttribute))]
    public class ChildOfAttributeDrawer : BaseMultiPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Assert.AreEqual(property.propertyType, SerializedPropertyType.ObjectReference);

            property.serializedObject.Update();

            var attribute = (ChildOfAttribute) Decorator;
            var parent = property.GetSiblingValue(attribute.ParentName);

            var parentTransform = parent switch
            {
                GameObject obj => obj == null ? null : obj.transform,
                Component comp => comp == null ? null : comp.transform,
                _ => throw new NotSupportedException()
            };

            var selfType = property.GetPropertyField().fieldInfo.FieldType;
            var selfObject = property.objectReferenceValue switch
            {
                GameObject obj => obj,
                Component comp => comp.gameObject,
                null => null,
                _ => throw new NotSupportedException()
            };
            var children = Enumerable.Range(0, parentTransform == null ? 0 : parentTransform.childCount).Select(i => parentTransform.GetChild(i).gameObject);
            if (typeof(Component).IsAssignableFrom(selfType)) children = children.Where(child => child.GetComponent(selfType) != null);

            var options = children.ToArray();
            var optionIndex = Array.IndexOf(options, selfObject);
            if (optionIndex < 0) optionIndex = 0;
            optionIndex = EditorGUI.Popup(position, label.text, optionIndex, options.Select(o => o.name).ToArray());

            if (optionIndex >= 0 && optionIndex < options.Length)
            {
                property.objectReferenceValue = options[optionIndex];
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                property.objectReferenceValue = null;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}