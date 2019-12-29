using System;
using System.Reflection;

namespace EntitiesBT.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BehaviorNodeAttribute : Attribute
    {
        public Guid Guid { get; } 
        public int Id { get; }
        public BehaviorNodeAttribute(string guid)
        {
            Guid = new Guid(guid);
            Id = Guid.GetHashCode();
        }
    }

    public static class BehaviorNodeAttributeExtensions
    {
        public static int GetBehaviorNodeId(this Type type)
        {
            return ((BehaviorNodeAttribute) type.GetCustomAttribute(typeof(BehaviorNodeAttribute))).Id;
        }
    }
}
