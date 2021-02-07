using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.Attributes.Editor;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(VariantNodeObjectAttribute))]
    public class VariantNodeObjectAttributeDrawer : PropertyDrawer
    {
        private INodeDataBuilder _nodeObject;
        private string[] _options = new string[0];
        private Type _genericType;
        private VariantNodeObjectAttribute _attribute;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (_genericType == null) _genericType = this.GetGenericType();
                if (_attribute == null) _attribute = (VariantNodeObjectAttribute)attribute;
                var nodeObject = (INodeDataBuilder)property.GetSiblingFieldValue(_attribute.NodeObjectFieldName);
                if (!Equals(nodeObject, _nodeObject))
                {
                    var readerType = typeof(BlobVariantReader<>).MakeGenericType(_genericType);
                    var rwType = typeof(BlobVariantReaderAndWriter<>).MakeGenericType(_genericType);
                    _nodeObject = nodeObject;
                    _options = nodeObject == null
                        ? new string[0]
                        : VirtualMachine.GetNodeType(nodeObject.NodeId)
                            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(fi => fi.FieldType == _genericType || fi.FieldType == readerType || fi.FieldType == rwType)
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
