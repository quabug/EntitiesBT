using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace EntitiesBT.Attributes.Editor
{
    public static class Extensions
    {
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

         public static object GetDeclaringObject(this SerializedProperty property)
         {
             var pathCount = property.propertyPath.Split('.').Length;
             return property.GetFieldsByPath().ElementAt(pathCount - 1).field;
         }

         public static bool IsReferencedArrayElement(this SerializedProperty property)
         {
             return property.propertyType == SerializedPropertyType.ManagedReference && property.propertyPath.EndsWith("]");
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
    }
}