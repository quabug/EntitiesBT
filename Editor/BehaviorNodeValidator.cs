using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using UnityEditor.Callbacks;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public static class BehaviorNodeValidator
    {
        [DidReloadScripts]
        public static void OnReload()
        {
            var dictionary = new Dictionary<int, (Type type, BehaviorNodeAttribute attribute)>(128);
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()))
            {
                var attributes = type.GetCustomAttributes(typeof(BehaviorNodeAttribute));
                if (!attributes.Any()) continue;
                
                var behaviorNodeAttribute = attributes.First() as BehaviorNodeAttribute;
                if (dictionary.TryGetValue(behaviorNodeAttribute.Id, out var other))
                    throw new Exception($"{other.type.FullName} has same id {behaviorNodeAttribute.Id} with {type.FullName}");
                dictionary.Add(behaviorNodeAttribute.Id, (type, behaviorNodeAttribute));
            }
        }
    }
    
}
