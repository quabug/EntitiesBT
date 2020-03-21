using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(VariableNodeObjectAttribute))]
    public class VariableNodeObjectAttributeDrawer : PropertyDrawer
    {
        private BTNode _nodeObject;
        private string[] _options = new string[0];
        private Type _genericType;
        private VariableNodeObjectAttribute _attribute;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (_genericType == null) _genericType = this.GetGenericType();
                if (_attribute == null) _attribute = (VariableNodeObjectAttribute)attribute;
                var nodeObject = (BTNode)property.GetSiblingFieldValue(_attribute.NodeObjectFieldName);
                if (!Equals(nodeObject, _nodeObject))
                {
                    _nodeObject = nodeObject;
                    _options = nodeObject == null
                        ? new string[0]
                        : VirtualMachine.GetNodeType(nodeObject.NodeId)
                            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(fi => fi.FieldType == _genericType || fi.FieldType == typeof(BlobVariable<>).MakeGenericType(_genericType))
                            .Select(fi => fi.Name)
                            .ToArray()
                    ;
                }
                property.PopupFunc()(position, label.text, _options);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}
