using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nuwa.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public static class Utilities
    {
         // private static MethodInfo _PROPERTY_VERIFY_METHOD;
         // private static MethodInfo _PROPERTY_SET_VALUE_METHOD;

         // static Utilities()
         // {
         //     const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
         //     _PROPERTY_VERIFY_METHOD = typeof(SerializedProperty).GetMethod(nameof(Verify), flags);
         //     _PROPERTY_SET_VALUE_METHOD = typeof(SerializedProperty).GetMethod(nameof(SetManagedReferenceValueInternal), flags);
         // }

         // private static IDictionary<Type, Type[]> _variableSubclasses = new Dictionary<Type, Type[]>();
         //
         // public static Type[] GetVariableSubclasses(this Type variableType)
         // {
         //     if (!_variableSubclasses.TryGetValue(variableType, out var subclasses))
         //     {
         //         subclasses =
         //         (
         //             from type in Utilities.BehaviorTreeAssemblyTypes
         //             where type.IsGenericType
         //                 ? IsSubclassOfRawGeneric(variableType.GetGenericTypeDefinition(), type)
         //                 : type.IsSubclassOf(variableType)
         //             select type
         //         ).ToArray();
         //         _variableSubclasses[variableType] = subclasses;
         //     }
         //     return subclasses;
         // }
         //
         // // https://stackoverflow.com/a/457708
         // private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck) {
         //     while (toCheck != null && toCheck != typeof(object)) {
         //         var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
         //         if (generic == cur) {
         //             return true;
         //         }
         //         toCheck = toCheck.BaseType;
         //     }
         //     return false;
         // }

        [Pure] internal static string ToCodeName(this string typeName) =>
            typeName.Split('`').First().Replace('+', '.')
        ;

        [Pure] internal static string ToShortNameWithInnerClass(this string typeName) =>
            typeName.Split('`').First().Split('.').Last().Replace("+", "")
        ;


        public static Assembly ToAssembly([NotNull] this AssemblyDefinitionAsset assembly)
        {
            var name = assembly.Deserialize().name;
            return Core.Utilities.ALL_ASSEMBLIES.Value.FirstOrDefault(assembly => assembly.GetName().Name == name);
        }
    }
}
