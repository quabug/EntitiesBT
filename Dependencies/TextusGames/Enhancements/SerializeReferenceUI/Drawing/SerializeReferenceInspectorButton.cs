#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SerializeReferenceInspectorButton
{
    /// Must be drawn before DefaultProperty in order to receive input
    public static void DrawSelectionButtonForManagedReference(this SerializedProperty property, 
        Rect position, IEnumerable<Func<Type, bool>> filters = null)  
    {
        //var backgroundColor = new Color(0f, 0.7f, 0.7f, 1f);  
        var backgroundColor = new Color(0.1f, 0.45f, 0.8f, 1f);   
        //var backgroundColor = GUI.backgroundColor;      
            
        var buttonPosition = position;
        buttonPosition.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
        buttonPosition.width = position.width - EditorGUIUtility.labelWidth - 1 * EditorGUIUtility.standardVerticalSpacing;
        buttonPosition.height = EditorGUIUtility.singleLineHeight;

        var storedIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        var storedColor = GUI.backgroundColor;
        GUI.backgroundColor = backgroundColor;
         
        
        var names = SerializeReferenceTypeNameUtility.GetSplitNamesFromTypename(property.managedReferenceFullTypename);
        var className = string.IsNullOrEmpty(names.ClassName) ? "Null (Assign)" : names.ClassName;
        var assemblyName = names.AssemblyName;
        if (GUI.Button(buttonPosition, new GUIContent(className.Split('.').Last(), className + "  ( "+ assemblyName +" )" )))
            property.ShowContextMenuForManagedReference(filters);
        
        GUI.backgroundColor = storedColor;
        EditorGUI.indentLevel = storedIndent;
    }
}

#endif