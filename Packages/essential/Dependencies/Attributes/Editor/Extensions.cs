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
             var pathCount = property.propertyPath.Split('.').Length;
             var obj = property.GetFieldsByPath().ElementAt(pathCount - 1).field;
             var fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
             return fieldInfo.GetValue(obj);
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
    }
}