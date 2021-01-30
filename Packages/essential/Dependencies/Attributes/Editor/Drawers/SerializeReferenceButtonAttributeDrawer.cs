using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Attributes.Editor
{
    [CustomMultiPropertyDrawer(typeof(SerializeReferenceButtonAttribute))]
    public class SerializeReferenceButtonAttributeDrawer : BaseMultiPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var labelPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelPosition, label);

            // var typeRestrictions = SerializedReferenceUIBuiltInTypeRestrictions.GetAllBuiltInTypeRestrictions(FieldInfo);
            // DrawSelectionButtonForManagedReference(property, position, typeRestrictions);

            EditorGUI.PropertyField(position, property, GUIContent.none, true);

            EditorGUI.EndProperty();
        }

        private void DrawSelectionButtonForManagedReference(SerializedProperty property, Rect position, IEnumerable<Func<Type, bool>> filters = null)
        {
            // //var backgroundColor = new Color(0f, 0.7f, 0.7f, 1f);
            // var backgroundColor = new Color(0.1f, 0.45f, 0.8f, 1f);
            // //var backgroundColor = GUI.backgroundColor;
            //
            // var buttonPosition = position;
            // buttonPosition.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
            // buttonPosition.width = position.width - EditorGUIUtility.labelWidth - 1 * EditorGUIUtility.standardVerticalSpacing;
            // buttonPosition.height = EditorGUIUtility.singleLineHeight;
            //
            // var storedIndent = EditorGUI.indentLevel;
            // EditorGUI.indentLevel = 0;
            // var storedColor = GUI.backgroundColor;
            // GUI.backgroundColor = backgroundColor;
            //
            // var names = SerializeReferenceTypeNameUtility.GetSplitNamesFromTypename(property.managedReferenceFullTypename);
            // var className = string.IsNullOrEmpty(names.ClassName) ? "Null (Assign)" : names.ClassName;
            // var assemblyName = names.AssemblyName;
            // if (GUI.Button(buttonPosition, new GUIContent(className.Split('.').Last(), className + "  ( "+ assemblyName +" )" )))
            //     property.ShowContextMenuForManagedReference(filters);
            //
            // GUI.backgroundColor = storedColor;
            // EditorGUI.indentLevel = storedIndent;
        }
    }
}