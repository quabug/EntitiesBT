using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;

namespace EntitiesBT.Attributes.Editor
{
    public static class Extensions
    {
         public static object GetSiblingValue(this SerializedProperty property, string name)
         {
             var obj = GetDeclaringObject(property);
             var type = obj.GetType();
             var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
             var fieldInfo = type.GetField(name, flags);
             if (fieldInfo != null) return fieldInfo.GetValue(obj);
             var propertyInfo = type.GetProperty(name, flags);
             if (propertyInfo != null) return propertyInfo.GetValue(obj);
             var methodInfo = type.GetMethod(name, flags);
             return methodInfo.Invoke(obj, Array.Empty<object>());
         }

         public static object GetSiblingFieldValue(this SerializedProperty property, string fieldName)
         {
             var obj = GetDeclaringObject(property);
             var fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
             return fieldInfo.GetValue(obj);
         }

         public static PropertyInfo GetSiblingPropertyInfo(this SerializedProperty property, string propertyName)
         {
             var obj = GetDeclaringObject(property);
             return obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
         }

         public static MethodInfo GetSiblingMethodInfo(this SerializedProperty property, string methodName)
         {
             var obj = GetDeclaringObject(property);
             return obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
         }

         public static object GetDeclaringObject(this SerializedProperty property)
         {
             return property.GetFieldsByPath().Reverse().Skip(1).First().field;
         }

         public static object GetObject(this SerializedProperty property)
         {
             return property.GetFieldsByPath().Last().field;
         }

         public static IEnumerable<(object field, FieldInfo fi)> GetFieldsByPath(this SerializedProperty property)
         {
             var obj = (object)property.serializedObject.targetObject;
             FieldInfo fi = null;
             yield return (obj, fi);
             foreach (var fieldName in property.propertyPath.Split('.'))
             {
                 if (obj.GetType().IsArray)
                 {
                     if (fieldName == "Array") continue;
                     var itemIndex = int.Parse(fieldName.Substring(5 /*"data["*/, fieldName.Length - 6 /*"data[]"*/));
                     obj = ((Array) obj).GetValue(itemIndex);
                     yield return (obj, fi);
                 }
                 else
                 {
                     var t = Field(obj, fieldName);
                     obj = t.field;
                     fi = t.fi;
                     yield return t;
                 }
             }

             (object field, FieldInfo fi) Field(object @object, string fieldName)
             {
                 var fieldInfo = @object.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                 var fieldValue = fieldInfo.GetValue(@object);
                 return (fieldValue, fieldInfo);
             }
         }

         internal static (Regex, string) ParseReplaceRegex(this string pattern, string separator = "||")
         {
             if (string.IsNullOrEmpty(pattern)) return (null, null);
             var patterns = pattern.Split(new [] { separator }, StringSplitOptions.None);
             if (patterns.Length == 2) return (new Regex(patterns[0]), patterns[1]);
             throw new ArgumentException($"invalid number of separator ({separator}) in pattern ({pattern})");
         }

    }
}