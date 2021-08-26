using System;
using JetBrains.Annotations;

namespace Nuwa
{
    [AttributeUsage(AttributeTargets.Field)]
    [BaseTypeRequired(typeof(string))]
    public class SerializedTypeAttribute : MultiPropertyAttribute
    {
        public Type BaseType;
        public SerializedTypeAttribute(Type baseType = null) => BaseType = baseType;
    }
}
