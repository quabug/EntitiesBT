using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using UnityEditor.Callbacks;
using UnityEngine;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Editor
{
    public static class BehaviorNodeValidator
    {
        [DidReloadScripts]
        public static void OnReload()
        {
            var dictionary = new Dictionary<int, (Type type, BehaviorNodeAttribute attribute)>(128);
            foreach (var type in BEHAVIOR_TREE_ASSEMBLY_TYPES.Value)
            {
                if (type.IsInterface
                    || type.IsAbstract
                    || type.IsGenericType
                    || !typeof(INodeData).IsAssignableFrom(type))
                    continue;

                var attributes = type.GetCustomAttributes(typeof(BehaviorNodeAttribute));
                if (!attributes.Any())
                {
                    Debug.LogError($"{type.FullName} must have a BehaviorNodeAttribute");
                    continue;
                }

                var behaviorNodeAttribute = attributes.First() as BehaviorNodeAttribute;
                if (dictionary.TryGetValue(behaviorNodeAttribute.Id, out var other))
                {
                    Debug.LogError($"{other.type.FullName} has same id {behaviorNodeAttribute.Id} with {type.FullName}");
                    continue;
                }
                dictionary.Add(behaviorNodeAttribute.Id, (type, behaviorNodeAttribute));
            }
        }
    }
    
}
