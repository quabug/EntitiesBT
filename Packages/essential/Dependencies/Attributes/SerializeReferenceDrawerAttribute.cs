using System;
using System.Diagnostics;

namespace EntitiesBT.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
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
