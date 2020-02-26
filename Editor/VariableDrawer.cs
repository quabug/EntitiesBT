using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.Entities;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(Variable<>))]
    public class VariableDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            var variableTypeProperty = property.FindPropertyRelative("ValueSource");
            var lines = (VariableValueSource) variableTypeProperty.enumValueIndex == VariableValueSource.CustomValue ? 3 : 5;
            return EditorGUIUtility.singleLineHeight * lines + 6;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // __prefab__An asset type that allows you to store a GameObject complete with components and properties. The prefab acts as a template from which you can create new object instances in the scene.  [More info](Prefabs.html)<span class="tooltipGlossaryLink">See in [Glossary](Glossary.html#Prefab)</span> override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 180;
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;
            
            var variableTypeProperty = property.FindPropertyRelative("ValueSource");
            EditorGUI.PropertyField(LineRect(1), variableTypeProperty);

            EditorGUI.PropertyField(LineRect(2), property.FindPropertyRelative("CustomValue"));
            if ((VariableValueSource) variableTypeProperty.enumValueIndex == VariableValueSource.ComponentValue)
            {
                var componentValueProperty = property.FindPropertyRelative("ComponentValue");
                // EditorGUI.BeginChangeCheck();
                var (hash, offset, valueType) = Variable.GetTypeHashAndFieldOffset(componentValueProperty.stringValue);
                var color = GUI.color;
                if (hash == 0 || offset < 0 || valueType != property.GetGenericType())
                    GUI.color = Color.red;
                EditorGUI.PropertyField(LineRect(3), componentValueProperty);
                GUI.color = color;
                // Call OnValueChanged callbacks
                // if (EditorGUI.EndChangeCheck())
                // {
                //     var (hash, offset, valueType) = Variable.GetTypeHashAndFieldOffset(componentValueProperty.stringValue);
                // }
            }

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indentLevel;

            EditorGUI.EndProperty();
            
            Rect LineRect(int lineNumber) =>
                new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * lineNumber, position.width, 16);
        }
    }

    public static class PropertyExtensions
    {
        public static Type GetGenericType(this SerializedProperty property)
        {
            return property.serializedObject.targetObject.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.Name == property.propertyPath)
                .Select(field => field.FieldType)
                .Where(type => type.IsGenericType && type.Name == property.type)
                .SelectMany(type => type.GetGenericArguments())
                .FirstOrDefault()
            ;
        }
    }
}
