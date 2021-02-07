using System;

namespace EntitiesBT.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializeReferenceButtonAttribute : MultiPropertyAttribute
    {
        public SerializeReferenceButtonAttribute() => order = int.MaxValue;
    }
}
