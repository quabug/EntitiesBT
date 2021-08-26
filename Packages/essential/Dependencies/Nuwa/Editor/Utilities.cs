using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Nuwa.Editor
{
    public static class Utilities
    {
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
    }
}