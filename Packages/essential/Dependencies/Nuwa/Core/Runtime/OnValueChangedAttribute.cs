using System;

namespace Nuwa
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