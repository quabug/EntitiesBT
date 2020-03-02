using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.Variable;
using UnityEditor;

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
         
         public static FieldInfo GetFieldInfo(this SerializedProperty property)
         {
             const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
             var fieldName = property.propertyPath.Split('.')[0];
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
         //
         // public static VariableProperty<> GetVariable(this SerializedProperty property, FieldInfo fieldInfo)
         // {
         //     return (VariableProperty)fieldInfo.GetValue(property.serializedObject.targetObject);
         // }
         
         // public static void SetVariable(this SerializedProperty property, FieldInfo fieldInfo, object variable)
         // {
         //     fieldInfo.SetValue(property.serializedObject.targetObject, variable);
         //     // SerializedProperty.VerifyFlags.IteratorNotAtEnd == 2
         //     // _PROPERTY_VERIFY_METHOD.Invoke(property, new object[] { 2 });
         //     // _PROPERTY_SET_VALUE_METHOD.Invoke(property, new[] { variable });
         // }
         //
         // public static object CreateVariable(this FieldInfo fieldInfo, Type variablePropertyType)
         // {
         //     var type = variablePropertyType.MakeGenericType(fieldInfo.GetGenericType());
         //     return Activator.CreateInstance(type);
         // }
    }
}
