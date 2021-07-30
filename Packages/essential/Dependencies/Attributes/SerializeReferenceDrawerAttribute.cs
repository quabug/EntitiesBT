using System;

namespace EntitiesBT.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializeReferenceDrawerAttribute : MultiPropertyAttribute
    {
        public string TypeRestrictBySiblingProperty;
        public string RenamePatter;
        public bool DisplayAssemblyName = false;
        public bool AlphabeticalOrder = true;
        public string CategoryName;
        public SerializeReferenceDrawerAttribute() => order = int.MaxValue;
    }
}
