using System;
using JetBrains.Annotations;

namespace Nuwa
{
    [AttributeUsage(AttributeTargets.Field)]
    [BaseTypeRequired(typeof(string))]
    public class SerializedTypeAttribute : MultiPropertyAttribute
    {
        public Type BaseType;
        public string RenamePatter;
        public bool DisplayAssemblyName = false;
        public bool AlphabeticalOrder = true;
        public string CategoryName;
        public bool Nullable = true;
        public string Where = "";
        public SerializedTypeAttribute(Type baseType = null) => BaseType = baseType;
    }
}
