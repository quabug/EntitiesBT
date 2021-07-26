using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace EntitiesBT.Core
{
    public enum BehaviorNodeType
    {
        Composite,
        Decorate,
        Action,
    }
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    [BaseTypeRequired(typeof(INodeData))]
    public class BehaviorNodeAttribute : PreserveAttribute
    {
        public Guid Guid { get; } 
        public int Id { get; }
        public BehaviorNodeType Type { get; }
        public bool Ignore = false;

        public BehaviorNodeAttribute([NotNull] string guid, BehaviorNodeType type = BehaviorNodeType.Action)
        {
            Type = type;
            Guid = Guid.Parse(guid);
            Id = Guid.GetHashCode();
        }
    }

    public static class BehaviorNodeAttributeExtensions
    {
        public static BehaviorNodeAttribute GetBehaviorNodeAttribute([NotNull] this Type type)
        {
            return (BehaviorNodeAttribute) type.GetCustomAttribute(typeof(BehaviorNodeAttribute));
        }
    }
}
