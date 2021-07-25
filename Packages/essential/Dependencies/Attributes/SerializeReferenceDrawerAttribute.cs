using System;

namespace EntitiesBT.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializeReferenceDrawerAttribute : MultiPropertyAttribute
    {
        public SerializeReferenceDrawerAttribute() => order = int.MaxValue;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SerializeReferenceDrawerPropertyBaseTypeAttribute : Attribute
    {
        public string PropertyName { get; }
        public SerializeReferenceDrawerPropertyBaseTypeAttribute(string propertyName) => PropertyName = propertyName;
    }
}
