using System;

namespace EntitiesBT.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializeReferenceDrawerAttribute : MultiPropertyAttribute
    {
        public string TypeRestrictionBySiblingProperty;
        public SerializeReferenceDrawerAttribute() => order = int.MaxValue;
    }
}
