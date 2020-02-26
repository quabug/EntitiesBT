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
            var lines = 0;
            switch ((VariableValueSource) variableTypeProperty.enumValueIndex)
            {
            case VariableValueSource.CustomValue:
                lines = 3;
                break;
            case VariableValueSource.ComponentValue:
                lines = 5;
                break;
            case VariableValueSource.ScriptableObjectValue:
                lines = 6;
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
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

            switch ((VariableValueSource) variableTypeProperty.enumValueIndex)
            {
            case VariableValueSource.CustomValue:
            {
                EditorGUI.PropertyField(LineRect(2), property.FindPropertyRelative("CustomValue"));
                break;
            }
            case VariableValueSource.ComponentValue:
            {
                EditorGUI.PropertyField(LineRect(2), property.FindPropertyRelative("FallbackValue"));
                var componentValueProperty = property.FindPropertyRelative("ComponentValue");
                var (hash, offset, valueType) = Variable.GetTypeHashAndFieldOffset(componentValueProperty.stringValue);
                var color = GUI.color;
                if (hash == 0 || offset < 0 || valueType != property.GetGenericType())
                    GUI.color = Color.red;
                EditorGUI.PropertyField(LineRect(3), componentValueProperty);
                GUI.color = color;
                break;
            }
            case VariableValueSource.ScriptableObjectValue:
            {
                EditorGUI.PropertyField(LineRect(2), property.FindPropertyRelative("FallbackValue"));
                var configSourceProperty = property.FindPropertyRelative("ConfigSource");
                var configValueNameProerpty = property.FindPropertyRelative("ConfigValueName");
                var color = GUI.color;
                var config = configSourceProperty.objectReferenceValue;
                if (config == null) GUI.color = Color.red;
                EditorGUI.PropertyField(LineRect(3), configSourceProperty);
                GUI.color = color;

                var valueName = configValueNameProerpty.stringValue;
                var valueInfo = config.GetType().GetField(valueName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (valueInfo == null || valueInfo.FieldType != property.GetGenericType()) GUI.color = Color.red;
                EditorGUI.PropertyField(LineRect(4), configValueNameProerpty);
                GUI.color = color;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
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
