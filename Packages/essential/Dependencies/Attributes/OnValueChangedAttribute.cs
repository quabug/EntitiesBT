using System;
using System.Diagnostics;

namespace EntitiesBT.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
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