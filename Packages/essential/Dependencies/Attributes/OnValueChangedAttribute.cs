using System;

namespace EntitiesBT.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class OnValueChangedAttribute : MultiPropertyAttribute
    {
        public string MethodName { get; }
        public OnValueChangedAttribute(string methodName)
        {
            order = int.MinValue;
            MethodName = methodName;
        }
    }
}