using System;
using System.Reflection;

namespace EntitiesBT.Core
{
    public enum BehaviorNodeType
    {
        Composite,
        Decorate,
        Action,
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class BehaviorNodeAttribute : Attribute
    {
        public Guid Guid { get; } 
        public int Id { get; }
        public BehaviorNodeType Type { get; }
        public string ResetFunc { get; }
        public string TickFunc { get; }
        public string TypesField { get; }
        
        public BehaviorNodeAttribute(string guid, BehaviorNodeType type = BehaviorNodeType.Action, string tickFunc = "Tick", string resetFunc = "Reset", string typesField = "Types")
        {
            Type = type;
            Guid = new Guid(guid);
            Id = Guid.GetHashCode();
            TickFunc = tickFunc;
            ResetFunc = resetFunc;
            TypesField = typesField;
        }
    }

    public static class BehaviorNodeAttributeExtensions
    {
        public static BehaviorNodeAttribute GetBehaviorNodeAttribute(this Type type)
        {
            return (BehaviorNodeAttribute) type.GetCustomAttribute(typeof(BehaviorNodeAttribute));
        }
    }
}
