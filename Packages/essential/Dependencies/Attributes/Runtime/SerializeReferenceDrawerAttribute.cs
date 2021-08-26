using System;

namespace Nuwa
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializeReferenceDrawerAttribute : MultiPropertyAttribute
    {
        public string TypeRestrictBySiblingProperty;
        public string RenamePatter;
        public bool DisplayAssemblyName = false;
        public bool AlphabeticalOrder = true;
        public string CategoryName;
        public bool Nullable = true;
        public SerializeReferenceDrawerAttribute() => order = int.MaxValue;
    }
}
