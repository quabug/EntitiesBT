using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Variable;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public static class Utility
    {
         private static MethodInfo _PROPERTY_VERIFY_METHOD;
         private static MethodInfo _PROPERTY_SET_VALUE_METHOD;

         static Utility()
         {
             const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
             _PROPERTY_VERIFY_METHOD = typeof(SerializedProperty).GetMethod("Verify", flags);
             _PROPERTY_SET_VALUE_METHOD = typeof(SerializedProperty).GetMethod("SetManagedReferenceValueInternal", flags);
         }
         
         public static (object field, FieldInfo fieldInfo) GetTargetField(this SerializedProperty property)
         {
             return property.GetFieldsByPath().ElementAt(1);
         }
         
         public static (object field, FieldInfo fieldInfo) GetPropertyField(this SerializedProperty property)
         {
             return property.GetFieldsByPath().Last();
         }
         
         public static FieldInfo GetTargetFieldInfo(this SerializedProperty property)
         {
             return property.GetFieldsByPath().ElementAt(1).fi;
         }

         public static IEnumerable<(object field, FieldInfo fi)> GetFieldsByPath(this SerializedProperty property)
         {
             var obj = (object)property.serializedObject.targetObject;
             yield return (obj, null);
             foreach (var fieldName in property.propertyPath.Split('.'))
             {
                 var t = Field(obj, fieldName);
                 obj = t.field;
                 yield return t;
             }

             (object field, FieldInfo fi) Field(object @object, string fieldName)
             {
                 var fieldInfo = @object.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                 var fieldValue = fieldInfo.GetValue(@object);
                 return (fieldValue, fieldInfo);
             }
         }
         
         public static Type GetGenericType(this PropertyDrawer propertyDrawer)
         {
             return propertyDrawer.fieldInfo.DeclaringType.GetGenericType();
         }
         
         public static T GetCustomAttribute<T>(this SerializedProperty property) where T : Attribute
         {
             var (_, fieldInfo) = property.GetPropertyField();
             return fieldInfo.GetCustomAttribute<T>();
         }
         
         public static FieldInfo GetTargetFieldInfo(this SerializedProperty property, string fieldName)
         {
             const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
             return property.serializedObject.targetObject.GetType().GetField(fieldName, flags);
         }

         public static object GetSiblingFieldValue(this SerializedProperty property, string fieldName)
         {
             var pathCount = property.propertyPath.Split('.').Length;
             var obj = property.GetFieldsByPath().ElementAt(pathCount - 1).field;
             var fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
             return fieldInfo.GetValue(obj);
         }

         public static Type GetGenericType(this Type type)
         {
             while (type != null)
             {
                 if (type.IsGenericType) return type.GenericTypeArguments.FirstOrDefault();
                 type = type.BaseType;
             }
             return null;
         }

         public static Func<Rect, string, string[], int> PopupFunc(this SerializedProperty property)
         {
             return (position, label, options) =>
             {
                 var optionIndex = Array.IndexOf(options, property.stringValue);
                 if (optionIndex < 0) optionIndex = 0;
                 optionIndex = EditorGUI.Popup(position, label, optionIndex, options);
                 property.stringValue = optionIndex < options.Length ? options[optionIndex] : "";
                 return optionIndex;
             };
         }

         private static IDictionary<Type, Type[]> _variableSubclasses = new Dictionary<Type, Type[]>();

         public static Type[] GetVariableSubclasses(this Type variableType)
         {
             if (!_variableSubclasses.TryGetValue(variableType, out var subclasses))
             {
                 subclasses =
                 (
                     from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where type.IsGenericType
                         ? IsSubclassOfRawGeneric(variableType.GetGenericTypeDefinition(), type)
                         : type.IsSubclassOf(variableType)
                     select type
                 ).ToArray();
                 _variableSubclasses[variableType] = subclasses;
             }
             return subclasses;
         }
         
         // https://stackoverflow.com/a/457708
         private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck) {
             while (toCheck != null && toCheck != typeof(object)) {
                 var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                 if (generic == cur) {
                     return true;
                 }
                 toCheck = toCheck.BaseType;
             }
             return false;
         }
    }
}
