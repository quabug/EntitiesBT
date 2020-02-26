using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using EntitiesBT.Entities;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    [CustomPropertyDrawer(typeof(Variable<>))]
    public class VariableDrawer : PropertyDrawer
    {
        private static Lazy<Dictionary<Type, Dictionary<string, (ulong hash, int offset)>>> _validComponents =
            new Lazy<Dictionary<Type, Dictionary<string, (ulong hash, int offset)>>>(() =>
            {
                var types =
                    from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.IsValueType && typeof(IComponentData).IsAssignableFrom(type)
                    select (type, hash: TypeHash.CalculateStableTypeHash(type))
                ;
                
                var typeFields =
                    from t in types
                    from field in t.type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                    where field.IsLiteral == false
                    select (t.type, field, t.hash, offset: Marshal.OffsetOf(t.type, field.Name).ToInt32())
                ;

                return typeFields.GroupBy(t => t.field.FieldType)
                    .ToDictionary(
                        group => group.Key
                      , group => group.ToDictionary(t => $"{t.type.Name}.{t.field.Name}", t => (t.hash, t.offset))
                    )
                ;
            });

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

            property.FindPropertyRelative("CustomValue");
            if ((VariableValueSource) variableTypeProperty.enumValueIndex == VariableValueSource.CustomValue)
            {
                EditorGUI.PropertyField(LineRect(2), property.FindPropertyRelative("CustomValue"));
            }
            else
            {
                var componentStableHashProperty = property.FindPropertyRelative("ComponentStableHash");
                var componentDataOffsetProperty = property.FindPropertyRelative("ComponentDataOffset");
                var componentValueProperty = property.FindPropertyRelative("ComponentValue");
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(LineRect(2), componentValueProperty);
                // Call OnValueChanged callbacks
                if (EditorGUI.EndChangeCheck())
                {
                    var (hash, offset) = GetComponentFromName(property.GetGenericType(), componentValueProperty.stringValue);
                    componentStableHashProperty.longValue = (long)hash;
                    componentDataOffsetProperty.intValue = offset;
                }
                var guiEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.PropertyField(LineRect(3), componentStableHashProperty);
                EditorGUI.PropertyField(LineRect(4), componentDataOffsetProperty);
                GUI.enabled = guiEnabled;
            }

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indentLevel;

            EditorGUI.EndProperty();
            
            Rect LineRect(int lineNumber) =>
                new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * lineNumber, position.width, 16);

            (ulong componentStableHash, int componentDataOffset) GetComponentFromName(Type type, string componentDataName)
            {
                (ulong, int) result = (0, 0);
                // if (_validComponents.Value.Values.Any(dict => dict.TryGetValue(componentDataName, out result)))
                    // return result;
                if (_validComponents.Value.TryGetValue(type, out var dict))
                    dict.TryGetValue(componentDataName, out result);
                return result;
            }
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
