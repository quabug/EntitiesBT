using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.Variable;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(VariableProperty<>))]
    public class VariableDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            var variableTypeProperty = property.FindPropertyRelative("ValueSource");
            var lines = 0;
            switch ((ValueSource) variableTypeProperty.enumValueIndex)
            {
            case ValueSource.Constant:
                lines = 3;
                break;
            case ValueSource.ConstantComponent:
                lines = 5;
                break;
            case ValueSource.ConstantScriptableObject:
                lines = 7;
                break;
            case ValueSource.ConstantNode:
                lines = 5;
                break;
            case ValueSource.DynamicComponent:
                lines = 5;
                break;
            case ValueSource.DynamicScriptableObject:
                lines = 7;
                break;
            case ValueSource.DynamicNode:
                lines = 5;
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

            switch ((ValueSource) variableTypeProperty.enumValueIndex)
            {
            case ValueSource.Constant:
            {
                EditorGUI.PropertyField(LineRect(2), property.FindPropertyRelative("ConstantValue"));
                break;
            }
            case ValueSource.DynamicComponent:
            {
                EditorGUI.PropertyField(LineRect(2), property.FindPropertyRelative("ConstantValue"), new GUIContent("Fallback"));
                var componentValueProperty = property.FindPropertyRelative("ComponentValue");
                var (hash, offset, valueType) = Utility.GetTypeHashAndFieldOffset(componentValueProperty.stringValue);
                var color = GUI.color;
                if (hash == 0 || offset < 0 || valueType != property.GetGenericType())
                    GUI.color = Color.red;
                EditorGUI.PropertyField(LineRect(3), componentValueProperty);
                GUI.color = color;
                break;
            }
            case ValueSource.ConstantScriptableObject:
            {
                EditorGUI.PropertyField(LineRect(2), property.FindPropertyRelative("ConstantValue"), new GUIContent("Fallback"));
                var scriptableObjectProperty = property.FindPropertyRelative("ScriptableObject");
                var valueNameProperty = property.FindPropertyRelative("ScriptableObjectValueName");
                var color = GUI.color;
                var config = scriptableObjectProperty.objectReferenceValue;
                if (config == null) GUI.color = Color.red;
                EditorGUI.PropertyField(LineRect(3), scriptableObjectProperty);
                GUI.color = color;

                var valueName = valueNameProperty.stringValue;
                var valueInfo = config?.GetType().GetField(valueName, BindingFlags.Public | BindingFlags.NonPublic);
                if (valueInfo == null || valueInfo.FieldType != property.GetGenericType()) GUI.color = Color.red;
                EditorGUI.PropertyField(LineRect(4), valueNameProperty);
                GUI.color = color;
                break;
            }
            default:
            {
                EditorGUI.TextArea(LineRect(2), "Not Implemented");
                break;
            }
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
