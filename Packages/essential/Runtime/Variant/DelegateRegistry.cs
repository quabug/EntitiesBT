using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using JetBrains.Annotations;
using UnityEngine;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Variant
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterDelegateClassAttribute : Attribute
    {
        public string Guid { get; }
        public RegisterDelegateClassAttribute([NotNull] string guid) => Guid = guid;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RegisterDelegateMethodAttribute : Attribute
    {
        public Type DelegateType { get; }
        public string OverrideGuid = null;
        public RegisterDelegateMethodAttribute([NotNull] Type delegateType) => DelegateType = delegateType;
    }

    internal static class DelegateRegistry
    {
        internal static readonly IReadOnlyDictionary<Type, IReadOnlyDictionary<int, MethodInfo>> DELEGATE_METHOD_MAP;

        static DelegateRegistry()
        {
            var methodFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            if (Application.isEditor || Debug.isDebugBuild) methodFlags |= BindingFlags.Instance;

            var map = new Dictionary<Type, Dictionary<int, MethodInfo>>();
            foreach (var (guid, methodInfo, delegateType) in
                from type in typeof(RegisterDelegateMethodAttribute).Assembly.GetTypesIncludeReference()
                from classAttribute in type.GetCustomAttributes<RegisterDelegateClassAttribute>()
                from method in type.GetMethods(methodFlags)
                from methodAttribute in method.GetCustomAttributes<RegisterDelegateMethodAttribute>()
                select (methodAttribute.OverrideGuid ?? classAttribute.Guid, method, methodAttribute.DelegateType)
            )
            {
                if (Validate(guid, methodInfo, delegateType, out var key, out var delegateIdMethodMap))
                {
                    if (delegateIdMethodMap == null)
                    {
                        delegateIdMethodMap = new Dictionary<int, MethodInfo>();
                        map[delegateType] = delegateIdMethodMap;
                    }
                    delegateIdMethodMap[key] = methodInfo;
                }
            }

            DELEGATE_METHOD_MAP = new ReadOnlyDictionary<Type, IReadOnlyDictionary<int, MethodInfo>>(
                map.Select(t => (t.Key, Value: new ReadOnlyDictionary<int, MethodInfo>(t.Value)))
                    .ToDictionary(t => t.Key, t => (IReadOnlyDictionary<int, MethodInfo>)t.Value)
            );

            bool Validate(string guid, MethodInfo methodInfo, Type delegateType, out int key, out Dictionary<int, MethodInfo> delegateIdMethodMap)
            {
                key = 0;
                delegateIdMethodMap = null;

                if (!methodInfo.IsStatic)
                {
                    Debug.LogError($"Non-static method {methodInfo.Name} {guid} is not support");
                    return false;
                }

                if (!delegateType.IsSubclassOf(typeof(MulticastDelegate)))
                {
                    Debug.LogError($"{delegateType.FullName} on {methodInfo.Name} is not a delegate.");
                    return false;
                }

                if (!Guid.TryParse(guid, out var id))
                {
                    Debug.LogError($"Invalid guid {guid} of delegate method {methodInfo.Name}");
                    return false;
                }

                key = GuidHashCode(id);
                if (map.TryGetValue(delegateType, out delegateIdMethodMap) && delegateIdMethodMap.TryGetValue(key, out var duplicateMethod))
                {
                    Debug.LogError($"Duplicate guid {guid} of delegate method {methodInfo.Name} and {duplicateMethod.Name}");
                    return false;
                }

                // TODO: check the compability of `methodInfo` and `delegateType`
                // if (!methodInfo.IsDelegate(delegateType))
                // {
                //     Debug.LogError($"{methodInfo.Name} cannot match delegate {delegateType.FullName}");
                //     return false;
                // }

                return true;
            }
        }
    }

    public static class DelegateRegistry<TDelegate> where TDelegate : Delegate
    {
        internal static readonly IReadOnlyDictionary<int, TDelegate> DELEGATES;

        static DelegateRegistry()
        {
            IReadOnlyDictionary<int, MethodInfo> delegateMap = null;
            IReadOnlyDictionary<int, MethodInfo> genericDelegateMap = null;
            var delegateType = typeof(TDelegate);
            DelegateRegistry.DELEGATE_METHOD_MAP.TryGetValue(delegateType, out delegateMap);
            if (delegateType.IsGenericType)
                DelegateRegistry.DELEGATE_METHOD_MAP.TryGetValue(delegateType, out genericDelegateMap);

            var count = (delegateMap?.Count ?? 0) + (genericDelegateMap?.Count ?? 0);
            var map = new Dictionary<int, TDelegate>(count);

            var genericArguments = delegateType.GetMethod("Invoke").GetGenericArguments();
            FillMap(genericDelegateMap, methodInfo => methodInfo.MakeGenericMethod(genericArguments));
            FillMap(delegateMap, methodInfo => methodInfo);
            DELEGATES = new ReadOnlyDictionary<int, TDelegate>(map);

            void FillMap(IReadOnlyDictionary<int, MethodInfo> idMethodMap, Func<MethodInfo, MethodInfo> createConcreteMethodInfo)
            {
                if (idMethodMap == null) return;

                foreach (var t in idMethodMap)
                {
                    if (map.TryGetValue(t.Key, out var oldDelegate))
                        Debug.Log($"Overwrite {delegateType.Name}[{t.Key}] from {oldDelegate.Method} to {t.Value}");

                    try
                    {
                        var @delegate =
                            (TDelegate) createConcreteMethodInfo(t.Value).CreateDelegate(typeof(TDelegate));
                        map[t.Key] = @delegate;
                    }
                    catch
                    {
                        Debug.LogError($"Cannot create delegate {delegateType.Name} from {t.Value.Name}");
                    }
                }
            }
        }

        [CanBeNull] public static TDelegate TryGetValue(int id)
        {
            DELEGATES.TryGetValue(id, out var value);
            return value;
        }
    }
}