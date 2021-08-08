using System;

namespace EntitiesBT.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class OnValueChangedAttribute : MultiPropertyAttribute
    {
        public string Callback { get; }
        public string PropertyName;
        public OnValueChangedAttribute(string callback)
        {
            order = int.MinValue;
            Callback = callback;
        }
    }
}