// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using EntitiesBT.Variable;
// using UnityEditor;
// using UnityEngine;
//
// namespace EntitiesBT.Editor
// {
//     [AttributeUsage(AttributeTargets.Class)]
//     public class VariablePropertyDrawerAttribute : Attribute
//     {
//         public Type VariableType;
//
//         public VariablePropertyDrawerAttribute(Type variableType)
//         {
//             VariableType = variableType;
//         }
//     }
//     
//     [CustomPropertyDrawer(typeof(VariableProperty<>), false)]
//     public class VariableDrawer : PropertyDrawer
//     {
//         // public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
//         // {
//             // var variableTypeProperty = property.FindPropertyRelative("ValueSource");
//             // var lines = 0;
//             // switch ((ValueSource) variableTypeProperty.enumValueIndex)
//             // {
//             // case ValueSource.CustomConstant:
//             //     lines = 3;
//             //     break;
//             // // case ValueSource.ConstantUnityComponent:
//             // //     lines = 5;
//             // //     break;
//             // // case ValueSource.ConstantScriptableObject:
//             // //     lines = 7;
//             // //     break;
//             // // case ValueSource.ConstantNode:
//             // //     lines = 5;
//             // //     break;
//             // // case ValueSource.DynamicComponent:
//             // //     lines = 5;
//             // //     break;
//             // // case ValueSource.DynamicScriptableObject:
//             // //     lines = 7;
//             // //     break;
//             // // case ValueSource.DynamicNode:
//             // //     lines = 5;
//             // //     break;
//             // default:
//             //     lines = 8;
//             //     break;
//             // }
//             // var lines = 10;
//             // return EditorGUIUtility.singleLineHeight * lines + 6;
//         // }
//         
//         // public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         // {
//             // Using BeginProperty / EndProperty on the parent property means that
//             // __prefab__An asset type that allows you to store a GameObject complete with components and properties. The prefab acts as a template from which you can create new object instances in the scene.  [More info](Prefabs.html)<span class="tooltipGlossaryLink">See in [Glossary](Glossary.html#Prefab)</span> override logic works on the entire property.
//             // EditorGUI.BeginProperty(position, label, property);
//             // EditorGUI.PropertyField(LineRect(1), property);
//             // property.managedReferenceValue = Activator.In;
//             //
//             // // Draw label
//             // EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
//             // // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
//             //
//             // var labelWidth = EditorGUIUtility.labelWidth;
//             // EditorGUIUtility.labelWidth = 180;
//             // var indentLevel = EditorGUI.indentLevel;
//             // EditorGUI.indentLevel++;
//             //
//             // var variableTypeProperty = property.FindPropertyRelative("ValueSource");
//             // EditorGUI.PropertyField(LineRect(1), variableTypeProperty);
//             //
//             // switch ((ValueSource) variableTypeProperty.enumValueIndex)
//             // {
//             // case ValueSource.CustomConstant:
//             // {
//             //     EditorGUI.PropertyField(LineRect(2), property.FindPropertyRelative("ConstantValue"));
//             //     break;
//             // }
//             // case ValueSource.ComponentDynamic:
//             // {
//             //     EditorGUI.PropertyField(LineRect(2), property.FindPropertyRelative("ConstantValue"), new GUIContent("Fallback"));
//             //     var componentTypeNameProperty = property.FindPropertyRelative("ComponentTypeName");
//             //     var componentValueNameProperty = property.FindPropertyRelative("ComponentValueName");
//             //     var (hash, offset, valueType) = Utility.GetTypeHashAndFieldOffset(
//             //         componentTypeNameProperty.stringValue, componentValueNameProperty.stringValue
//             //     );
//             //     var color = GUI.color;
//             //     if (hash == 0 || offset < 0 || valueType != property.GetGenericType())
//             //         GUI.color = Color.red;
//             //     EditorGUI.PropertyField(LineRect(3), componentTypeNameProperty);
//             //     EditorGUI.PropertyField(LineRect(4), componentValueNameProperty);
//             //     GUI.color = color;
//             //     break;
//             // }
//             // case ValueSource.ScriptableObjectConstant:
//             // {
//             //     EditorGUI.PropertyField(LineRect(2), property.FindPropertyRelative("ConstantValue"), new GUIContent("Fallback"));
//             //     var scriptableObjectProperty = property.FindPropertyRelative("ScriptableObject");
//             //     var valueNameProperty = property.FindPropertyRelative("ScriptableObjectValueName");
//             //     var color = GUI.color;
//             //     var config = scriptableObjectProperty.objectReferenceValue;
//             //     if (config == null) GUI.color = Color.red;
//             //     EditorGUI.PropertyField(LineRect(3), scriptableObjectProperty);
//             //     GUI.color = color;
//             //
//             //     var valueName = valueNameProperty.stringValue;
//             //     var valueInfo = config?.GetType().GetField(valueName, VariableProperty.FIELD_BINDING_FLAGS);
//             //     if (valueInfo == null || valueInfo.FieldType != property.GetGenericType()) GUI.color = Color.red;
//             //     EditorGUI.PropertyField(LineRect(4), valueNameProperty);
//             //     GUI.color = color;
//             //     break;
//             // }
//             // default:
//             // {
//             //     EditorGUI.TextArea(LineRect(2), "Not Implemented");
//             //     break;
//             // }
//             // }
//             //
//             // EditorGUIUtility.labelWidth = labelWidth;
//             // EditorGUI.indentLevel = indentLevel;
//             //
//             // EditorGUI.EndProperty();
//             //
//             // Rect LineRect(int lineNumber) =>
//             //     new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * lineNumber, position.width, 16);
//         // }
//         
//         
//         private static readonly Dictionary<Type, PropertyDrawer> _PROPERTY_DRAWERS;
//
//         static VariableDrawer()
//         {
//             _PROPERTY_DRAWERS = new Dictionary<Type, PropertyDrawer>();
//             foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
//                 .SelectMany(assembly => assembly.GetTypes()))
//             {
//                 var attribute = type.GetCustomAttribute<VariablePropertyDrawerAttribute>();
//                 if (attribute == null) continue;
//                 _PROPERTY_DRAWERS[attribute.VariableType] = (PropertyDrawer)Activator.CreateInstance(type);
//             }
//         }
//          
//         
//         public PropertyDrawer GetPolyPropertyDrawer(SerializedProperty property)
//         {
//             return new CustomVariablePropertyDrawer();
//             // var instance = typeof(CustomVariableProperty<>).MakeGenericType(property.GetFieldInfo().GetGenericType());
//             // ScriptAttributeUtility.GetFieldInfoAndStaticTypeFromProperty(this, out var type);
//             // if (value != null && !type.IsAssignableFrom(value.GetType()))
//             //   throw new InvalidOperationException($"Cannot assign an object of type '{value.GetType().Name}' to a managed reference with a base type of '{type.Name}': types are not compatible");
//             // property.Verify(SerializedProperty.VerifyFlags.IteratorNotAtEnd);
//             // property.SetManagedReferenceValueInternal(value);
//         }
//         //
//         // void Set(SerializedProperty property, object value)
//         // {
//         // }
//         
//  
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             return EditorGUIUtility.singleLineHeight * 3 + 6;
//             // var drawer = GetPolyPropertyDrawer(property);
//             // return drawer?.GetPropertyHeight(property, label) ?? base.GetPropertyHeight(property, label);
//         }
//  
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             var fieldInfo = property.GetFieldInfo();
//             var variable = property.GetVariable(fieldInfo);
//             EditorGUI.TextField(LineRect(1), variable.GetType().ToString());
//             // if (variable.GetType().GetGenericTypeDefinition() == typeof(VariableProperty<>))
//             // {
//             //     var customVariable = fieldInfo.CreateVariable(typeof(CustomVariableProperty<>));
//             //     fieldInfo.SetValue(property.serializedObject.targetObject, customVariable);
//             //     EditorGUI.TextField(LineRect(2), customVariable.GetType().ToString());
//             // }
//             //
//             // var drawer = GetPolyPropertyDrawer(property);
//             // if (drawer == null)
//             //     EditorGUI.HelpBox(position, "Cannot find drawer for type " + property.objectReferenceValue.GetType(), MessageType.Error);
//             // else
//             //     drawer.OnGUI(position, property, label);   
//             
//             Rect LineRect(int lineNumber) =>
//                 new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * lineNumber, position.width, 16);
//         }
//     }
//     
//     public static class PropertyExtensions
//     {
//         private static MethodInfo _PROPERTY_VERIFY_METHOD;
//         private static MethodInfo _PROPERTY_SET_VALUE_METHOD;
//
//         static PropertyExtensions()
//         {
//             const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
//             _PROPERTY_VERIFY_METHOD = typeof(SerializedProperty).GetMethod("Verify", flags);
//             _PROPERTY_SET_VALUE_METHOD = typeof(SerializedProperty).GetMethod("SetManagedReferenceValueInternal", flags);
//         }
//         
//         public static FieldInfo GetFieldInfo(this SerializedProperty property)
//         {
//             return property.serializedObject.targetObject.GetType()
//                 .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
//                 .Where(field => field.Name == property.propertyPath)
//                 // .Select(field => field.FieldType)
//                 // .Where(type => type.IsGenericType && type.Name == property.type)
//                 // .SelectMany(type => type.GetGenericArguments())
//                 .FirstOrDefault()
//             ;
//         }
//
//         public static Type GetGenericType(this FieldInfo fieldInfo) =>
//             fieldInfo.FieldType.GenericTypeArguments.FirstOrDefault();
//
//         public static VariableProperty GetVariable(this SerializedProperty property, FieldInfo fieldInfo)
//         {
//             return (VariableProperty)fieldInfo.GetValue(property.serializedObject.targetObject);
//         }
//         
//         // public static void SetVariable(this SerializedProperty property, FieldInfo fieldInfo, object variable)
//         // {
//         //     fieldInfo.SetValue(property.serializedObject.targetObject, variable);
//         //     // SerializedProperty.VerifyFlags.IteratorNotAtEnd == 2
//         //     // _PROPERTY_VERIFY_METHOD.Invoke(property, new object[] { 2 });
//         //     // _PROPERTY_SET_VALUE_METHOD.Invoke(property, new[] { variable });
//         // }
//         
//         public static object CreateVariable(this FieldInfo fieldInfo, Type variablePropertyType)
//         {
//             var type = variablePropertyType.MakeGenericType(fieldInfo.GetGenericType());
//             return Activator.CreateInstance(type);
//         }
//     }
// }
